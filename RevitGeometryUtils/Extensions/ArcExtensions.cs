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

        //Arc
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
        public static Arc ProjectArcOnPlaneByPlanarFace(this Arc arc, PlanarFace planarFace)
        {
            XYZ originalStartPoint = arc.GetEndPoint(0);
            XYZ originalEndPoint = arc.GetEndPoint(1);
            XYZ originalMiddlePoint = arc.Evaluate(0.5, true);
            XYZ newStartPoint = originalStartPoint.ProjectOnSamePlaneAsFace(planarFace);
            XYZ newEndPoint = originalEndPoint.ProjectOnSamePlaneAsFace(planarFace);
            XYZ newMiddlePoint = originalMiddlePoint.ProjectOnSamePlaneAsFace(planarFace);
            Arc newArc = Arc.Create(newStartPoint, newEndPoint, newMiddlePoint);

            return newArc;
        }
        public static Arc ProjectArcOnPlaneByPlanarFace(Arc arc, PlanarFace planarFace, int digitsToRoundCoordinates)
        {
            XYZ originalStartPoint = arc.GetEndPoint(0);
            XYZ originalEndPoint = arc.GetEndPoint(1);
            XYZ originalMiddlePoint = arc.Evaluate(0.5, true);
            XYZ newStartPoint = originalStartPoint.ProjectOnSamePlaneAsFace(planarFace, digitsToRoundCoordinates);
            XYZ newEndPoint = originalEndPoint.ProjectOnSamePlaneAsFace(planarFace, digitsToRoundCoordinates);
            XYZ newMiddlePoint = originalMiddlePoint.ProjectOnSamePlaneAsFace(planarFace, digitsToRoundCoordinates);
            Arc newArc = Arc.Create(newStartPoint, newEndPoint, newMiddlePoint);

            return newArc;
        }
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
