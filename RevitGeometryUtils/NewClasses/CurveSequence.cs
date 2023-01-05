using Autodesk.Revit.DB;
using RevitGeometryUtils.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static RevitGeometryUtils.Extensions.CurveExtensions;

namespace RevitGeometryUtils.NewClasses
{
    public class CurveSequence
    {
        //PROPERTIES
        private List<Curve> SequentialCurves { get; set; }
        private int LastIndex { get; set; }
        public CurveSequenceType Type { get; set; }

        //ENUMERATORS
        public enum CurveSequenceType
        {
            Open,
            Closed
        }

        //CONSTRUCTORS
        public CurveSequence(CurveLoop curveLoop)
        {
            SequentialCurves = GetValidCurves(curveLoop);
            LastIndex = SequentialCurves.Count - 1;
            Type = curveLoop.IsOpen() ? CurveSequenceType.Open : CurveSequenceType.Closed;
        }


        //METHODS
        private List<Curve> GetValidCurves(CurveLoop curveLoop)
        {
            //curveLoop.ToArray();
            List<Curve> sequentialCurves = new List<Curve>();
            CurveLoopIterator curveLoopIterator = curveLoop.GetCurveLoopIterator();
            curveLoopIterator.Reset();

            while (curveLoopIterator.MoveNext())
            {
                Curve curve = curveLoopIterator.Current;

                if (curve != null)
                {
                    sequentialCurves.Add(curve);
                }
            }

            return sequentialCurves;
        }
        public CurveLoop ToCurveLoop()
        {
            return CurveLoop.Create(this.SequentialCurves);
        }




        public List<XYZ> GetSequentialVertices()
        {
            List<XYZ> sequentialVertices = this.SequentialCurves
                .Select(x => x.GetEndPoint(0))
                .ToList();

            return sequentialVertices;
        }



        /*
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

                    Line joinedLine = Line.CreateBound(firstLineStartPoint, lastLineEndPoint);
                    newListOfCurves.Add(joinedLine);
                }
                else
                {
                    newListOfCurves.Add(originalCurves[listOfIndexes[0]]);
                }
            }

            return newListOfCurves;
        }*/
        /*
        private List<List<int>> GetCurveindexesToJoinOrder()
        {
            List<Curve> curveLoopCurves = this.SequentialCurves;
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
        }*/
        private static bool IsActualCurveColinearAndContinuousToLast(Curve curve, XYZ referenceDirection)
        {
            bool isLine = curve.GetType().Name != "Line";
            bool isColinear = false;

            if (isLine)
            {
                isColinear = (curve as Line).Direction.Normalize().IsAlmostEqualTo(referenceDirection);
            }

            return isLine && isColinear ? true : false;
        }

