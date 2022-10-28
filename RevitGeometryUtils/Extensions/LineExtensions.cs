using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Mechanical;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevitGeometryUtils.Extensions
{
    public static class LineExtensions
    {
        public enum CurveEnd
        {
            Start,
            End
        }

        public enum GlobalPlane
        {
            XYPlane,
            XZPlane,
            YZPlane
        }

        public static double AngleTolerance = 0.00174532925199433;

        //Line
        public static Line ProjectOnGlobalPlane(this Line line, GlobalPlane globalPlane)
        {
            XYZ originalStartPoint = line.GetEndPoint(0);
            XYZ originalEndPoint = line.GetEndPoint(1);
            XYZ newStartPoint = originalStartPoint.ProjectOnGlobalPlane(globalPlane);
            XYZ newEndPoint = originalEndPoint.ProjectOnGlobalPlane(globalPlane);
            Line newLine = Line.CreateBound(newStartPoint, newEndPoint);

            return newLine;
        }
        public static Line ProjectOnGlobalPlane(this Line line, GlobalPlane globalPlane, int digitsToRoundCoordinates)
        {
            XYZ originalStartPoint = line.GetEndPoint(0);
            XYZ originalEndPoint = line.GetEndPoint(1);
            XYZ newStartPoint = originalStartPoint.ProjectOnGlobalPlane(globalPlane, digitsToRoundCoordinates);
            XYZ newEndPoint = originalEndPoint.ProjectOnGlobalPlane(globalPlane, digitsToRoundCoordinates);
            Line newLine = Line.CreateBound(newStartPoint, newEndPoint);

            return newLine;
        }
        public static Line ProjectLineOnPlaneByPlanarFace(this Line line, PlanarFace planarFace)
        {
            XYZ originalStartPoint = line.GetEndPoint(0);
            XYZ originalEndPoint = line.GetEndPoint(1);
            XYZ newStartPoint = originalStartPoint.ProjectOnSamePlaneAsPlanarFace(planarFace);
            XYZ newEndPoint = originalEndPoint.ProjectOnSamePlaneAsPlanarFace(planarFace);
            Line newLine = Line.CreateBound(newStartPoint, newEndPoint);

            return newLine;
        }
        public static Line ProjectLineOnPlaneByPlanarFace(this Line line, PlanarFace planarFace, int digitsToRoundCoordinates)
        {
            XYZ originalStartPoint = line.GetEndPoint(0);
            XYZ originalEndPoint = line.GetEndPoint(1);
            XYZ newStartPoint = originalStartPoint.ProjectOnSamePlaneAsPlanarFace(planarFace, digitsToRoundCoordinates);
            XYZ newEndPoint = originalEndPoint.ProjectOnSamePlaneAsPlanarFace(planarFace, digitsToRoundCoordinates);
            Line newLine = Line.CreateBound(newStartPoint, newEndPoint);

            return newLine;
        }
        public static Line ProjectLineOnPlaneByOriginAndNormal(this Line line, XYZ origin, XYZ normal)
        {
            XYZ originalStartPoint = line.GetEndPoint(0);
            XYZ originalEndPoint = line.GetEndPoint(1);
            XYZ newStartPoint = originalStartPoint.ProjectOnPlaneByPlaneOriginAndNormal(origin, normal);
            XYZ newEndPoint = originalEndPoint.ProjectOnPlaneByPlaneOriginAndNormal(origin, normal);
            Line newLine = Line.CreateBound(newStartPoint, newEndPoint);

            return newLine;
        }
        public static Line ProjectLineOnPlaneByOriginAndNormal(this Line line, XYZ origin, XYZ normal, int digitsToRoundCoordinates)
        {
            XYZ originalStartPoint = line.GetEndPoint(0);
            XYZ originalEndPoint = line.GetEndPoint(1);
            XYZ newStartPoint = originalStartPoint.ProjectOnPlaneByPlaneOriginAndNormal(origin, normal, digitsToRoundCoordinates);
            XYZ newEndPoint = originalEndPoint.ProjectOnPlaneByPlaneOriginAndNormal(origin, normal, digitsToRoundCoordinates);
            Line newLine = Line.CreateBound(newStartPoint, newEndPoint);

            return newLine;
        }
        public static bool IsAlmostParallelTo(this Line firstLine, Line secondLine, double tolerance = 0.001)
        {
            return firstLine.Direction.IsAlmostParallelTo(secondLine.Direction);
        }
        public static double[] GetAnglesToGlobalAxes(this Line line)
        {
            XYZ lineVector = line.Direction;

            double angleToGlobalXAxis = lineVector.AngleTo(XYZ.BasisX);
            double angleToGlobalYAxis = lineVector.AngleTo(XYZ.BasisY);
            double angleToGlobalZAxis = lineVector.AngleTo(XYZ.BasisZ);
            double[] anglesToGlobalAxes = new double[3] { angleToGlobalXAxis, angleToGlobalYAxis, angleToGlobalZAxis };

            return anglesToGlobalAxes;
        }
        public static bool IsSlightlyOffAxis(this Line line)
        {
            double[] anglesToGlobalAxes = GetAnglesToGlobalAxes(line);
            return anglesToGlobalAxes.Any(x => x > AngleTolerance && x <= AngleTolerance * 2);
        }
        public static Line ExtendByEndAndValue(this Line line, double value, CurveEnd curveEnd)
        {
            int endToExtend = 1;
            int endToMaintain = 0;
            XYZ directionToExtend = line.Direction;

            if (curveEnd == CurveEnd.Start)
            {
                endToExtend = 0;
                endToMaintain = 1;
                directionToExtend = directionToExtend.Negate();
            }

            XYZ pointToMaintain = line.GetEndPoint(endToMaintain);
            XYZ pointToChange = line.GetEndPoint(endToExtend) + (directionToExtend * value);
            Line extendedLine = curveEnd == CurveEnd.Start ? Line.CreateBound(pointToChange, pointToMaintain) : Line.CreateBound(pointToMaintain, pointToChange);

            return extendedLine;
        }
        public static Line ExtendByVector(this Line line, XYZ vector)
        {
            int endToExtend = 1;
            int endToMaintain = 0;

            if (vector.IsAlmostParallelTo(line.Direction))
            {
                return null;
            }

            bool doesVectorsPointToSameDirection = vector.IsAlmostEqualTo(line.Direction);

            if (!doesVectorsPointToSameDirection)
            {
                endToExtend = 0;
                endToMaintain = 1;
            }

            XYZ pointToMaintain = line.GetEndPoint(endToMaintain);
            XYZ pointToChange = line.GetEndPoint(endToExtend) + vector;
            Line extendedLine = doesVectorsPointToSameDirection ? Line.CreateBound(pointToMaintain, pointToChange) : Line.CreateBound(pointToChange, pointToMaintain);

            return extendedLine;
        }
        public static Line ExtendLineByPointAndValue(Line line, double value, XYZ pointEndToExtend)
        {
            XYZ lineStartPoint = line.GetEndPoint(0);
            XYZ lineEndPoint = line.GetEndPoint(1);

            if (pointEndToExtend.IsNumericallyEqualTo(lineStartPoint))
            {
                return line.ExtendByEndAndValue(value, CurveEnd.Start);
            }
            else if (pointEndToExtend.IsAlmostEqualTo(lineEndPoint))
            {
                return line.ExtendByEndAndValue(value, CurveEnd.End);
            }
            else
            {
                //Retornar uma exception?
                return null;
            }
        }
        public static Line ReconstructWithNewPoint(this Line line, XYZ newPoint, CurveEnd endToChange)
        {
            XYZ newStart = line.GetEndPoint(0);
            XYZ newEnd = newPoint;

            if (endToChange == CurveEnd.Start)
            {
                newStart = newPoint;
                newEnd = line.GetEndPoint(1);
            }

            Line newLine = Line.CreateBound(newStart, newEnd);

            return newLine;
        }
        public static Line ReconstructIfSlightlyOffAxis(this Line line, CurveEnd endToChange)
        {
            int endToMaintain = (int)endToChange == 1 ? 0 : 1;

            XYZ endToMaintainPoint = line.GetEndPoint(endToMaintain);
            XYZ endToChangePoint = line.GetEndPoint((int)endToChange);
            XYZ lineDirection = line.Direction;

            double newPointX = (lineDirection.X > AngleTolerance && lineDirection.X <= AngleTolerance * 2) ? endToMaintainPoint.X : endToChangePoint.X;
            double newPointY = (lineDirection.Y > AngleTolerance && lineDirection.Y <= AngleTolerance * 2) ? endToMaintainPoint.Y : endToChangePoint.Y;
            double newPointZ = (lineDirection.Z > AngleTolerance && lineDirection.Z <= AngleTolerance * 2) ? endToMaintainPoint.Z : endToChangePoint.Z;
            
            XYZ newPoint = new XYZ(newPointX, newPointY, newPointZ);
            Line newLine = line.ReconstructWithNewPoint(newPoint, endToChange);

            return newLine;
        }


        public static XYZ GetPointIntersectionWithAnotherLineOnXYPlane(this Line firstLine, Line secondLine, double zValue)
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
        
    }
}
