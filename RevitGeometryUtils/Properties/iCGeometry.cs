// IMPORTS PADRÃO ====================
using System;
using System.Collections.Generic;
using System.Linq;

// IMPORTS AUTODESK ====================
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using AppServ = Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.ApplicationServices;

namespace VZTagMaterial.Auxiliar
{
    public static class DocInfo
    {
        // CAMPOS ====================
        public static UIApplication UIApp { get; private set; }
        public static UIDocument UIDoc { get; private set; }
        public static Document Doc { get; private set; }
        public static View ActiveView { get; private set; }

        // CONSTRUTOR ====================
        static DocInfo() { }

        // MÉTODOS ====================
        public static void Init(ExternalCommandData commandData)
        {
            UIApp = commandData.Application;
            UIDoc = UIApp.ActiveUIDocument;
            Doc = UIDoc.Document;
            ActiveView = Doc.ActiveView;
        }
    }
    public class iCGeometry
    {
        public static Application Application = DocInfo.Doc.Application;
        public static double ShortCurveTolerance = Application.ShortCurveTolerance;
        public static double AngleTolerance = Application.AngleTolerance;
        public static double VertexTolerance = Application.VertexTolerance;
        public const int CoordinatesDecimalsToRound = 1;
        
        
        //METHODS




        //Range
        public static double GetIntersectionBetweenTwoRanges(double[] firstRange, double[] secondRange)
        {
            double bottomRange = Math.Max(firstRange[0], secondRange[0]);
            double topRange = Math.Min(firstRange[1], secondRange[1]);
            double intersectionBetweenRanges = topRange - bottomRange;

            return intersectionBetweenRanges;
        }


        //Point2D
        public static bool IsPointInsideSolid(Solid solid, XYZ point)
        {
            SolidCurveIntersectionOptions solidCurveIntersectionOptions = new SolidCurveIntersectionOptions();
            solidCurveIntersectionOptions.ResultType = SolidCurveIntersectionMode.CurveSegmentsInside;

            Line line = Line.CreateBound(point, point.Add(XYZ.BasisX));

            double tolerance = 0.000001;

            SolidCurveIntersection solidCurveIntersection = solid.IntersectWithCurve(line, solidCurveIntersectionOptions);

            for (int i = 0; i < solidCurveIntersection.SegmentCount; i++)
            {
                Curve curve = solidCurveIntersection.GetCurveSegment(i);

                if (point.IsAlmostEqualTo(curve.GetEndPoint(0), tolerance) || point.IsAlmostEqualTo(curve.GetEndPoint(1), tolerance))
                {
                    return true;
                }
            }

            return false;
        }
        public static List<XYZ> ProjectListOfPointsInZPlane(List<XYZ> points)
        {
            List<XYZ> projectedPoints = points
                .Select(x => new XYZ(x.X, x.Y, 0))
                .ToList();

            return projectedPoints;
        }


        //Point3D
        public static List<XYZ> RoundMultiplePointCoordinates(List<XYZ> points, int digitsToRoundCoordinates)
        {
            List<XYZ> roundedPoints = points
                .Select(x => RoundPointCoordinates(x, digitsToRoundCoordinates))
                .ToList();

            return roundedPoints;
        }
        public static XYZ RoundPointCoordinates(XYZ point, int digitsToRoundCoordinates)
        {
            double roundedX = Math.Round(point.X, digitsToRoundCoordinates);
            double roundedY = Math.Round(point.Y, digitsToRoundCoordinates);
            double roundedZ = Math.Round(point.Z, digitsToRoundCoordinates);

            return new XYZ(roundedX, roundedY, roundedZ);
        }
        public static bool ArePointsNumericallyEqual(XYZ firstPoint, XYZ secondPoint)
        {
            if (firstPoint.DistanceTo(secondPoint) <= VertexTolerance)
            {
                return true;
            }

            return false;
        }
        public static bool ArePointsNumericallyEqual(XYZ firstPoint, XYZ secondPoint, double tolerance)
        {
            if (firstPoint.DistanceTo(secondPoint) <= tolerance)
            {
                return true;
            }

            return false;
        }
        public static List<XYZ> RemovePointsAtIndexes(List<XYZ> unfilteredPoints, List<int> indexesToRemove)
        {
            List<int> filteredIndexesToRemove = indexesToRemove
                .Distinct()
                .OrderByDescending(x => x)
                .ToList();

            foreach (int index in filteredIndexesToRemove)
            {
                unfilteredPoints.RemoveAt(index);
            }

            return unfilteredPoints;
        }
        public static List<XYZ> PruneNonSequentialDuplicatedPoints(List<XYZ> nonSequentialPoints)
        {
            List<int> indexesToRemove = new List<int>();

            for (int i = 0; i < nonSequentialPoints.Count(); i++)
            {
                for (int j = i + 1; j < nonSequentialPoints.Count(); j++)
                {
                    if (ArePointsNumericallyEqual(nonSequentialPoints[i], nonSequentialPoints[j]))
                    {
                        indexesToRemove.Add(j);
                    }
                }
            }

            List<XYZ> filteredNonSequentialPoints = RemovePointsAtIndexes(nonSequentialPoints, indexesToRemove);

            return filteredNonSequentialPoints;
        }
        public static List<XYZ> PruneNonSequentialDuplicatedPoints(List<XYZ> nonSequentialPoints, double tolerance)
        {
            List<int> indexesToRemove = new List<int>();

            for (int i = 0; i < nonSequentialPoints.Count(); i++)
            {
                for (int j = i + 1; j < nonSequentialPoints.Count(); j++)
                {
                    if (ArePointsNumericallyEqual(nonSequentialPoints[i], nonSequentialPoints[j], tolerance))
                    {
                        indexesToRemove.Add(j);
                    }
                }
            }

            List<XYZ> filteredNonSequentialPoints = RemovePointsAtIndexes(nonSequentialPoints, indexesToRemove);

            return filteredNonSequentialPoints;
        }
        public static List<XYZ> PruneSequentialDuplicatedPoints(List<XYZ> sequentialPoints)
        {
            List<int> indexesToRemove = new List<int>();

            for (int i = 0; i < (sequentialPoints.Count - 1); i++)
            {
                if (ArePointsNumericallyEqual(sequentialPoints[i], sequentialPoints[i + 1]))
                {
                    indexesToRemove.Add(i);
                }
            }

            List<XYZ> filteredSequentialPoints = RemovePointsAtIndexes(sequentialPoints, indexesToRemove);

            return filteredSequentialPoints;
        }
        public static List<XYZ> PruneSequentialDuplicatedPoints(List<XYZ> sequentialPoints, double tolerance)
        {
            List<int> indexesToRemove = new List<int>();

            for (int i = 0; i < (sequentialPoints.Count - 1); i++)
            {
                if (ArePointsNumericallyEqual(sequentialPoints[i], sequentialPoints[i + 1], tolerance))
                {
                    indexesToRemove.Add(i);
                }
            }

            List<XYZ> filteredSequentialPoints = RemovePointsAtIndexes(sequentialPoints, indexesToRemove);

            return filteredSequentialPoints;
        }
        public static XYZ ProjectPointOnGlobalPlane(XYZ point, GlobalPlane globalPlane)
        {
            switch (globalPlane)
            {
                case GlobalPlane.XYPlane:
                    return new XYZ(point.X, point.Y, 0);

                case GlobalPlane.XZPlane:
                    return new XYZ(0, point.Y, point.Z);

                case GlobalPlane.YZPlane:
                    return new XYZ(point.X, 0, point.Z);
            }

            return null;
        }
        public static XYZ ProjectPointOnGlobalPlane(XYZ point, GlobalPlane globalPlane, int digitsToRoundCoordinates)
        {
            switch (globalPlane)
            {
                case GlobalPlane.XYPlane:
                    return new XYZ(Math.Round(point.X, digitsToRoundCoordinates), Math.Round(point.Y, digitsToRoundCoordinates), 0);

                case GlobalPlane.XZPlane:
                    return new XYZ(0, Math.Round(point.Y, digitsToRoundCoordinates), Math.Round(point.Z, digitsToRoundCoordinates));

                case GlobalPlane.YZPlane:
                    return new XYZ(Math.Round(point.X, digitsToRoundCoordinates), 0, Math.Round(point.Z, digitsToRoundCoordinates));
            }

            return null;
        }
        public static XYZ ProjectPointOnFacePlane(XYZ pointToProject, PlanarFace planarFace)
        {
            XYZ normal = planarFace.FaceNormal;
            XYZ origin = planarFace.Evaluate(new UV(0.5, 0.5));
            XYZ projectedPoint = ProjectPointByOriginAndNormal(pointToProject, origin, normal);

            return projectedPoint;
        }
        public static XYZ ProjectPointOnFacePlane(XYZ pointToProject, PlanarFace planarFace, int digitsToRoundCoordinates)
        {
            XYZ normal = planarFace.FaceNormal;
            XYZ origin = planarFace.Evaluate(new UV(0.5, 0.5));
            XYZ roundedProjectedPoint = ProjectPointByOriginAndNormal(pointToProject, origin, normal, digitsToRoundCoordinates);

            return roundedProjectedPoint;
        }
        public static XYZ ProjectPointByOriginAndNormal(XYZ pointToProject, XYZ origin, XYZ normal)
        {
            XYZ vectorBetweenOriginAndPointToProject = pointToProject - origin;
            double distanceToTranslate = vectorBetweenOriginAndPointToProject.DotProduct(normal);
            XYZ projectedPoint = pointToProject - (distanceToTranslate * normal);

            return projectedPoint;
        }
        public static XYZ ProjectPointByOriginAndNormal(XYZ pointToProject, XYZ origin, XYZ normal, int digitsToRoundCoordinates)
        {
            XYZ vectorBetweenOriginAndPointToProject = pointToProject - origin;
            double distanceToTranslate = vectorBetweenOriginAndPointToProject.DotProduct(normal);
            XYZ projectedPoint = pointToProject - (distanceToTranslate * normal);
            XYZ roundedProjectedPoint = RoundPointCoordinates(projectedPoint, digitsToRoundCoordinates);

            return roundedProjectedPoint;
        }


