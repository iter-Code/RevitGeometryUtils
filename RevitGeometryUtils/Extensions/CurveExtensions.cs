using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static RevitGeometryUtils.iCGeometry;

namespace RevitGeometryUtils.Extensions
{
    public static class CurveExtensions
    {
        //Curve
        public enum CurveEnd
        {
            Start,
            End
        }

        public static double ShortCurveTolerance = 0.00256026455729167;
        public static double AngleTolerance = 0.00174532925199433;
        public static double VertexTolerance = 0.0005233832795;

        public static bool IsBelowLengthTolerance(this Curve curve, double tolerance = 0.00256026455729167)
        {
            return curve.Length <= ShortCurveTolerance ? true : false;
        }
        public static Curve TranslateByVector(this Curve curve, XYZ vector)
        {
            Transform transform = Transform.CreateTranslation(vector);
            Curve translatedCurve = curve.CreateTransformed(transform);

            return translatedCurve;
        }
        public static Curve ProjectOnGlobalPlane(this Curve curve, GlobalPlane globalPlane)
        {
            Curve zPlanifiedCurve;
            string curveTypeName = curve.GetType().Name;

            switch (curveTypeName)
            {
                case "Line":
                    zPlanifiedCurve = ProjectLineOnGlobalPlane(curve as Line, globalPlane);
                    break;

                case "Arc":
                    zPlanifiedCurve = ProjectArcOnGlobalPlane(curve as Arc, globalPlane);
                    break;

                case "Ellipse":
                    Line line = curve as Line;
                    zPlanifiedCurve = ProjectEllipseOnGlobalPlane(curve as Ellipse, globalPlane);
                    break;

                default:
                    //TODO: tratar outras curvas e projetar em outros planos
                    XYZ originalStartPoint = curve.GetEndPoint(0);
                    Transform transform = GetProjectionOnZBasisPlaneTransform(originalStartPoint);
                    zPlanifiedCurve = curve.CreateTransformed(transform);
                    break;
            }

            return zPlanifiedCurve;
        }



        public static List<Curve> ProjectMultipleCurvesOnGlobalPlane(List<Curve> curves, GlobalPlane globalPlane)
        {
            List<Curve> projectedCurves = new List<Curve>();

            foreach (Curve curve in curves)
            {
                try
                {
                    Curve projectedCurve = ProjectCurveOnGlobalPlane(curve, globalPlane);
                    projectedCurves.Add(projectedCurve);
                }
                catch (Autodesk.Revit.Exceptions.ArgumentsInconsistentException)
                {
                    throw;
                }

            }

            return projectedCurves;
        }
        public static List<Curve> PurgeSequentialCurvesSmallLinesAndTrimAdjacent(List<Curve> curveLoopCurves)
        {
            int curveLoopLastIndex = curveLoopCurves.Count - 1;
            List<int> smallCurvesIndexes = GetSmallCurvesIndexes(curveLoopCurves);

            foreach (int smallCurveIndex in smallCurvesIndexes)
            {
                int precedentCurveIndex = GetPrecedentCurveIndexInCurveLoop(smallCurveIndex, curveLoopLastIndex);
                int nextCurveIndex = GetNextCurveIndexInCurveLoop(smallCurveIndex, curveLoopLastIndex);

                Line firstLine = curveLoopCurves[precedentCurveIndex] as Line;
                Line lastLine = curveLoopCurves[nextCurveIndex] as Line;

                Line[] remakedLines = TrimTwoSequentialLines(firstLine, lastLine);

                curveLoopCurves[precedentCurveIndex] = remakedLines[0];
                curveLoopCurves[nextCurveIndex] = remakedLines[1];
            }

            smallCurvesIndexes.Reverse();

            foreach (int index in smallCurvesIndexes)
            {
                curveLoopCurves.RemoveAt(index);
            }

            return curveLoopCurves;
        }
        public static List<Curve> PurgeSequentialCurvesSmallLinesAndTrimAdjacent(List<Curve> curveLoopCurves, double tolerance)
        {
            int curveLoopLastIndex = curveLoopCurves.Count - 1;
            List<int> smallCurvesIndexes = GetSmallCurvesIndexes(curveLoopCurves, tolerance);

            foreach (int smallCurveIndex in smallCurvesIndexes)
            {
                int precedentCurveIndex = GetPrecedentCurveIndexInCurveLoop(smallCurveIndex, curveLoopLastIndex);
                int nextCurveIndex = GetNextCurveIndexInCurveLoop(smallCurveIndex, curveLoopLastIndex);

                Line firstLine = curveLoopCurves[precedentCurveIndex] as Line;
                Line lastLine = curveLoopCurves[nextCurveIndex] as Line;

                Line[] remakedLines = TrimTwoSequentialLines(firstLine, lastLine);

                curveLoopCurves[precedentCurveIndex] = remakedLines[0];
                curveLoopCurves[nextCurveIndex] = remakedLines[1];
            }

            smallCurvesIndexes.Reverse();

            foreach (int index in smallCurvesIndexes)
            {
                curveLoopCurves.RemoveAt(index);
            }

            return curveLoopCurves;
        }
        //public static List<CurveLoop> CorrectCurvesThatDontConnectInMultipleCurveLoopSequentialCurves(List<CurveLoop> curveLoops)
        //{
        //    List<CurveLoop> newCurveLoops = new List<CurveLoop>();

