using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevitGeometryUtils.Extensions
{
    public static class ArcExtensions
    {
        /// <summary>
        /// Projects this arc onto a global plane.
        /// </summary>
        /// <param name="arc"></param>
        /// <param name="globalPlane"></param>
        /// <returns>
        /// The arc projection on the global plane.
        /// </returns>
        public static Arc ProjectOnGlobalPlane(this Arc arc, PlaneExtensions.GlobalPlane globalPlane)
        {
            XYZ originalStartPoint = arc.GetEndPoint(0);
            XYZ originalEndPoint = arc.GetEndPoint(1);
            XYZ originalMiddlePoint = arc.Evaluate(0.5, true);
            XYZ projectedStartPoint = originalStartPoint.ProjectOnGlobalPlane(globalPlane);
            XYZ projectedEndPoint = originalEndPoint.ProjectOnGlobalPlane(globalPlane);
            XYZ projectedMiddlePoint = originalMiddlePoint.ProjectOnGlobalPlane(globalPlane);
            Arc projectedArc = Arc.Create(projectedStartPoint, projectedEndPoint, projectedMiddlePoint);

            return projectedArc;
        }

        /// <summary>
        /// Projects this arc onto a global plane and rounds its initial, middle and final coordinates.
        /// </summary>
        /// <param name="arc"></param>
        /// <param name="globalPlane"></param>
        /// <param name="digitsToRoundCoordinates"></param>
        /// <returns>
        /// The arc projection on the global plane with rounded initial, middle and final coordinates.
        /// </returns>
        public static Arc ProjectOnGlobalPlane(this Arc arc, PlaneExtensions.GlobalPlane globalPlane, int digitsToRoundCoordinates)
        {
            XYZ originalStartPoint = arc.GetEndPoint(0);
            XYZ originalEndPoint = arc.GetEndPoint(1);
            XYZ originalMiddlePoint = arc.Evaluate(0.5, true);
            XYZ projectedStartPoint = originalStartPoint.ProjectOnGlobalPlane(globalPlane, digitsToRoundCoordinates);
            XYZ projectedEndPoint = originalEndPoint.ProjectOnGlobalPlane(globalPlane, digitsToRoundCoordinates);
            XYZ projectedMiddlePoint = originalMiddlePoint.ProjectOnGlobalPlane(globalPlane, digitsToRoundCoordinates);
            Arc projectedArc = Arc.Create(projectedStartPoint, projectedEndPoint, projectedMiddlePoint);

            return projectedArc;
        }

        /// <summary>
        /// Projects this arc onto the same plane as a given PlanarFace.
        /// </summary>
        /// <param name="arc"></param>
        /// <param name="planarFace"></param>
        /// <returns>
        /// The arc projection on the same plane as the PlanarFace.
        /// </returns>
        public static Arc ProjectArcOnSamePlaneAsPlanarFace(this Arc arc, PlanarFace planarFace)
        {
            XYZ originalStartPoint = arc.GetEndPoint(0);
            XYZ originalEndPoint = arc.GetEndPoint(1);
            XYZ originalMiddlePoint = arc.Evaluate(0.5, true);
            XYZ projectedStartPoint = originalStartPoint.ProjectOnSamePlaneAsPlanarFace(planarFace);
            XYZ projectedEndPoint = originalEndPoint.ProjectOnSamePlaneAsPlanarFace(planarFace);
            XYZ projectedMiddlePoint = originalMiddlePoint.ProjectOnSamePlaneAsPlanarFace(planarFace);
            Arc projectedArc = Arc.Create(projectedStartPoint, projectedEndPoint, projectedMiddlePoint);

            return projectedArc;
        }

        /// <summary>
        /// Projects this point to the same plane as a given PlanarFace and round its initial, middle and final coordinates.
        /// </summary>
        /// <param name="arc"></param>
        /// <param name="planarFace"></param>
        /// <param name="digitsToRoundCoordinates"></param>
        /// <returns>
        /// The rounded arc projection on the same plane as the PlanarFace.
        /// </returns>
        public static Arc ProjectArcOnSamePlaneAsPlanarFace(this Arc arc, PlanarFace planarFace, int digitsToRoundCoordinates)
        {
            XYZ originalStartPoint = arc.GetEndPoint(0);
            XYZ originalEndPoint = arc.GetEndPoint(1);
            XYZ originalMiddlePoint = arc.Evaluate(0.5, true);
            XYZ projectedStartPoint = originalStartPoint.ProjectOnSamePlaneAsPlanarFace(planarFace, digitsToRoundCoordinates);
            XYZ projectedEndPoint = originalEndPoint.ProjectOnSamePlaneAsPlanarFace(planarFace, digitsToRoundCoordinates);
            XYZ projectedMiddlePoint = originalMiddlePoint.ProjectOnSamePlaneAsPlanarFace(planarFace, digitsToRoundCoordinates);
            Arc projectedArc = Arc.Create(projectedStartPoint, projectedEndPoint, projectedMiddlePoint);

            return projectedArc;
        }

        /// <summary>
        /// Projects this arc onto a plane given the plane origin and normal.
        /// </summary>
        /// <param name="arc"></param>
        /// <param name="normal"></param>
        /// <param name="origin"></param>
        /// <returns>
        /// The arc projection on the plane generated by the origin XYZ and the normal vector.
        /// </returns>
        public static Arc ProjectArcOnPlaneByOriginAndNormal(Arc arc, XYZ origin, XYZ normal)
        {
            XYZ originalStartPoint = arc.GetEndPoint(0);
            XYZ originalEndPoint = arc.GetEndPoint(1);
            XYZ originalMiddlePoint = arc.Evaluate(0.5, true);
            XYZ projectedStartPoint = originalStartPoint.ProjectOnPlaneByPlaneOriginAndNormal(origin, normal);
            XYZ projectedEndPoint = originalEndPoint.ProjectOnPlaneByPlaneOriginAndNormal(origin, normal);
            XYZ projectedMiddlePoint = originalMiddlePoint.ProjectOnPlaneByPlaneOriginAndNormal(origin, normal);
            Arc projectedArc = Arc.Create(projectedStartPoint, projectedEndPoint, projectedMiddlePoint);

            return projectedArc;
        }

        /// <summary>
        /// Projects this arc onto a plane given the plane origin and normal and rounds the projection initial, middle and final coordinates.
        /// </summary>
        /// <param name="arc"></param>
        /// <param name="normal"></param>
        /// <param name="origin"></param>
        /// <param name="digitsToRoundCoordinates"></param>
        /// <returns>
        /// The rounded arc projection on the plane generated by the origin XYZ and the normal vector.
        /// </returns>
        public static Arc ProjectArcOnPlaneByOriginAndNormal(Arc arc, XYZ origin, XYZ normal, int digitsToRoundCoordinates)
        {
            XYZ originalStartPoint = arc.GetEndPoint(0);
            XYZ originalEndPoint = arc.GetEndPoint(1);
            XYZ originalMiddlePoint = arc.Evaluate(0.5, true);
            XYZ projectedStartPoint = originalStartPoint.ProjectOnPlaneByPlaneOriginAndNormal(origin, normal, digitsToRoundCoordinates);
            XYZ projectedEndPoint = originalEndPoint.ProjectOnPlaneByPlaneOriginAndNormal(origin, normal, digitsToRoundCoordinates);
            XYZ projectedMiddlePoint = originalMiddlePoint.ProjectOnPlaneByPlaneOriginAndNormal(origin, normal, digitsToRoundCoordinates);
            Arc projectedArc = Arc.Create(projectedStartPoint, projectedEndPoint, projectedMiddlePoint);

            return projectedArc;
        }
    }
}
