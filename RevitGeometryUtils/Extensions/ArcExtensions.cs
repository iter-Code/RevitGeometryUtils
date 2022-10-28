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

        /// <summary>
        /// Projects this arc onto a global plane.
        /// </summary>
        /// <param name="arc"></param>
        /// <param name="globalPlane"></param>
        /// <returns>
        /// The arc projection on the global plane.
        /// </returns>
        public static Arc ProjectOnGlobalPlane(this Arc arc, GlobalPlane globalPlane)
        {
            XYZ originalStartPoint = arc.GetEndPoint(0);
            XYZ originalEndPoint = arc.GetEndPoint(1);
            XYZ originalMiddlePoint = arc.Evaluate(0.5, true);
            XYZ newStartPoint = originalStartPoint.ProjectOnGlobalPlane(globalPlane);
            XYZ newEndPoint = originalEndPoint.ProjectOnGlobalPlane(globalPlane);
            XYZ newMiddlePoint = originalMiddlePoint.ProjectOnGlobalPlane(globalPlane);
            Arc newArc = Arc.Create(newStartPoint, newEndPoint, newMiddlePoint);

            return newArc;
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
        public static Arc ProjectOnGlobalPlane(this Arc arc, GlobalPlane globalPlane, int digitsToRoundCoordinates)
        {
            XYZ originalStartPoint = arc.GetEndPoint(0);
            XYZ originalEndPoint = arc.GetEndPoint(1);
            XYZ originalMiddlePoint = arc.Evaluate(0.5, true);
            XYZ newStartPoint = originalStartPoint.ProjectOnGlobalPlane(globalPlane, digitsToRoundCoordinates);
            XYZ newEndPoint = originalEndPoint.ProjectOnGlobalPlane(globalPlane, digitsToRoundCoordinates);
            XYZ newMiddlePoint = originalMiddlePoint.ProjectOnGlobalPlane(globalPlane, digitsToRoundCoordinates);
            Arc newArc = Arc.Create(newStartPoint, newEndPoint, newMiddlePoint);

            return newArc;
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
            XYZ newStartPoint = originalStartPoint.ProjectOnSamePlaneAsPlanarFace(planarFace);
            XYZ newEndPoint = originalEndPoint.ProjectOnSamePlaneAsPlanarFace(planarFace);
            XYZ newMiddlePoint = originalMiddlePoint.ProjectOnSamePlaneAsPlanarFace(planarFace);
            Arc newArc = Arc.Create(newStartPoint, newEndPoint, newMiddlePoint);

            return newArc;
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
            XYZ newStartPoint = originalStartPoint.ProjectOnSamePlaneAsPlanarFace(planarFace, digitsToRoundCoordinates);
            XYZ newEndPoint = originalEndPoint.ProjectOnSamePlaneAsPlanarFace(planarFace, digitsToRoundCoordinates);
            XYZ newMiddlePoint = originalMiddlePoint.ProjectOnSamePlaneAsPlanarFace(planarFace, digitsToRoundCoordinates);
            Arc newArc = Arc.Create(newStartPoint, newEndPoint, newMiddlePoint);

            return newArc;
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
            XYZ newStartPoint = originalStartPoint.ProjectOnPlaneByPlaneOriginAndNormal(origin, normal);
            XYZ newEndPoint = originalEndPoint.ProjectOnPlaneByPlaneOriginAndNormal(origin, normal);
            XYZ newMiddlePoint = originalMiddlePoint.ProjectOnPlaneByPlaneOriginAndNormal(origin, normal);
            Arc newArc = Arc.Create(newStartPoint, newEndPoint, newMiddlePoint);

            return newArc;
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
            XYZ newStartPoint = originalStartPoint.ProjectOnPlaneByPlaneOriginAndNormal(origin, normal, digitsToRoundCoordinates);
            XYZ newEndPoint = originalEndPoint.ProjectOnPlaneByPlaneOriginAndNormal(origin, normal, digitsToRoundCoordinates);
            XYZ newMiddlePoint = originalMiddlePoint.ProjectOnPlaneByPlaneOriginAndNormal(origin, normal, digitsToRoundCoordinates);
            Arc newArc = Arc.Create(newStartPoint, newEndPoint, newMiddlePoint);

            return newArc;
        }
    }
}