        //    foreach (CurveLoop curveLoop in curveLoops)
        //    {
        //        CurveLoop newCurveLoop = CorrectCurvesThatDontConnectInCurveLoop(curveLoop);
        //        newCurveLoops.Add(newCurveLoop);
        //    }

        //    return newCurveLoops;
        //}
        public static List<Curve> CorrectCurvesThatDontConnectInCurveLoopSequentialCurves(List<Curve> curveLoopCurves)
        {
            List<int[]> curvesThatDontConnectIndexes = GetIndexesOfCurvesThatDontConnectInListOfCurves(curveLoopCurves);

            foreach (int[] pairOfIndexes in curvesThatDontConnectIndexes)
            {
                Curve firstCurve = curveLoopCurves[pairOfIndexes[0]];
                Curve secondCurve = curveLoopCurves[pairOfIndexes[1]];
                string firstCurveTypeName = firstCurve.GetType().Name;
                string secondCurveTypeName = secondCurve.GetType().Name;

                if (firstCurveTypeName == "Line" && secondCurveTypeName == "Line")
                {
                    if (AreLinesAlmostParallel(firstCurve as Line, secondCurve as Line))
                    {
                        XYZ newPoint = secondCurve.GetEndPoint(0);
                        curveLoopCurves[pairOfIndexes[0]] = RemakeLineWithNewPoint(firstCurve as Line, newPoint, CurveEnd.End);
                    }

                    else
                    {
                        Line[] trimmedLines = TrimTwoSequentialLines(firstCurve as Line, secondCurve as Line);
                        curveLoopCurves[pairOfIndexes[0]] = trimmedLines[0];
                        curveLoopCurves[pairOfIndexes[1]] = trimmedLines[1];
                    }
                }

                else if (firstCurveTypeName == "Line" && secondCurveTypeName != "Line")
                {
                    XYZ newPoint = secondCurve.GetEndPoint(0);
                    curveLoopCurves[pairOfIndexes[0]] = RemakeLineWithNewPoint(firstCurve as Line, newPoint, CurveEnd.End);
                }

                else if (firstCurveTypeName != "Line" && secondCurveTypeName == "Line")
                {
                    XYZ newPoint = firstCurve.GetEndPoint(1);
                    curveLoopCurves[pairOfIndexes[1]] = RemakeLineWithNewPoint(secondCurve as Line, newPoint, CurveEnd.Start);
                }
            }

            return curveLoopCurves;
        }
    }
}
