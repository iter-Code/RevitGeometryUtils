    using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static RevitGeometryUtils.Extensions.PlaneExtensions;

namespace RevitGeometryUtils.Extensions
{
    public static class SolidExtensions
    {
        //Solid

        //TODO: Tratar quando não for PlanarFace
        public static List<PlanarFace> GetPlanarFacesAsList(this Solid solid)
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
        public static Solid TranslateByVector(this Solid solid, XYZ vector)
        {
            Transform objectTranslation = Transform.CreateTranslation(vector);
            Solid translatedSolid = SolidUtils.CreateTransformed(solid, objectTranslation);

            return translatedSolid;
        }
        public static XYZ GetVectorBetweenOriginAndCentroid(this Solid solid)
        {
            XYZ solidCentroid = solid.ComputeCentroid();
            XYZ origin = XYZ.Zero;
            XYZ centroidToOriginVector = origin - solidCentroid;

            return centroidToOriginVector;
        }
        public static List<XYZ> GetSolidOutwardFaceDirections(this Solid solid)
        {
            List<XYZ> outwardFaceDirections = new List<XYZ>();
            List<PlanarFace> faces = solid.GetPlanarFacesAsList();

            foreach (PlanarFace face in faces)
            {
                XYZ outwardFaceDirection = GetSolidOutwardFaceDirection(solid, face);
                outwardFaceDirections.Add(outwardFaceDirection);
            }

            return outwardFaceDirections;
        }
        public static XYZ GetSolidOutwardFaceDirection(this Solid solid, PlanarFace face)
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
        private static bool IsPointInsideSolid(Solid solid, XYZ point)
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
        public static Solid ScaleByValue(this Solid solid, double scale)
        {
            XYZ centroidToOriginVector = solid.GetVectorBetweenOriginAndCentroid();
            Solid translatedSolid = solid.TranslateByVector(centroidToOriginVector);
            Transform scaleTransform = Transform.CreateTranslation(XYZ.Zero).ScaleBasis(scale);
            Solid scaledSolidAtOrigin = SolidUtils.CreateTransformed(translatedSolid, scaleTransform);
            Transform translationToOriginalPlace = Transform.CreateTranslation(centroidToOriginVector.Negate());
            Solid scaledSolid = SolidUtils.CreateTransformed(scaledSolidAtOrigin, translationToOriginalPlace);

            return scaledSolid;
        }
        public static List<PlanarFace> GetZNormalFaces(this Solid solid)
        {
            List<PlanarFace> faceList = solid.GetPlanarFacesAsList();
            List<PlanarFace> zNormalFaces = faceList
                .Where(x => x.IsAlmostParallelToGlobalPlane(GlobalPlane.XYPlane))
                .ToList();

            return zNormalFaces;
        }





        /*
        public static Solid GetSolidFromGeometry(this Solid solid1, GeometryElement geometry)
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
        public static List<PlanarFace> PickFacesParallelToGlobalPlaneFromSolid(Solid solid, GlobalPlane globalPlane)
        {
            List<PlanarFace> faceList = TransformFaceArrayIntoList(solid);
            List<PlanarFace> zNormalFaces = GetFacesParallelToGlobalPlane(faceList, globalPlane);

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
                .Where(x => Math.Round(x.FaceNormal.Z, 2) != 0)
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
        }*/
    }
}