        private bool AreThereJoinableLines()
        {
            List<Curve> curveLoopCurves = this.SequentialCurves;
            int curveLoopLastIndex = this.LastIndex;

            for (int i = 0; i < curveLoopLastIndex; i++)
            {
                bool isPairOfLines = curveLoopCurves[i].GetType().Name == "Line" && curveLoopCurves[i + 1].GetType().Name == "Line";
                
                if (isPairOfLines)
                {
                    if (IsPairOfSequenceLinesColinear(curveLoopCurves[i] as Line, curveLoopCurves[i + 1] as Line))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private bool IsPairOfSequenceLinesColinear(Line firstLine, Line secondLine)
        {
            return firstLine.Direction.IsAlmostEqualTo(secondLine.Direction) ? true : false;
        }


        public void PurgeSmallLinesAndTrimAdjacent()
        {
            List<int> smallCurvesIndexes = this.GetSmallCurvesIndexes();
            smallCurvesIndexes = RemoveSmallCurvesOnEnds(smallCurvesIndexes);

            //TODO: ESTENDER ABRANGÊNCIA PARA OUTROS TIPOS DE CURVAS
            foreach (int smallCurveIndex in smallCurvesIndexes)
            {
                int precedentCurveIndex = smallCurveIndex - 1;
                int nextCurveIndex = smallCurveIndex + 1;

                Line firstLine = this.SequentialCurves[precedentCurveIndex] as Line;
                Line lastLine = this.SequentialCurves[nextCurveIndex] as Line;

                Line[] remakedLines = TrimSequentialLinePair(firstLine, lastLine);

                this.SequentialCurves[precedentCurveIndex] = remakedLines[0];
                this.SequentialCurves[nextCurveIndex] = remakedLines[1];
            }

            
            //TODO: FALTA TRATAR OS CASOS QUE TENHAM SMALL LINES NO COMEÇO E FIM
            smallCurvesIndexes.Reverse();

            foreach (int index in smallCurvesIndexes)
            {
                this.SequentialCurves.RemoveAt(index);
            }
        }
        public void PurgeSmallLinesAndTrimAdjacent(double tolerance)
        {
            List<int> smallCurvesIndexes = this.GetSmallCurvesIndexes(tolerance);
            smallCurvesIndexes = RemoveSmallCurvesOnEnds(smallCurvesIndexes);

            //TODO: ESTENDER ABRANGÊNCIA PARA OUTROS TIPOS DE CURVAS
            foreach (int smallCurveIndex in smallCurvesIndexes)
            {
                int precedentCurveIndex = smallCurveIndex - 1;
                int nextCurveIndex = smallCurveIndex + 1;

                Line firstLine = this.SequentialCurves[precedentCurveIndex] as Line;
                Line lastLine = this.SequentialCurves[nextCurveIndex] as Line;

                Line[] remakedLines = TrimSequentialLinePair(firstLine, lastLine);

                this.SequentialCurves[precedentCurveIndex] = remakedLines[0];
                this.SequentialCurves[nextCurveIndex] = remakedLines[1];
            }


            //TODO: FALTA TRATAR OS CASOS QUE TENHAM SMALL LINES NO COMEÇO E FIM
            smallCurvesIndexes.Reverse();

            foreach (int index in smallCurvesIndexes)
            {
                this.SequentialCurves.RemoveAt(index);
            }
        }
        private List<int> RemoveSmallCurvesOnEnds(List<int> smallCurvesIndexes)
        {
            if (smallCurvesIndexes.First() == 0)
            {
                smallCurvesIndexes = RemoveSmallCurveAtBeggining(smallCurvesIndexes);
            }

            if (smallCurvesIndexes.Last() == this.LastIndex)
            {
                smallCurvesIndexes = RemoveSmallCurveAtEnd(smallCurvesIndexes);
            }

            return smallCurvesIndexes;
        }
        private List<int> RemoveSmallCurveAtBeggining(List<int> smallCurvesIndexes)
        {
            //TODO: IMPLEMENTAR FUNÇÕES DISTINTAS SE A CURVESEQUENCE FOR FECHADA OU ABERTA
            switch (this.Type)
            {
                case CurveSequenceType.Open:
                    break;
                case CurveSequenceType.Closed:
                    break;
                default:
                    break;
            }

            smallCurvesIndexes.RemoveAt(0);
            
            return smallCurvesIndexes;
        }
        private List<int> RemoveSmallCurveAtEnd(List<int> smallCurvesIndexes)
        {
            //TODO: IMPLEMENTAR FUNÇÕES DISTINTAS SE A CURVESEQUENCE FOR FECHADA OU ABERTA
            switch (this.Type)
            {
                case CurveSequenceType.Open:
                    break;
                case CurveSequenceType.Closed:
                    break;
                default:
                    break;
            }

            smallCurvesIndexes.RemoveAt(this.LastIndex);

            return smallCurvesIndexes;
        }
        private List<int> GetSmallCurvesIndexes()
        {
            List<int> smallCurvesIndices = new List<int>();

            for (int i = 0; i < this.SequentialCurves.Count; i++)
            {
                if (this.SequentialCurves[i].IsBelowLengthTolerance())
                {
                    smallCurvesIndices.Add(i);
                }
            }

            return smallCurvesIndices;
        }
        private List<int> GetSmallCurvesIndexes(double tolerance)
        {
            List<int> smallCurvesIndices = new List<int>();

            for (int i = 0; i < this.SequentialCurves.Count; i++)
            {
                if (this.SequentialCurves[i].IsBelowLengthTolerance(tolerance))
                {
                    smallCurvesIndices.Add(i);
                }
            }

            return smallCurvesIndices;
        }


        public void CorrectUnconnectedSequentialCurves()
        {
            List<Curve> curves = SequentialCurves;
            List<int[]> unconnectedCurvePairIndices = GetUnconnectedCurveIndicePairs();

            foreach (int[] pairOfUnconnectedCurveIndices in unconnectedCurvePairIndices)
            {
                Curve firstCurve = curves[pairOfUnconnectedCurveIndices[0]];
                Curve secondCurve = curves[pairOfUnconnectedCurveIndices[1]];
                string firstCurveType = firstCurve.GetType().ToString();
                string secondCurveType = secondCurve.GetType().ToString();
                
                if (firstCurveType == "Line" && secondCurveType == "Line")
                {
                    curves = ConnectPairOfLines(firstCurve as Line, secondCurve as Line, curves, pairOfUnconnectedCurveIndices);
                }

                else if (firstCurveType == "Line" && secondCurveType != "Line")
                {
                    curves[pairOfUnconnectedCurveIndices[0]] = (firstCurve as Line).ReconstructWithNewPoint(secondCurve.GetEndPoint(0), CurveEnd.End);
                }

                else if (firstCurveType != "Line" && secondCurveType == "Line")
                {
                    curves[pairOfUnconnectedCurveIndices[1]] = (secondCurve as Line).ReconstructWithNewPoint(firstCurve.GetEndPoint(1), CurveEnd.Start);
                }
            }

            SequentialCurves = curves;
        }
        private List<int[]> GetUnconnectedCurveIndicePairs()
        {
            List<int[]> unconnectedCurvePairIndexes = new List<int[]>();

            for (int i = 0; i < this.SequentialCurves.Count(); i++)
            {
                unconnectedCurvePairIndexes = AppendIndicesIfUnconnected(unconnectedCurvePairIndexes, i, i + 1);
            }

            unconnectedCurvePairIndexes = IterateLastAndFirstCurvesIfClosed(unconnectedCurvePairIndexes);

            return unconnectedCurvePairIndexes;
        }
        private List<int[]> IterateLastAndFirstCurvesIfClosed(List<int[]> unconnectedCurvePairIndices)
        {
            if (this.Type == CurveSequenceType.Closed)
            {
                AppendIndicesIfUnconnected(unconnectedCurvePairIndices, this.LastIndex, 0);
            }

            return unconnectedCurvePairIndices;
        }
        private List<int[]> AppendIndicesIfUnconnected(List<int[]> unconnectedCurvePairIndices, int actualIndex, int nextIndex)
        {
            if (!IsActualCurveEndPointEqualToNextCurveStartPoint(this.SequentialCurves[actualIndex], this.SequentialCurves[nextIndex]))
            {
                int[] curvePairIndexes = new int[2] { actualIndex, nextIndex };
                unconnectedCurvePairIndices.Add(curvePairIndexes);
            }

            return unconnectedCurvePairIndices;
        }
        private bool IsActualCurveEndPointEqualToNextCurveStartPoint(Curve actualCurve, Curve nextCurve)
        {
            XYZ actualCurveEndPoint = actualCurve.GetEndPoint(1);
            XYZ nextCurveStartPoint = nextCurve.GetEndPoint(0);

            return actualCurveEndPoint.IsAlmostEqualTo(nextCurveStartPoint) ? true : false;
        }
        private List<Curve> ConnectPairOfLines(Line firstLine, Line secondLine, List<Curve> curveLoopCurves, int[] pairOfUnconnectedCurveIndices)
        {
            if (firstLine.IsAlmostParallelTo(secondLine))
            {
                curveLoopCurves[pairOfUnconnectedCurveIndices[0]] = JoinColinearSequentialLines(firstLine, secondLine);
            }
            else
            {
                Line[] joinedLinePair = TrimSequentialLinePair(firstLine, secondLine);
                curveLoopCurves[pairOfUnconnectedCurveIndices[0]] = joinedLinePair[0];
                curveLoopCurves[pairOfUnconnectedCurveIndices[1]] = joinedLinePair[1];
            }

            return curveLoopCurves;
        }
        private Line JoinColinearSequentialLines(Line firstCurve, Line secondCurve)
        {
            XYZ newPoint = secondCurve.GetEndPoint(0);
            Line joinedLine = firstCurve.ReconstructWithNewPoint(newPoint, CurveEnd.End);

            return joinedLine;
        }
        private static Line[] TrimSequentialLinePair(Line firstLine, Line lastLine)
        {
            double valueToExtend = 10000000000;
            Line extendedFirstLine = firstLine.ExtendByEndAndValue(valueToExtend, CurveEnd.End);
            Line extendedSecondLine = lastLine.ExtendByEndAndValue(valueToExtend, CurveEnd.End);
            IntersectionResultArray intersectionResultArray = new IntersectionResultArray();
            SetComparisonResult intersectionSet = extendedFirstLine.Intersect(extendedSecondLine, out intersectionResultArray);
            List<XYZ> intersectionBetweenTwolines = new List<XYZ>();

            foreach (IntersectionResult intersectionResult in intersectionResultArray)
            {
                XYZ intersectionPoint = intersectionResult.XYZPoint;
                intersectionBetweenTwolines.Add(intersectionPoint);
            }

            XYZ firstCurvePoint = firstLine.GetEndPoint(1);
            XYZ pointOfInterest = intersectionBetweenTwolines
                .OrderBy(x => x.DistanceTo(firstCurvePoint))
                .First();

            /*
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
            }*/

            Line newFirstLine = firstLine.ReconstructWithNewPoint(pointOfInterest, CurveEnd.End);
            Line newLastLine = lastLine.ReconstructWithNewPoint(pointOfInterest, CurveEnd.Start);

            Line[] newLines = new Line[2] { newFirstLine, newLastLine };

            return newLines;
        }

        


    }
}
