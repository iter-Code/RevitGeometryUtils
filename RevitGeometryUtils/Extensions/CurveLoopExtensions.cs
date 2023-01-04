using Autodesk.Revit.DB;
using RevitGeometryUtils.NewClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevitGeometryUtils.Extensions
{
    public static class CurveLoopExtensions
    {
        /*
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
        

        public static List<XYZ> GetSequentialVerticesFromLinesCurveLoop(CurveLoop curveLoop)
        {
            List<Curve> sequentialCurves = GetSequentialCurvesFromCurveLoop(curveLoop);
            List<XYZ> sequentialVertices = GetSequentialVerticesFromSequentialLines(sequentialCurves);

            return sequentialVertices;
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
        */
       
        
    
    }
}
