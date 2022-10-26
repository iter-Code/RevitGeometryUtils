using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevitGeometryUtils.Extensions
{
    public static class PlanarFaceExtensions
    {
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
    }
}