        //Vector
        public static bool AreVectorsAlmostParallel(XYZ firstVector, XYZ secondVector, double tolerance = 0.001)
        {
            if (firstVector.IsAlmostEqualTo(secondVector, tolerance) || firstVector.IsAlmostEqualTo(secondVector.Negate(), tolerance))
            {
                return true;
            }

            return false;
        }


        //Curve
        public enum CurveEnd
        {
            Start,
            End
        }
        public static bool IsCurveBelowLengthTolerance(Curve curve)
        {
            if (curve.Length <= ShortCurveTolerance)
            {
                return true;
            }

            return false;
        }
        public static bool IsCurveBelowLengthTolerance(Curve curve, double tolerance)
        {
            if (curve.Length <= 0.1)
            {
                return true;
            }

            return false;
        }
        public static Curve TranslateCurveByVector(Curve curve, XYZ vector)
        {
            Transform transform = Transform.CreateTranslation(vector);
            Curve translatedCurve = curve.CreateTransformed(transform);

            return translatedCurve;
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
        public static Curve ProjectCurveOnGlobalPlane(Curve curve, GlobalPlane globalPlane)
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



        //CurveLoop
        public static List<CurveLoop> PurgeMultipleCurveLoopsSmallLinesAndTrimAdjacentLines(List<CurveLoop> curveLoops)
        {
            List<CurveLoop> filteredCurveLoops = new List<CurveLoop>();

            foreach (CurveLoop curveLoop in curveLoops)
            {
                CurveLoop filteredCurveLoop = PurgeCurveLoopSmallLinesAndTrimAdjacent(curveLoop);
                filteredCurveLoops.Add(filteredCurveLoop);
            }

            return filteredCurveLoops;
        }
        public static List<CurveLoop> PurgeMultipleCurveLoopsSmallLinesAndTrimAdjacentLines(List<CurveLoop> curveLoops, double tolerance)
        {
            List<CurveLoop> filteredCurveLoops = new List<CurveLoop>();

            foreach (CurveLoop curveLoop in curveLoops)
            {
                CurveLoop filteredCurveLoop = PurgeCurveLoopSmallLinesAndTrimAdjacent(curveLoop, tolerance);
                filteredCurveLoops.Add(filteredCurveLoop);
            }

            return filteredCurveLoops;
        }
        public static CurveLoop PurgeCurveLoopSmallLinesAndTrimAdjacent(CurveLoop curveLoop)
        {
            List<Curve> curveLoopCurves = GetSequentialCurvesFromCurveLoop(curveLoop);
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

            CurveLoop newCurveLoop = CurveLoop.Create(curveLoopCurves);

            return newCurveLoop;
        }
        public static CurveLoop PurgeCurveLoopSmallLinesAndTrimAdjacent(CurveLoop curveLoop, double tolerance)
        {
            List<Curve> curveLoopCurves = GetSequentialCurvesFromCurveLoop(curveLoop);
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

            CurveLoop newCurveLoop = CurveLoop.Create(curveLoopCurves);

            return newCurveLoop;
        }
        private static int GetPrecedentCurveIndexInCurveLoop(int actualIndex, int curveLoopLastIndex)
        {
            if (actualIndex == 0)
            {
                return curveLoopLastIndex;
            }

            return actualIndex - 1;
        }
        private static int GetNextCurveIndexInCurveLoop(int actualIndex, int curveLoopLastIndex)
        {
            if (actualIndex == curveLoopLastIndex)
            {
                return 0;
            }

            return actualIndex + 1;
        }
        private static List<int> GetSmallCurvesIndexes(List<Curve> curveLoopCurves)
        {
            List<int> smallCurvesIndexes = new List<int>();

            for (int i = 0; i < curveLoopCurves.Count; i++)
            {
                if (IsCurveBelowLengthTolerance(curveLoopCurves[i]))
                {
                    smallCurvesIndexes.Add(i);
                }
            }

            return smallCurvesIndexes;
        }
        private static List<int> GetSmallCurvesIndexes(List<Curve> curveLoopCurves, double tolerance)
        {
            List<int> smallCurvesIndexes = new List<int>();

            for (int i = 0; i < curveLoopCurves.Count; i++)
            {
                if (IsCurveBelowLengthTolerance(curveLoopCurves[i], tolerance))
                {
                    smallCurvesIndexes.Add(i);
                }
            }

            return smallCurvesIndexes;
        }
        private static Line[] TrimTwoSequentialLines(Line firstLine, Line lastLine)
        {
            double valueToExtend = 10000000000;
            Line extendedFirstLine = ExtendLineByEndAndValue(firstLine, valueToExtend, CurveEnd.End);
            Line extendedSecondLine = ExtendLineByEndAndValue(lastLine, valueToExtend, CurveEnd.Start);
            IntersectionResultArray intersectionResultArray = new IntersectionResultArray();
            SetComparisonResult intersectionSet = extendedFirstLine.Intersect(extendedSecondLine, out intersectionResultArray);
            List<XYZ> intersectionBetweenTwolines = new List<XYZ>();

            XYZ pointOfInterest;

            if (intersectionResultArray ==  null && extendedFirstLine.Origin.Z == extendedSecondLine.Origin.Z )
            {
                pointOfInterest = GetPointIntersectionBetweenTwoLinesOnZPlane(extendedFirstLine, extendedSecondLine, extendedSecondLine.Origin.Z);
            }
            else
            {
                foreach (IntersectionResult intersectionResult in intersectionResultArray)
                {
                    XYZ intersectionPoint = intersectionResult.XYZPoint;
                    intersectionBetweenTwolines.Add(intersectionPoint);
                }

                XYZ firstCurvePoint = firstLine.GetEndPoint(1);
                pointOfInterest = intersectionBetweenTwolines
                    .OrderBy(x => x.DistanceTo(firstCurvePoint))
                    .First();
            }

            Line newFirstLine = RemakeLineWithNewPointMantainingDirection(firstLine as Line, pointOfInterest, CurveEnd.End);
            Line newLastLine = RemakeLineWithNewPointMantainingDirection(lastLine as Line, pointOfInterest, CurveEnd.Start);

            Line[] newLines = new Line[2] { newFirstLine, newLastLine };

            return newLines;
        }
        public static List<XYZ> GetSequentialVerticesFromLinesCurveLoop(CurveLoop curveLoop)
        {
            List<Curve> sequentialCurves = GetSequentialCurvesFromCurveLoop(curveLoop);
            List<XYZ> sequentialVertices = GetSequentialVerticesFromSequentialLines(sequentialCurves);

            return sequentialVertices;
        }
        public static List<Curve> GetSequentialCurvesFromCurveLoop(CurveLoop curveLoop)
        {
            List<Curve> curves = new List<Curve>();
            CurveLoopIterator curveLoopIterator = curveLoop.GetCurveLoopIterator();
            curveLoopIterator.Reset();

            while (curveLoopIterator.MoveNext())
            {
                Curve curve = curveLoopIterator.Current;

                if (curve != null)
                {
                    curves.Add(curve);
                }
            }

            return curves;
        }

        public static List<CurveLoop> JoinContinuousLinesOnMultipleCurveLoops(List<CurveLoop> curveLoops)
        {
            List<CurveLoop> newCurveLoops = new List<CurveLoop>();

            foreach (CurveLoop curveLoop in curveLoops)
            {
                CurveLoop newCurveLoop = JoinContinuousLinesOnCurveLoop(curveLoop);
                newCurveLoops.Add(newCurveLoop);
            }

            return newCurveLoops;
        }
        private static CurveLoop JoinContinuousLinesOnCurveLoop(CurveLoop curveLoop)
        {
            List<Curve> curveLoopCurves = GetSequentialCurvesFromCurveLoop(curveLoop);

            if (AreThereJoinableLinesInCurveLoop(curveLoopCurves))
            {
                List<Curve> newCurves = JoinContinuousLinesOnCurveList(curveLoopCurves);

                curveLoop = new CurveLoop();

                foreach (Curve curve in newCurves)
                {
                    curveLoop.Append(curve);
                }
            }

            return curveLoop;
        }
        private static List<Curve> JoinContinuousLinesOnCurveList(List<Curve> curveLoopCurves)
        {
            List<List<int>> curveIndexesToJoin = GetCurveindexesToJoinOrder(curveLoopCurves);
            List<List<int>> filteredCurveIndexesToJoin = ModifyListIfThereIsContinuityAtBeggining(curveIndexesToJoin);
            List<Curve> newCurveLoopCurves = GetNewCurvesForCurveLoop(filteredCurveIndexesToJoin, curveLoopCurves);

            return newCurveLoopCurves;
        }
        private static bool AreThereJoinableLinesInCurveLoop(List<Curve> curveLoopCurves)
        {
            int curveLoopLastIndex = curveLoopCurves.Count - 1;

            for (int i = 0; i <= curveLoopLastIndex; i++)
            {
                string curveTypeName = curveLoopCurves[i].GetType().Name;

                if (curveTypeName == "Line")
                {
                    int nextCurveIndex = GetNextCurveIndexInCurveLoop(i, curveLoopLastIndex);
                    string nextCurveTypeName = curveLoopCurves[nextCurveIndex].GetType().Name;

                    if (nextCurveTypeName == "Line")
                    {
                        Line actualLine = curveLoopCurves[i] as Line;
                        XYZ actualLineDirection = actualLine.Direction;

                        Line nextLine = curveLoopCurves[nextCurveIndex] as Line;
                        XYZ nextLineDirection = nextLine.Direction;

                        if (actualLineDirection.IsAlmostEqualTo(nextLineDirection))
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }
        private static List<List<int>> GetCurveindexesToJoinOrder(List<Curve> curveLoopCurves)
        {
            bool isFirstLineOfContinuity = true;
            List<List<int>> curveIndexesToJoin = new List<List<int>>();
            List<int> sequentialCurveIndicesWithGroupedColinear = new List<int>();
            XYZ lineDirectionReference = null;

            for (int i = 0; i < curveLoopCurves.Count(); i++)
            {
                if (!IsActualCurveLine(curveLoopCurves[i]))
                {
                    curveIndexesToJoin.Add(new List<int>() { i });
                    isFirstLineOfContinuity = true;
                }

                else
                {
                    if (isFirstLineOfContinuity == true)
                    {
                        sequentialCurveIndicesWithGroupedColinear = new List<int>() { i };
                        isFirstLineOfContinuity = false;
                        Line firstLineOfContinuity = curveLoopCurves[i] as Line;
                        lineDirectionReference = firstLineOfContinuity.Direction;

                        if (i == curveLoopCurves.Count() - 1)
                        {
                            curveIndexesToJoin.Add(sequentialCurveIndicesWithGroupedColinear);
                        }
                    }

                    else
                    {
                        if (IsActualCurveColinearAndContinuousToLast(curveLoopCurves[i], lineDirectionReference))
                        {
                            sequentialCurveIndicesWithGroupedColinear.Add(i);
                        }

                        else
                        {
                            curveIndexesToJoin.Add(sequentialCurveIndicesWithGroupedColinear);
                            isFirstLineOfContinuity = true;
                        }
                    }
                }
            }

            return curveIndexesToJoin;
        }
        private static bool IsActualCurveLine(Curve curve)
        {
            if (curve.GetType().Name != "Line")
            {
                return true;
            }

            return false;
        }
        private static bool IsActualCurveColinearAndContinuousToLast(Curve curve, XYZ referenceDirection)
        {
            if (curve.GetType().Name != "Line" && (curve as Line).Direction.Normalize().IsAlmostEqualTo(referenceDirection))
            {
                return true;
            }

            return false;
        }
        private static List<List<int>> ModifyListIfThereIsContinuityAtBeggining(List<List<int>> curveIndexesToJoin)
        {
            if (curveIndexesToJoin.Count() > 1)
            {
                if (curveIndexesToJoin.First().First() == curveIndexesToJoin.Last().Last())
                {
                    List<int> lastItem = curveIndexesToJoin.Last();
                    lastItem.RemoveAt(lastItem.Count() - 1);
                    List<int> newList = lastItem.Concat(curveIndexesToJoin.First()).ToList();
                    curveIndexesToJoin[lastItem.Count() - 1] = newList;
                    curveIndexesToJoin.RemoveAt(0);
                }
            }

            return curveIndexesToJoin;
        }
        private static List<Curve> GetNewCurvesForCurveLoop(List<List<int>> curveIndexesToJoin, List<Curve> originalCurves)
        {
            List<Curve> newListOfCurves = new List<Curve>();

            foreach (List<int> listOfIndexes in curveIndexesToJoin)
            {
                if (listOfIndexes.Count() > 1)
                {
                    int firstCurveIndex = listOfIndexes.First();
                    int secondCurveIndex = listOfIndexes.Last();

                    Line firstLine = originalCurves[firstCurveIndex] as Line;
                    Line lastLine = originalCurves[secondCurveIndex] as Line;

                    XYZ firstLineStartPoint = firstLine.GetEndPoint(0);
                    XYZ lastLineEndPoint = lastLine.GetEndPoint(1);

                    Line newLine = Line.CreateBound(firstLineStartPoint, lastLineEndPoint);
                    newListOfCurves.Add(newLine);
                }
                else
                {
                    newListOfCurves.Add(originalCurves[listOfIndexes[0]]);
                }
            }

            return newListOfCurves;
        }
        private static List<int[]> GetIndexesOfCurvesThatDontConnectInListOfCurves(List<Curve> curveLoopCurves)
        {
            int curveLoopLastIndex = curveLoopCurves.Count - 1;
            List<int[]> curvesIndexesThatDontConnectInCurveLoop = new List<int[]>();

            for (int i = 0; i < curveLoopCurves.Count(); i++)
            {
                int nextCurveIndex = GetNextCurveIndexInCurveLoop(i, curveLoopLastIndex);
                Curve actualCurve = curveLoopCurves[i];
                Curve nextCurve = curveLoopCurves[nextCurveIndex];

                if (!IsActualCurveEndPointEqualToNextCurveStartPoint(actualCurve, nextCurve))
                {
                    int[] pairOfCurves = new int[2] { i, nextCurveIndex };
                    curvesIndexesThatDontConnectInCurveLoop.Add(pairOfCurves);
                }
            }

            return curvesIndexesThatDontConnectInCurveLoop;
        }
        private static bool IsActualCurveEndPointEqualToNextCurveStartPoint(Curve actualCurve, Curve nextCurve)
        {
            XYZ actualCurveEndPoint = actualCurve.GetEndPoint(1);
            XYZ nextCurveStartPoint = nextCurve.GetEndPoint(0);

            if (ArePointsNumericallyEqual(actualCurveEndPoint, nextCurveStartPoint))
            {
                return true;
            }

            return false;
        }
        public static List<CurveLoop> CorrectCurvesThatDontConnectInMultipleCurveLoops(List<CurveLoop> curveLoops)
        {
            List<CurveLoop> newCurveLoops = new List<CurveLoop>();

            foreach (CurveLoop curveLoop in curveLoops)
            {
                CurveLoop newCurveLoop = CorrectCurvesThatDontConnectInCurveLoop(curveLoop);
                newCurveLoops.Add(newCurveLoop);
            }

            return newCurveLoops;
        }
        public static CurveLoop CorrectCurvesThatDontConnectInCurveLoop(CurveLoop curveLoop)
        {
            List<Curve> curveLoopCurves = GetSequentialCurvesFromCurveLoop(curveLoop);
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

            CurveLoop newCurveLoop = new CurveLoop();

            foreach (Curve curveLoopCurve in curveLoopCurves)
            {
                newCurveLoop.Append(curveLoopCurve);
            }

            return newCurveLoop;
        }






        //Line
        public static Line ProjectLineOnGlobalPlane(Line line, GlobalPlane globalPlane)
        {
            XYZ originalStartPoint = line.GetEndPoint(0);
            XYZ originalEndPoint = line.GetEndPoint(1);
            XYZ newStartPoint = ProjectPointOnGlobalPlane(originalStartPoint, globalPlane);
            XYZ newEndPoint = ProjectPointOnGlobalPlane(originalEndPoint, globalPlane);
            Line newLine = Line.CreateBound(newStartPoint, newEndPoint);

            return newLine;
        }
        public static Line ProjectLineOnGlobalPlane(Line line, GlobalPlane globalPlane, int digitsToRoundCoordinates)
        {
            XYZ originalStartPoint = line.GetEndPoint(0);
            XYZ originalEndPoint = line.GetEndPoint(1);
            XYZ newStartPoint = ProjectPointOnGlobalPlane(originalStartPoint, globalPlane, digitsToRoundCoordinates);
            XYZ newEndPoint = ProjectPointOnGlobalPlane(originalEndPoint, globalPlane, digitsToRoundCoordinates);
            Line newLine = Line.CreateBound(newStartPoint, newEndPoint);

            return newLine;
        }
        public static Line ProjectLineOnPlaneByPlanarFace(Line line, PlanarFace planarFace)
        {
            XYZ originalStartPoint = line.GetEndPoint(0);
            XYZ originalEndPoint = line.GetEndPoint(1);
            XYZ newStartPoint = ProjectPointOnFacePlane(originalStartPoint, planarFace);
            XYZ newEndPoint = ProjectPointOnFacePlane(originalEndPoint, planarFace);
            Line newLine = Line.CreateBound(newStartPoint, newEndPoint);

            return newLine;
        }
        public static Line ProjectLineOnPlaneByPlanarFace(Line line, PlanarFace planarFace, int digitsToRoundCoordinates)
        {
            XYZ originalStartPoint = line.GetEndPoint(0);
            XYZ originalEndPoint = line.GetEndPoint(1);
            XYZ newStartPoint = ProjectPointOnFacePlane(originalStartPoint, planarFace, digitsToRoundCoordinates);
            XYZ newEndPoint = ProjectPointOnFacePlane(originalEndPoint, planarFace, digitsToRoundCoordinates);
            Line newLine = Line.CreateBound(newStartPoint, newEndPoint);

            return newLine;
        }
        public static Line ProjectLineOnPlaneByOriginAndNormal(Line line, XYZ origin, XYZ normal)
        {
            XYZ originalStartPoint = line.GetEndPoint(0);
            XYZ originalEndPoint = line.GetEndPoint(1);
            XYZ newStartPoint = ProjectPointByOriginAndNormal(originalStartPoint, origin, normal);
            XYZ newEndPoint = ProjectPointByOriginAndNormal(originalEndPoint, origin, normal);
            Line newLine = Line.CreateBound(newStartPoint, newEndPoint);

            return newLine;
        }
        public static Line ProjectLineOnPlaneByOriginAndNormal(Line line, XYZ origin, XYZ normal, int digitsToRoundCoordinates)
        {
            XYZ originalStartPoint = line.GetEndPoint(0);
            XYZ originalEndPoint = line.GetEndPoint(1);
            XYZ newStartPoint = ProjectPointByOriginAndNormal(originalStartPoint, origin, normal, digitsToRoundCoordinates);
            XYZ newEndPoint = ProjectPointByOriginAndNormal(originalEndPoint, origin, normal, digitsToRoundCoordinates);
            Line newLine = Line.CreateBound(newStartPoint, newEndPoint);

            return newLine;
        }
        public static XYZ GetPointIntersectionBetweenTwoLinesOnZPlane(Line firstLine, Line secondLine, double zValue)
        {
            XYZ[] firstLinePoints = new XYZ[2] { firstLine.GetEndPoint(0), firstLine.GetEndPoint(1) };
            XYZ[] secondLinePoints = new XYZ[2] { secondLine.GetEndPoint(0), secondLine.GetEndPoint(1) };

            // Line AB represented as a1x + b1y = c1
            double a1 = firstLinePoints[1].Y - firstLinePoints[0].Y;
            double b1 = firstLinePoints[0].X - firstLinePoints[1].X;
            double c1 = a1 * firstLinePoints[0].X + b1 * firstLinePoints[0].Y;

            // Line CD represented as a2x + b2y = c2
            double a2 = secondLinePoints[1].Y - secondLinePoints[0].Y;
            double b2 = secondLinePoints[0].X - secondLinePoints[1].X;
            double c2 = a2 * secondLinePoints[0].X + b2 * secondLinePoints[0].Y;

            double determinant = a1 * b2 - a2 * b1;

            if (determinant == 0)
            {
                return null;
            }
            else
            {
                double x = (b2 * c1 - b1 * c2) / determinant;
                double y = (a1 * c2 - a2 * c1) / determinant;
                return new XYZ(x, y, zValue);
            }
        }
        public static bool AreLinesAlmostParallel(Line firstLine, Line secondLine)
        {
            return AreVectorsAlmostParallel(firstLine.Direction, secondLine.Direction);
        }
        public static List<XYZ> GetSequentialVerticesFromSequentialLines(List<Curve> sequentialCurves)
        {
            List<XYZ> vertices = sequentialCurves
                .Select(x => x.GetEndPoint(0))
                .ToList();

            return vertices;
        }
        public static List<XYZ> GetSequentialVerticesFromSequentialLines(List<Curve> sequentialCurves, int decimalsToRound)
        {
            List<XYZ> vertices = sequentialCurves
                .Select(x => RoundPointCoordinates(x.GetEndPoint(0), decimalsToRound))
                .ToList();

            return vertices;
        }
        public static CurveLoop CreateCurveLoopFromSequentialLineVertices(List<XYZ> sequentialVertices, bool alignSlightlyOffAxisLines)
        {
            CurveLoop curveLoop = new CurveLoop();

            for (int i = 0; i < (sequentialVertices.Count - 1); i++)
            {
                Line line = Line.CreateBound(sequentialVertices[i], sequentialVertices[i + 1]);

                if (alignSlightlyOffAxisLines)
                {
                    double[] anglesToGlobalAxes = GetLineAnglesToGlobalAxes(line);

                    if (IsLineSlightlyOffAxis(anglesToGlobalAxes))
                    {
                        line = RemakeLineSlightlyOffAxis(line, CurveEnd.Start);
                    }
                }

                curveLoop.Append(line);
            }

            return curveLoop;
        }
        public static CurveLoop CreateCurveLoopFromSequentialLineVertices(List<XYZ> sequentialVertices, int decimalsToRound, bool alignSlightlyOffAxisLines)
        {
            CurveLoop curveLoop = new CurveLoop();
            List<XYZ> roundedSequentialVertices = RoundMultiplePointCoordinates(sequentialVertices, decimalsToRound);

            for (int i = 0; i < (roundedSequentialVertices.Count - 1); i++)
            {
                Line line = Line.CreateBound(roundedSequentialVertices[i], roundedSequentialVertices[i + 1]);

                if (alignSlightlyOffAxisLines)
                {
                    double[] anglesToGlobalAxes = GetLineAnglesToGlobalAxes(line);

                    if (IsLineSlightlyOffAxis(anglesToGlobalAxes))
                    {
                        line = RemakeLineSlightlyOffAxis(line, CurveEnd.Start);
                    }
                }

                curveLoop.Append(line);
            }

            return curveLoop;
        }
        public static List<Line> CreateSequentialLinesFromSequentialVertices(List<XYZ> sequentialVertices, bool alignSlightlyOffAxisLines)
        {
            List<Line> sequentialLines = new List<Line>();

            for (int i = 0; i < (sequentialVertices.Count - 1); i++)
            {
                Line line = Line.CreateBound(sequentialVertices[i], sequentialVertices[i + 1]);

                if (alignSlightlyOffAxisLines)
                {
                    double[] anglesToGlobalAxes = GetLineAnglesToGlobalAxes(line);

                    if (IsLineSlightlyOffAxis(anglesToGlobalAxes))
                    {
                        line = RemakeLineSlightlyOffAxis(line, CurveEnd.Start);
                    }
                }

                sequentialLines.Append(line);
            }

            return sequentialLines;
        }
        public static List<Line> CreateSequentialLinesFromSequentialVertices(List<XYZ> sequentialVertices, int decimalsToRound, bool alignSlightlyOffAxisLines)
        {
            List<Line> sequentialLines = new List<Line>();
            List<XYZ> roundedSequentialVertices = RoundMultiplePointCoordinates(sequentialVertices, decimalsToRound);

            for (int i = 0; i < (roundedSequentialVertices.Count - 1); i++)
            {
                Line line = Line.CreateBound(roundedSequentialVertices[i], roundedSequentialVertices[i + 1]);

                if (alignSlightlyOffAxisLines)
                {
                    double[] anglesToGlobalAxes = GetLineAnglesToGlobalAxes(line);

                    if (IsLineSlightlyOffAxis(anglesToGlobalAxes))
                    {
                        line = RemakeLineSlightlyOffAxis(line, CurveEnd.Start);
                    }
                }

                sequentialLines.Add(line);
            }

            return sequentialLines;
        }
        public static Line ExtendLineByEndAndValue(Line line, double value, CurveEnd curveEnd)
        {
            int endToExtend = 1;
            int endToMantain = 0;
            XYZ directionToExtend = line.Direction;

            if (curveEnd == CurveEnd.Start)
            {
                endToExtend = 0;
                endToMantain = 1;
                directionToExtend = directionToExtend.Negate();
            }

            XYZ pointToMaintain = line.GetEndPoint(endToMantain);
            XYZ pointToChange = line.GetEndPoint(endToExtend) + (directionToExtend * value);
            Line extendedLine;

            if (curveEnd == CurveEnd.Start)
            {
                extendedLine = Line.CreateBound(pointToChange, pointToMaintain);
            }
            else
            {
                extendedLine = Line.CreateBound(pointToMaintain, pointToChange);
            }

            return extendedLine;
        }
        public static Line ExtendLineByVector(Line line, XYZ vector)
        {
            int endToExtend = 1;
            int endToMantain = 0;

            if (AreVectorsAlmostParallel(vector.Normalize(), line.Direction.Normalize()))
            {
                return null;
            }

            bool doesVectorsPointToSameDirection = vector.Normalize().IsAlmostEqualTo(line.Direction.Normalize());

            if (!doesVectorsPointToSameDirection)
            {
                endToExtend = 0;
                endToMantain = 1;
            }

            XYZ pointToMaintain = line.GetEndPoint(endToMantain);
            XYZ pointToChange = line.GetEndPoint(endToExtend) + vector;
            Line extendedLine;

            if (!doesVectorsPointToSameDirection)
            {
                extendedLine = Line.CreateBound(pointToChange, pointToMaintain);
            }
            else
            {
                extendedLine = Line.CreateBound(pointToMaintain, pointToChange);
            }

            return extendedLine;
        }
        public static Line ExtendLineByPointAndValue(Line line, double value, XYZ pointEndToExtend)
        {
            if (pointEndToExtend.IsAlmostEqualTo(line.GetEndPoint(0)))
            {
                return ExtendLineByEndAndValue(line, value, CurveEnd.Start);
            }
            else if (pointEndToExtend.IsAlmostEqualTo(line.GetEndPoint(1)))
            {
                return ExtendLineByEndAndValue(line, value, CurveEnd.End);
            }
            else
            {
                //Retornar uma exception?
                return null;
            }
        }
        
        public static Line RemakeLineWithNewPoint(Line line, XYZ newPoint, CurveEnd endToChange)
        {
            XYZ newStart = line.GetEndPoint(0);
            XYZ newEnd = newPoint;

            if (endToChange == CurveEnd.Start)
            {
                newStart = newEnd;
                newEnd = line.GetEndPoint(1);
            }

            Line newLine = Line.CreateBound(newStart, newEnd);

            return newLine;
        }
        public static Line RemakeLineWithNewPointMantainingDirection(Line line, XYZ newPoint, CurveEnd endToChange)
        {
            Line newLine = RemakeLineWithNewPoint(line, newPoint, endToChange);

            if (endToChange == CurveEnd.Start)
            {
                newLine = newLine.CreateReversed() as Line;
            }

            //if (!newLine.Direction.Normalize().IsAlmostEqualTo(line.Direction.Normalize()))
            //{
            //    newLine = newLine.CreateReversed() as Line;
            //}

            return newLine;
        }
        public static double[] GetLineAnglesToGlobalAxes(Line line)
        {
            XYZ lineVector = line.Direction;

            double angleToGlobalXAxis = lineVector.AngleTo(XYZ.BasisX);
            double angleToGlobalYAxis = lineVector.AngleTo(XYZ.BasisY);
            double angleToGlobalZAxis = lineVector.AngleTo(XYZ.BasisZ);
            double[] anglesToGlobalAxes = new double[3] { angleToGlobalXAxis, angleToGlobalYAxis, angleToGlobalZAxis };

            return anglesToGlobalAxes;
        }
        public static bool IsLineSlightlyOffAxis(double[] lineAnglesToGlobalAxes)
        {
            return lineAnglesToGlobalAxes.Any(x => x <= AngleTolerance * 2 && x > VertexTolerance);
        }
        public static Line RemakeLineSlightlyOffAxis(Line line, CurveEnd endToChange)
        {
            int endToMaintain = 1;

            if ((int)endToChange == 1)
            {
                endToMaintain = 0;
            }

            XYZ endToMaintainPoint = line.GetEndPoint(endToMaintain);
            XYZ endToChangePoint = line.GetEndPoint((int)endToChange);
            XYZ lineDirection = line.Direction;


            double newPointX = endToChangePoint.X;
            double newPointY = endToChangePoint.Y;
            double newPointZ = endToChangePoint.Z;

            if (lineDirection.X <= AngleTolerance * 2 && lineDirection.X > VertexTolerance)
            {
                newPointX = endToMaintainPoint.X;
            }

            if (lineDirection.Y <= AngleTolerance * 2 && lineDirection.Y > VertexTolerance)
            {
                newPointY = endToMaintainPoint.Y;
            }

            if (lineDirection.Z <= AngleTolerance * 2 && lineDirection.Z > VertexTolerance)
            {
                newPointZ = endToMaintainPoint.Z;
            }

            XYZ newPoint = new XYZ(newPointX, newPointY, newPointZ);
            Line newLine = RemakeLineWithNewPointMantainingDirection(line, newPoint, endToChange);

            return newLine;
        }


        //Arc
        public static Arc ProjectArcOnGlobalPlane(Arc arc, GlobalPlane globalPlane)
        {
            XYZ originalStartPoint = arc.GetEndPoint(0);
            XYZ originalEndPoint = arc.GetEndPoint(1);
            XYZ originalMiddlePoint = arc.Evaluate(0.5, true);
            XYZ newStartPoint = ProjectPointOnGlobalPlane(originalStartPoint, globalPlane);
            XYZ newEndPoint = ProjectPointOnGlobalPlane(originalEndPoint, globalPlane);
            XYZ newMiddlePoint = ProjectPointOnGlobalPlane(originalMiddlePoint, globalPlane);
            Arc newArc = Arc.Create(newStartPoint, newEndPoint, newMiddlePoint);

            return newArc;
        }
        public static Arc ProjectArcOnGlobalPlane(Arc arc, GlobalPlane globalPlane, int digitsToRoundCoordinates)
        {
            XYZ originalStartPoint = arc.GetEndPoint(0);
            XYZ originalEndPoint = arc.GetEndPoint(1);
            XYZ originalMiddlePoint = arc.Evaluate(0.5, true);
            XYZ newStartPoint = ProjectPointOnGlobalPlane(originalStartPoint, globalPlane, digitsToRoundCoordinates);
            XYZ newEndPoint = ProjectPointOnGlobalPlane(originalEndPoint, globalPlane, digitsToRoundCoordinates);
            XYZ newMiddlePoint = ProjectPointOnGlobalPlane(originalMiddlePoint, globalPlane, digitsToRoundCoordinates);
            Arc newArc = Arc.Create(newStartPoint, newEndPoint, newMiddlePoint);

            return newArc;
        }
        public static Arc ProjectArcOnPlaneByPlanarFace(Arc arc, PlanarFace planarFace)
        {
            XYZ originalStartPoint = arc.GetEndPoint(0);
            XYZ originalEndPoint = arc.GetEndPoint(1);
            XYZ originalMiddlePoint = arc.Evaluate(0.5, true);
            XYZ newStartPoint = ProjectPointOnFacePlane(originalStartPoint, planarFace);
            XYZ newEndPoint = ProjectPointOnFacePlane(originalEndPoint, planarFace);
            XYZ newMiddlePoint = ProjectPointOnFacePlane(originalMiddlePoint, planarFace);
            Arc newArc = Arc.Create(newStartPoint, newEndPoint, newMiddlePoint);

            return newArc;
        }
        public static Arc ProjectArcOnPlaneByPlanarFace(Arc arc, PlanarFace planarFace, int digitsToRoundCoordinates)
        {
            XYZ originalStartPoint = arc.GetEndPoint(0);
            XYZ originalEndPoint = arc.GetEndPoint(1);
            XYZ originalMiddlePoint = arc.Evaluate(0.5, true);
            XYZ newStartPoint = ProjectPointOnFacePlane(originalStartPoint, planarFace, digitsToRoundCoordinates);
            XYZ newEndPoint = ProjectPointOnFacePlane(originalEndPoint, planarFace, digitsToRoundCoordinates);
            XYZ newMiddlePoint = ProjectPointOnFacePlane(originalMiddlePoint, planarFace, digitsToRoundCoordinates);
            Arc newArc = Arc.Create(newStartPoint, newEndPoint, newMiddlePoint);

            return newArc;
        }
        public static Arc ProjectArcOnPlaneByOriginAndNormal(Arc arc, XYZ origin, XYZ normal)
        {
            XYZ originalStartPoint = arc.GetEndPoint(0);
            XYZ originalEndPoint = arc.GetEndPoint(1);
            XYZ originalMiddlePoint = arc.Evaluate(0.5, true);
            XYZ newStartPoint = ProjectPointByOriginAndNormal(originalStartPoint, origin, normal);
            XYZ newEndPoint = ProjectPointByOriginAndNormal(originalEndPoint, origin, normal);
            XYZ newMiddlePoint = ProjectPointByOriginAndNormal(originalMiddlePoint, origin, normal);
            Arc newArc = Arc.Create(newStartPoint, newEndPoint, newMiddlePoint);

            return newArc;
        }
        public static Arc ProjectArcOnPlaneByOriginAndNormal(Arc arc, XYZ origin, XYZ normal, int digitsToRoundCoordinates)
        {
            XYZ originalStartPoint = arc.GetEndPoint(0);
            XYZ originalEndPoint = arc.GetEndPoint(1);
            XYZ originalMiddlePoint = arc.Evaluate(0.5, true);
            XYZ newStartPoint = ProjectPointByOriginAndNormal(originalStartPoint, origin, normal, digitsToRoundCoordinates);
            XYZ newEndPoint = ProjectPointByOriginAndNormal(originalEndPoint, origin, normal, digitsToRoundCoordinates);
            XYZ newMiddlePoint = ProjectPointByOriginAndNormal(originalMiddlePoint, origin, normal, digitsToRoundCoordinates);
            Arc newArc = Arc.Create(newStartPoint, newEndPoint, newMiddlePoint);

            return newArc;
        }


        //Ellipse
        public static Ellipse ProjectEllipseOnGlobalPlane(Ellipse ellipse, GlobalPlane globalPlane)
        {
            XYZ originalCenter = ellipse.Center;
            XYZ originalXDirection = ellipse.XDirection;
            XYZ originalYDirection = ellipse.YDirection;

            XYZ newCenter = ProjectPointOnGlobalPlane(originalCenter, globalPlane);
            XYZ newXDirection = ProjectPointOnGlobalPlane(originalXDirection, globalPlane);
            XYZ newYDirection = ProjectPointOnGlobalPlane(originalYDirection, globalPlane);
            double newXRadius = newXDirection.DistanceTo(newCenter);
            double newYRadius = newYDirection.DistanceTo(newCenter);
            double normalizedStartParameter = ellipse.ComputeNormalizedParameter(ellipse.GetEndParameter(0));
            double normalizedEndParameter = ellipse.ComputeNormalizedParameter(ellipse.GetEndParameter(1));

            Curve newEllipse = Ellipse.CreateCurve(newCenter, newXRadius, newYRadius, newXDirection, newYDirection, normalizedStartParameter, normalizedEndParameter);
            
            return newEllipse as Ellipse;
        }
        public static Ellipse ProjectEllipseOnGlobalPlane(Ellipse ellipse, GlobalPlane globalPlane, int digitsToRoundCoordinates)
        {
            XYZ originalCenter = ellipse.Center;
            XYZ originalXDirection = ellipse.XDirection;
            XYZ originalYDirection = ellipse.YDirection;

            XYZ newCenter = ProjectPointOnGlobalPlane(originalCenter, globalPlane, digitsToRoundCoordinates);
            XYZ newXDirection = ProjectPointOnGlobalPlane(originalXDirection, globalPlane, digitsToRoundCoordinates);
            XYZ newYDirection = ProjectPointOnGlobalPlane(originalYDirection, globalPlane, digitsToRoundCoordinates);
            double newXRadius = newXDirection.DistanceTo(newCenter);
            double newYRadius = newYDirection.DistanceTo(newCenter);
            double normalizedStartParameter = ellipse.ComputeNormalizedParameter(ellipse.GetEndParameter(0));
            double normalizedEndParameter = ellipse.ComputeNormalizedParameter(ellipse.GetEndParameter(1));

            Curve newEllipse = Ellipse.CreateCurve(newCenter, newXRadius, newYRadius, newXDirection, newYDirection, normalizedStartParameter, normalizedEndParameter);

            return newEllipse as Ellipse;
        }

        /*
        NÃO IMPLEMENTADAS
        public static Ellipse ProjectArcOnPlaneByPlanarFace(Ellipse ellipse, PlanarFace planarFace)
        {
            throw new NotImplementedException();
        }
        public static Ellipse ProjectArcOnPlaneByPlanarFace(Ellipse ellipse, PlanarFace planarFace, int digitsToRoundCoordinates)
        {
            throw new NotImplementedException();
        }
        public static Ellipse ProjectArcOnPlaneByOriginAndNormal(Ellipse ellipse, XYZ origin, XYZ normal)
        {
            throw new NotImplementedException();
        }
        public static Ellipse ProjectArcOnPlaneByOriginAndNormal(Ellipse ellipse, XYZ origin, XYZ normal, int digitsToRoundCoordinates)
        {
            throw new NotImplementedException();
        }
        */

        //Plane
        public enum GlobalPlane
        {
            XYPlane,
            XZPlane,
            YZPlane
        }
        public static XYZ GetGlobalPlaneNormal(GlobalPlane globalPlane)
        {
            switch (globalPlane)
            {
                case GlobalPlane.XYPlane:
                    return XYZ.BasisZ;

                case GlobalPlane.YZPlane:
                    return XYZ.BasisX;

                case GlobalPlane.XZPlane:
                    return XYZ.BasisZ;
            }

            return null;
        }


        //PlanarFace
        public static List<PlanarFace> GetFacesParallelToGlobalPlane(List<PlanarFace> faceList, GlobalPlane globalPlane)
        {
            XYZ globalPlaneNormal = GetGlobalPlaneNormal(globalPlane);
            List<PlanarFace> globalPlaneOrthogonalFaces = faceList
                    .Where(x => AreVectorsAlmostParallel(x.FaceNormal, globalPlaneNormal))
                    .ToList();

            return globalPlaneOrthogonalFaces;
        }
        public static List<PlanarFace> GetXOrthogonalFaces(List<PlanarFace> faceList)
        {
            List<PlanarFace> xOrthogonalFaces = faceList
                    .Where(x => Math.Round(x.FaceNormal.X, 2) == 0)
                    .ToList();

            return xOrthogonalFaces;
        }
        public static List<PlanarFace> GetYOrthogonalFaces(List<PlanarFace> faceList)
        {
            List<PlanarFace> yOrthogonalFaces = faceList
                    .Where(x => Math.Round(x.FaceNormal.Y, 2) == 0)
                    .ToList();

            return yOrthogonalFaces;
        }
        public static List<PlanarFace> GetZOrthogonalFaces(List<PlanarFace> faceList)
        {
            List<PlanarFace> zOrthogonalFaces = faceList
                    .Where(x => Math.Round(x.FaceNormal.Z, 2) == 0)
                    .ToList();

            return zOrthogonalFaces;
        }
        //TODO:verificar se GetLineCurveLoopsFromFace é mais estável que o .GetEdgesAsCurveLoops() do revit
        public static List<CurveLoop> GetLineCurveLoopsFromFace(PlanarFace planarFace, double smallLinesTolerance)
        {
            EdgeArrayArray edgeArrays = planarFace.EdgeLoops;
            List<CurveLoop> curveLoops = new List<CurveLoop>();

            foreach (EdgeArray edges in edgeArrays)
            {
                List<XYZ> sequentialVertices = new List<XYZ>();

                foreach (Edge edge in edges)
                {
                    sequentialVertices.Add(edge.Evaluate(0));
                }

                sequentialVertices.Add(sequentialVertices[0]);
                List<XYZ> filteredSequentialVertices = PruneSequentialDuplicatedPoints(sequentialVertices, smallLinesTolerance);
                CurveLoop curveloop = CreateCurveLoopFromSequentialLineVertices(filteredSequentialVertices, true);
                curveLoops.Add(curveloop);
            }

            return curveLoops;
        }
        public static PlanarFace ProjectFaceOnZWithEdgeArray(PlanarFace planarFace)
        {
            //ProjectFaceOnZWithEdgeArray não possibilita a projeção de curvas que não sejam linhas

            EdgeArrayArray edgeArrays = planarFace.EdgeLoops;
            List<CurveLoop> curveLoops = new List<CurveLoop>();

            foreach (EdgeArray edges in edgeArrays)
            {
                List<XYZ> sequentialVertices = new List<XYZ>();

                foreach (Edge edge in edges)
                {
                    XYZ originalVertex = edge.Evaluate(0);
                    XYZ projectedVertex = new XYZ(originalVertex.X, originalVertex.Y, 0);
                    sequentialVertices.Add(projectedVertex);
                }

                sequentialVertices.Add(sequentialVertices[0]);
                List<XYZ> filteredSequentialVertices = PruneSequentialDuplicatedPoints(sequentialVertices, 0.01);
                CurveLoop curveloop = CreateCurveLoopFromSequentialLineVertices(filteredSequentialVertices, true);
                curveLoops.Add(curveloop);
            }

            Solid newSolid = GeometryCreationUtilities.CreateExtrusionGeometry(curveLoops, XYZ.BasisZ, 1);

            List<PlanarFace> newFaceList = TransformFaceArrayIntoList(newSolid);
            List<PlanarFace> zNormalFaces = GetFacesParallelToGlobalPlane(newFaceList, GlobalPlane.XYPlane);
            PlanarFace zNormalFaceAtOrigin = GetZNormalFaceAtOrigin(zNormalFaces);

            return zNormalFaceAtOrigin;
        }
        public static PlanarFace ProjectFaceOnZWithGetEdgesAsCurveLoops(PlanarFace planarFace, GlobalPlane globalPlane)
        {
            List<CurveLoop> zProjectedCurveLoops = new List<CurveLoop>();
            List<CurveLoop> curveLoops = planarFace.GetEdgesAsCurveLoops()
                .ToList();

            foreach (CurveLoop curveLoop in curveLoops)
            {
                List<Curve> sequentialCurves = GetSequentialCurvesFromCurveLoop(curveLoop);
                List<Curve> projectedCurves = ProjectMultipleCurvesOnGlobalPlane(sequentialCurves, globalPlane);

                CurveLoop filteredCurveLoop = new CurveLoop();

                foreach (Curve projectedCurve in projectedCurves)
                {
                    filteredCurveLoop.Append(projectedCurve);
                }

                zProjectedCurveLoops.Add(filteredCurveLoop);
            }

            XYZ originalFaceNormal = planarFace.FaceNormal;
            Solid newSolid = GeometryCreationUtilities.CreateExtrusionGeometry(zProjectedCurveLoops, XYZ.BasisZ, 1);
            List<PlanarFace> newFaceList = TransformFaceArrayIntoList(newSolid);
            PlanarFace newFace = newFaceList
                .Where(x => x.Origin.Z == 0)
                .First();

            return newFace;
        }
        public static List<PlanarFace> PlanifyGeometry(List<PlanarFace> faceList)
        {
            faceList = faceList
                .OrderBy(x => Math.Round(x.FaceNormal.Z, 2))
                .Reverse()
                .ToList();

            PlanarFace ZNormalClosestFace = faceList.First();

            List<XYZ> faceVertices = ZNormalClosestFace.Triangulate(1).Vertices.ToList();

            List<XYZ> vertices = new List<XYZ>();

            foreach (XYZ faceVertex in faceVertices)
            {
                XYZ point = new XYZ(faceVertex.X, faceVertex.Y, 0);
                vertices.Add(point);
            }

            CurveLoop curveLoop = CreateCurveLoopFromSequentialLineVertices(vertices, true);

            List<CurveLoop> curveLoopList = new List<CurveLoop> { curveLoop };
            Solid newSolid = GeometryCreationUtilities.CreateExtrusionGeometry(curveLoopList, XYZ.BasisZ, 1);

            List<PlanarFace> newFaceList = TransformFaceArrayIntoList(newSolid);
            List<PlanarFace> zNormalFaces = GetFacesParallelToGlobalPlane(newFaceList, GlobalPlane.XYPlane);

            return zNormalFaces;
        }
        public static PlanarFace MoveFaceToZBasisPlane(PlanarFace face)
        {
            Transform zBasisPlaneTransform = GetProjectionOnZBasisPlaneTransform(face.Origin);
            IList<CurveLoop> curveLoops = face.GetEdgesAsCurveLoops();
            List<CurveLoop> projectedCurveLoops = new List<CurveLoop>();

            foreach (CurveLoop curveLoop in curveLoops)
            {
                CurveLoop projectedCurveLoop = CurveLoop.CreateViaTransform(curveLoop, zBasisPlaneTransform);
                projectedCurveLoops.Add(projectedCurveLoop);
            }

            PlanarFace zNormalFaceAtOrigin = CreateZNormalFaceAtOriginFromCurveLoops(projectedCurveLoops);

            return zNormalFaceAtOrigin;
        }
        public static Transform GetProjectionOnZBasisPlaneTransform(XYZ originPoint)
        {
            XYZ pointProjectedOnBasisZPlane = new XYZ(originPoint.X, originPoint.Y, 0);
            XYZ projectionVector = pointProjectedOnBasisZPlane - originPoint;
            Transform objectTranslation = Transform.CreateTranslation(projectionVector);

            return objectTranslation;
        }
        public static PlanarFace GetZNormalFaceAtOrigin(List<PlanarFace> faceList)
        {
            //TODO: adicionar try catch para o caso de não haver face na origem
            PlanarFace zNormalFaceAtOrigin = faceList
                .Where(x => Math.Round(x.FaceNormal.Z, 2) == 1 || Math.Round(x.FaceNormal.Z, 2) == -1)
                .Where(x => x.Origin.Z == 0)
                .First();

            return zNormalFaceAtOrigin;
        }
        public static PlanarFace CreateZNormalFaceAtOriginFromCurveLoops(List<CurveLoop> curveLoops)
        {
            Solid solid;

            try
            {
                solid = GeometryCreationUtilities.CreateExtrusionGeometry(curveLoops, XYZ.BasisZ, 1);
            }
            catch (Exception)
            {
                curveLoops = PurgeMultipleCurveLoopsSmallLinesAndTrimAdjacentLines(curveLoops);
                solid = GeometryCreationUtilities.CreateExtrusionGeometry(curveLoops, XYZ.BasisZ, 1);
            }

            List<PlanarFace> faceList = TransformFaceArrayIntoList(solid);
            PlanarFace zNormalFaceAtOrigin = GetZNormalFaceAtOrigin(faceList);

            return zNormalFaceAtOrigin;
        }
        public static PlanarFace CreateFaceToBothSidesOfLineWithVector(Line line, XYZ vector)
        {
            XYZ lineStartPoint = line.GetEndPoint(0);
            XYZ lineEndPoint = line.GetEndPoint(1);

            XYZ firstVertex = lineStartPoint + vector;
            XYZ secondVertex = lineEndPoint + vector;
            XYZ thirdVertex = lineEndPoint - vector;
            XYZ fourthVertex = lineStartPoint - vector;

            List<XYZ> sequentialVertices = new List<XYZ> { firstVertex, secondVertex, thirdVertex, fourthVertex, firstVertex };
            CurveLoop curveLoop = CreateCurveLoopFromSequentialLineVertices(sequentialVertices, true);
            List<CurveLoop> curveLoops = new List<CurveLoop> { curveLoop };
            Solid solid = GeometryCreationUtilities.CreateExtrusionGeometry(curveLoops, XYZ.BasisZ, 1);
            List<PlanarFace> faceList = TransformFaceArrayIntoList(solid);
            PlanarFace faceWithClosestNormalToZDown = GetFaceOnZOrigin(faceList);

            return faceWithClosestNormalToZDown;
        }
        public static PlanarFace GetFaceOnZOrigin(List<PlanarFace> faceList)
        {
            PlanarFace faceWithClosestNormalToZDown = faceList
                .Where(x => x.Origin.Z == 0)
                .First();

            return faceWithClosestNormalToZDown;
        }
        public static PlanarFace GetPlanarFaceIntersection(Solid firstSolid, Solid secondSolid)
        {
            Solid intersection = BooleanOperationsUtils.ExecuteBooleanOperation(firstSolid, secondSolid, BooleanOperationsType.Intersect);
            List<PlanarFace> zPlanarFaces = PickZNormalFacesFromSolid(intersection);
            PlanarFace zFace = zPlanarFaces.FirstOrDefault();

            return zFace;
        }
        public static Solid RemakeFaceExtrusionWithoutSmallLines(PlanarFace face)
        {
            List<CurveLoop> faceCurveLoops = face.GetEdgesAsCurveLoops().ToList();
            List<CurveLoop> correctedCurveLoops = CorrectCurvesThatDontConnectInMultipleCurveLoops(faceCurveLoops);
            List<CurveLoop> continuousCurveLoops = JoinContinuousLinesOnMultipleCurveLoops(correctedCurveLoops);
            List<CurveLoop> filteredFaceCurveLoops = PurgeMultipleCurveLoopsSmallLinesAndTrimAdjacentLines(continuousCurveLoops);
            Solid newSolid = GeometryCreationUtilities.CreateExtrusionGeometry(filteredFaceCurveLoops, XYZ.BasisZ, 1);

            return newSolid;
        }
        public static bool AreThereFacesParallelToGlobalPlane(List<PlanarFace> faceList, GlobalPlane globalPlane)
        {
            XYZ globalPlaneNormal = GetGlobalPlaneNormal(globalPlane);
            bool areThereFacesParallelToGlobalPlane = faceList
                    .Any(x => AreVectorsAlmostParallel(x.FaceNormal, globalPlaneNormal));

            return areThereFacesParallelToGlobalPlane;
        }
        private static PlanarFace PickSolidParallelXYFaceClosestToPlane(Solid solid)
        {
            List<PlanarFace> ZPlanarFaces = PickFacesParallelToGlobalPlaneFromSolid(solid, GlobalPlane.XYPlane);
            PlanarFace lowestZNormalFace = ZPlanarFaces
                                .OrderBy(x => x.Origin.Z)
                                .First();

            return lowestZNormalFace;
        }


        



        //Solid
        public static List<PlanarFace> TransformFaceArrayIntoList(Solid solid)
        {
            FaceArray faces = solid.Faces;
            FaceArrayIterator faceIterator = faces.ForwardIterator();
            faceIterator.Reset();

            List<PlanarFace> faceList = new List<PlanarFace>();

            while (faceIterator.MoveNext())
            {
                PlanarFace face = faceIterator.Current as PlanarFace;

                if (face != null)
                {
                    faceList.Add(face);
                }
            }

            return faceList;
        }
        public static Solid TranslateSolidByVector(Solid solid, XYZ vector)
        {
            Autodesk.Revit.DB.Transform objectTranslation = Autodesk.Revit.DB.Transform.CreateTranslation(vector);
            Solid translatedSolid = SolidUtils.CreateTransformed(solid, objectTranslation);

            return translatedSolid;
        }
        public static XYZ GetVectorBetweenOriginAndSolidCentroid(Solid solid)
        {
            XYZ solidCentroid = solid.ComputeCentroid();
            XYZ origin = XYZ.Zero;

            XYZ centroidToOriginVector = origin - solidCentroid;

            return centroidToOriginVector;
        }
        public static Solid GetSolidFromGeometry(GeometryElement geometry)
        {
            if (geometry.OfType<Solid>().ToList().Count > 1)
            {
                TaskDialog.Show("Erro", "Mais de um sólido no objeto. Mandar o modelo para a iterCode para análise.");
            }


            Solid solid = geometry
                .OfType<Solid>()
                .First();

            return solid;
        }
        public static List<XYZ> GetSolidOutwardFaceDirections(Solid solid, List<PlanarFace> faces)
        {
            List<XYZ> outwardFaceDirections = new List<XYZ>();

            foreach (PlanarFace face in faces)
            {
                XYZ outwardFaceDirection = GetSolidOutwardFaceDirection(solid, face);
                outwardFaceDirections.Add(outwardFaceDirection);
            }

            return outwardFaceDirections;
        }
        public static XYZ GetSolidOutwardFaceDirection(Solid solid, PlanarFace face)
        {
            XYZ normalVector = face.ComputeNormal(new UV(0.5, 0.5));
            BoundingBoxUV faceBoundingBox = face.GetBoundingBox();
            XYZ faceMiddlePoint = face.Evaluate(new UV((faceBoundingBox.Min.U + faceBoundingBox.Max.U) * 0.5, (faceBoundingBox.Min.V + faceBoundingBox.Max.V) * 0.5));
            XYZ translatedPoint = faceMiddlePoint + normalVector;

            if (!IsPointInsideSolid(solid, translatedPoint))
            {
                normalVector = normalVector.Negate();
            }

            return normalVector;
        }
        public static Solid CreateExtrudedSolidFromPlanarFace(PlanarFace face)
        {
            IList<CurveLoop> planeCurveLoop = face.GetEdgesAsCurveLoops();
            Solid planeExtrudedSolid;

            try
            {
                planeExtrudedSolid = GeometryCreationUtilities.CreateExtrusionGeometry(planeCurveLoop, face.FaceNormal, 1);
            }
            catch (Exception)
            {

                planeExtrudedSolid = RemakeFaceExtrusionWithoutSmallLines(face);
            }
            
            return planeExtrudedSolid;  
        }
        public static Solid ThickenPlanarFaceByValue(PlanarFace face, double thickness)
        {
            IList<CurveLoop> planeCurveLoop = face.GetEdgesAsCurveLoops();
            Solid planeExtrudedSolid = GeometryCreationUtilities.CreateExtrusionGeometry(planeCurveLoop, face.FaceNormal, thickness);
            XYZ translationVector = face.FaceNormal * -(thickness / 2);
            Solid thickenedPlanarFace = TranslateSolidByVector(planeExtrudedSolid, translationVector);

            return thickenedPlanarFace;
        }
        public static Solid ScaleSolidByValue(Solid solid, double scale)
        {
            XYZ centroidToOriginVector = GetVectorBetweenOriginAndSolidCentroid(solid);
            Solid translatedSolid = TranslateSolidByVector(solid, centroidToOriginVector);
            Autodesk.Revit.DB.Transform scaleTransform = Autodesk.Revit.DB.Transform.CreateTranslation(XYZ.Zero).ScaleBasis(scale);
            Solid scaledSolidAtOrigin = SolidUtils.CreateTransformed(translatedSolid, scaleTransform);
            Autodesk.Revit.DB.Transform translationToOriginalPlace = Autodesk.Revit.DB.Transform.CreateTranslation(centroidToOriginVector.Negate());
            Solid scaledSolid = SolidUtils.CreateTransformed(scaledSolidAtOrigin, translationToOriginalPlace);

            return scaledSolid;
        }
        public static List<PlanarFace> PickFacesParallelToGlobalPlaneFromSolid(Solid solid, GlobalPlane globalPlane)
        {
            List<PlanarFace> faceList = TransformFaceArrayIntoList(solid);
            List<PlanarFace> zNormalFaces = GetFacesParallelToGlobalPlane(faceList, globalPlane);

            return zNormalFaces;
        }
        public static List<PlanarFace> PickZNormalFacesFromSolid(Solid solid)
        {
            List<PlanarFace> faceList = TransformFaceArrayIntoList(solid);
            List<PlanarFace> zNormalFaces = GetFacesParallelToGlobalPlane(faceList, GlobalPlane.XYPlane);

            return zNormalFaces;
        }
        public static Solid RemakeExtrudedSolidOnZWithoutSmallLines(Solid solid)
        {
            List<PlanarFace> faceList = TransformFaceArrayIntoList(solid);
            PlanarFace lowestZFace = faceList.Where(x => x.Origin.Z == faceList.Min(y => y.Origin.Z)).First();
            List<CurveLoop> originalFaceCurveLoops = lowestZFace.GetEdgesAsCurveLoops().ToList();
            List<CurveLoop> filteredCurveLoops = PurgeMultipleCurveLoopsSmallLinesAndTrimAdjacentLines(originalFaceCurveLoops);
            Solid newSolid = GeometryCreationUtilities.CreateExtrusionGeometry(filteredCurveLoops, XYZ.BasisZ, 1);

            return newSolid;
        }



        public static Solid GetSolidOnXYPlane(Solid solid)
        {
            List<PlanarFace> planarFaces = TransformFaceArrayIntoList(solid);
            Solid translatedSolid;

            if (!AreThereFacesParallelToGlobalPlane(planarFaces, GlobalPlane.XYPlane))
            {
                try
                {
                    translatedSolid = RemakeSlopedSolidWithPlanifiedGeometryOnXYPlane(solid);
                    return translatedSolid;
                }
                catch (Exception)
                {
                    return null;
                }
                
            }

            translatedSolid = TranslateSolidToXYPlane(solid);
            return translatedSolid;
        }
        public static Solid RemakeSlopedSolidWithPlanifiedGeometryOnXYPlane(Solid solid)
        {
            PlanarFace biggestSolidFace = GetSolidFaceWithBiggestArea(solid);
            List<CurveLoop> faceCurveLoops = biggestSolidFace.GetEdgesAsCurveLoops().ToList();
            
            List<CurveLoop> newFaceCurveLoops = new List<CurveLoop>();

            foreach (CurveLoop curveLoop in faceCurveLoops)
            {
                List<Curve> curveLoopCurves = GetSequentialCurvesFromCurveLoop(curveLoop);
                List<Curve> projectedCurveLoopCurves = ProjectMultipleCurvesOnGlobalPlane(curveLoopCurves, GlobalPlane.XYPlane);
                List<Curve> correctedProjectedCurveLoopCurves = CorrectCurvesThatDontConnectInCurveLoopSequentialCurves(projectedCurveLoopCurves);
                List<Curve> joinedProjectedCurveLoopCurves = JoinContinuousLinesOnCurveList(correctedProjectedCurveLoopCurves);
                List<Curve> filteredProjectedCurveLoopCurves = PurgeSequentialCurvesSmallLinesAndTrimAdjacent(joinedProjectedCurveLoopCurves);
                
                CurveLoop newCurveLoop = new CurveLoop();

                foreach (Curve curve in filteredProjectedCurveLoopCurves)
                {
                    newCurveLoop.Append(curve);
                }

                newFaceCurveLoops.Add(newCurveLoop);
            }

            Solid newSolid = GeometryCreationUtilities.CreateExtrusionGeometry(newFaceCurveLoops, XYZ.BasisZ, 1);

            return newSolid;
        }
        public static PlanarFace GetSolidFaceWithClosestXYPlaneInclination(Solid solid)
        {
            List<PlanarFace> faceList = TransformFaceArrayIntoList(solid);
            PlanarFace zClosestNormalFace = faceList
                .OrderByDescending(x => x.FaceNormal.Z)
                .First();

            PlanarFace zNormalFaceAtOrigin = ProjectFaceOnZWithGetEdgesAsCurveLoops(zClosestNormalFace, GlobalPlane.XYPlane);

            return zNormalFaceAtOrigin;
        }
        public static PlanarFace GetSolidFaceWithBiggestArea(Solid solid)
        {
            List<PlanarFace> faceList = TransformFaceArrayIntoList(solid);
            PlanarFace zClosestNormalFace = faceList
                .Where(x => Math.Round(x.FaceNormal.Z,2) != 0)
                .OrderByDescending(x => x.Area)
                .First();

            PlanarFace zNormalFaceAtOrigin = ProjectFaceOnZWithGetEdgesAsCurveLoops(zClosestNormalFace, GlobalPlane.XYPlane);

            return zNormalFaceAtOrigin;
        }
        public static Solid TranslateSolidToXYPlane(Solid solid)
        {
            PlanarFace lowestZFace = PickSolidParallelXYFaceClosestToPlane(solid);
            XYZ faceOrigin = lowestZFace.Origin;
            XYZ faceOriginOnXYPlane = ProjectPointOnGlobalPlane(faceOrigin, GlobalPlane.XYPlane);
            XYZ solidTranslationVector = faceOriginOnXYPlane - faceOrigin;
            Solid translatedSolid = TranslateSolidByVector(solid, solidTranslationVector);

            return translatedSolid;
        }
        



    }


}

    

