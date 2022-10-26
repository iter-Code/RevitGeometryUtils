using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Mechanical;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static RevitGeometryUtils.iCGeometry;

namespace RevitGeometryUtils.Extensions
{
    public static class XYZExtensions
    {
        public static double VertexTolerance = 0.0005233832795;

        //Point 3D
        public static bool IsNumericallyEqualTo(this XYZ thisPoint, XYZ pointToCompare)
        {
            return (thisPoint.DistanceTo(pointToCompare) <= VertexTolerance) ? true : false;
        }
        public static bool IsNumericallyEqualTo(this XYZ thisPoint, XYZ pointToCompare, double tolerance)
        {
            return (thisPoint.DistanceTo(pointToCompare) <= tolerance) ? true : false;
        }
        public static XYZ RoundCoordinates(this XYZ thisPoint, int digitsToRoundCoordinates)
        {
            double roundedX = Math.Round(thisPoint.X, digitsToRoundCoordinates);
            double roundedY = Math.Round(thisPoint.Y, digitsToRoundCoordinates);
            double roundedZ = Math.Round(thisPoint.Z, digitsToRoundCoordinates);

            return new XYZ(roundedX, roundedY, roundedZ);
        }
        public static XYZ ProjectOnGlobalPlane(this XYZ pointToProject, GlobalPlane globalPlane)
        {
            switch (globalPlane)
            {
                case GlobalPlane.XYPlane:
                    return new XYZ(pointToProject.X, pointToProject.Y, 0);

                case GlobalPlane.XZPlane:
                    return new XYZ(0, pointToProject.Y, pointToProject.Z);

                case GlobalPlane.YZPlane:
                    return new XYZ(pointToProject.X, 0, pointToProject.Z);
            }

            return null;
        }
        public static XYZ ProjectOnGlobalPlane(this XYZ pointToProject, GlobalPlane globalPlane, int digitsToRoundCoordinates)
        {
            switch (globalPlane)
            {
                case GlobalPlane.XYPlane:
                    return new XYZ(Math.Round(pointToProject.X, digitsToRoundCoordinates), Math.Round(pointToProject.Y, digitsToRoundCoordinates), 0);

                case GlobalPlane.XZPlane:
                    return new XYZ(0, Math.Round(pointToProject.Y, digitsToRoundCoordinates), Math.Round(pointToProject.Z, digitsToRoundCoordinates));

                case GlobalPlane.YZPlane:
                    return new XYZ(Math.Round(pointToProject.X, digitsToRoundCoordinates), 0, Math.Round(pointToProject.Z, digitsToRoundCoordinates));
            }

            return null;
        }
        public static XYZ ProjectOnPlaneByPlaneOriginAndNormal(this XYZ pointToProject, XYZ origin, XYZ normal)
        {
            XYZ vectorBetweenOriginAndPointToProject = pointToProject - origin;
            double distanceToTranslate = vectorBetweenOriginAndPointToProject.DotProduct(normal);
            XYZ projectedPoint = pointToProject - (distanceToTranslate * normal);

            return projectedPoint;
        }
        public static XYZ ProjectOnPlaneByPlaneOriginAndNormal(this XYZ pointToProject, XYZ origin, XYZ normal, int digitsToRoundCoordinates)
        {
            XYZ vectorBetweenOriginAndPointToProject = pointToProject - origin;
            double distanceToTranslate = vectorBetweenOriginAndPointToProject.DotProduct(normal);
            XYZ projectedPoint = pointToProject - (distanceToTranslate * normal);
            XYZ roundedProjectedPoint = RoundPointCoordinates(projectedPoint, digitsToRoundCoordinates);

            return roundedProjectedPoint;
        }
        public static XYZ ProjectOnSamePlaneAsFace(this XYZ pointToProject, PlanarFace planarFace)
        {
            XYZ normal = planarFace.FaceNormal;
            XYZ origin = planarFace.Evaluate(new UV(0.5, 0.5));
            XYZ projectedPoint = pointToProject.ProjectOnPlaneByPlaneOriginAndNormal(origin, normal);

            return projectedPoint;
        }
        public static XYZ ProjectOnSamePlaneAsFace(this XYZ pointToProject, PlanarFace planarFace, int digitsToRoundCoordinates)
        {
            XYZ normal = planarFace.FaceNormal;
            XYZ origin = planarFace.Evaluate(new UV(0.5, 0.5));
            XYZ roundedProjectedPoint = pointToProject.ProjectOnPlaneByPlaneOriginAndNormal(origin, normal, digitsToRoundCoordinates);

            return roundedProjectedPoint;
        }



