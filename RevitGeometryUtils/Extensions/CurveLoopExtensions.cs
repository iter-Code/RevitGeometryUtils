using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevitGeometryUtils.Extensions
{
    public static class CurveLoopExtensions
    {
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

            if (intersectionResultArray == null && extendedFirstLine.Origin.Z == extendedSecondLine.Origin.Z)
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
    }
}