        //Funções que talvez seja bom deixar como métodos em vez de fazer uma extensão da classe
        public static List<XYZ> RemovePointsAtIndexes(this List<XYZ> unfilteredPoints, List<int> indexesToRemove)
        {
            for (int i = indexesToRemove.Count - 1; i >= 0; i--)
            {
                int indexToRemove = indexesToRemove[i];
                unfilteredPoints.RemoveAt(indexToRemove);
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
                    if (nonSequentialPoints[i].IsNumericallyEqualTo(nonSequentialPoints[j]))
                    {
                        indexesToRemove.Add(j);
                    }
                }
            }

            List<XYZ> filteredNonSequentialPoints = nonSequentialPoints.RemovePointsAtIndexes(indexesToRemove);

            return filteredNonSequentialPoints;
        }
        public static List<XYZ> PruneNonSequentialDuplicatedPoints(List<XYZ> nonSequentialPoints, double tolerance)
        {
            List<int> indexesToRemove = new List<int>();

            for (int i = 0; i < nonSequentialPoints.Count(); i++)
            {
                for (int j = i + 1; j < nonSequentialPoints.Count(); j++)
                {
                    if (nonSequentialPoints[i].IsNumericallyEqualTo(nonSequentialPoints[j], tolerance))
                    {
                        indexesToRemove.Add(j);
                    }
                }
            }

            List<XYZ> filteredNonSequentialPoints = nonSequentialPoints.RemovePointsAtIndexes(indexesToRemove);

            return filteredNonSequentialPoints;
        }
        public static List<XYZ> PruneSequentialDuplicatedPoints(List<XYZ> sequentialPoints)
        {
            List<int> indexesToRemove = new List<int>();

            for (int i = 0; i < (sequentialPoints.Count - 1); i++)
            {
                if (sequentialPoints[i].IsNumericallyEqualTo(sequentialPoints[i + 1]))
                {
                    indexesToRemove.Add(i);
                }
            }

            List<XYZ> filteredSequentialPoints = sequentialPoints.RemovePointsAtIndexes(indexesToRemove);

            return filteredSequentialPoints;
        }
        public static List<XYZ> PruneSequentialDuplicatedPoints(List<XYZ> sequentialPoints, double tolerance)
        {
            List<int> indexesToRemove = new List<int>();

            for (int i = 0; i < (sequentialPoints.Count - 1); i++)
            {
                if (sequentialPoints[i].IsNumericallyEqualTo(sequentialPoints[i + 1], tolerance))
                {
                    indexesToRemove.Add(i);
                }
            }

            List<XYZ> filteredSequentialPoints = sequentialPoints.RemovePointsAtIndexes(indexesToRemove);

            return filteredSequentialPoints;
        }
        public static List<XYZ> RoundMultiplePointCoordinates(List<XYZ> points, int digitsToRoundCoordinates)
        {
            List<XYZ> roundedPoints = points
                .Select(x => x.RoundCoordinates(digitsToRoundCoordinates))
                .ToList();

            return roundedPoints;
        }


        //Vector
        public static bool IsAlmostParallelTo(this XYZ firstVector, XYZ secondVector, double tolerance = 0.001)
        {
            return firstVector.IsAlmostEqualTo(secondVector, tolerance) || firstVector.IsAlmostEqualTo(secondVector.Negate(), tolerance) ? true : false;
        }




    }
}
