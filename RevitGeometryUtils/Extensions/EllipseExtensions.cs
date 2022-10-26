using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevitGeometryUtils.Extensions
{
    public static class EllipseExtensions
    {
        public enum GlobalPlane
        {
            XYPlane,
            XZPlane,
            YZPlane
        }

        //Ellipse
        public static Ellipse ProjectEllipseOnGlobalPlane(this Ellipse ellipse, GlobalPlane globalPlane)
        {
            XYZ originalCenter = ellipse.Center;
            XYZ originalXDirection = ellipse.XDirection;
            XYZ originalYDirection = ellipse.YDirection;

            XYZ newCenter = originalCenter.ProjectOnGlobalPlane(globalPlane);
            XYZ newXDirection = originalXDirection.ProjectOnGlobalPlane(globalPlane);
            XYZ newYDirection = originalYDirection.ProjectOnGlobalPlane(globalPlane);
            double newXRadius = newXDirection.DistanceTo(newCenter);
            double newYRadius = newYDirection.DistanceTo(newCenter);
            double normalizedStartParameter = ellipse.ComputeNormalizedParameter(ellipse.GetEndParameter(0));
            double normalizedEndParameter = ellipse.ComputeNormalizedParameter(ellipse.GetEndParameter(1));
            Curve newEllipse = Ellipse.CreateCurve(newCenter, newXRadius, newYRadius, newXDirection, newYDirection, normalizedStartParameter, normalizedEndParameter);

            return newEllipse as Ellipse;
        }
        public static Ellipse ProjectEllipseOnGlobalPlane(this Ellipse ellipse, GlobalPlane globalPlane, int digitsToRoundCoordinates)
        {
            XYZ originalCenter = ellipse.Center;
            XYZ originalXDirection = ellipse.XDirection;
            XYZ originalYDirection = ellipse.YDirection;

            XYZ newCenter = originalCenter.ProjectOnGlobalPlane(globalPlane, digitsToRoundCoordinates);
            XYZ newXDirection = originalXDirection.ProjectOnGlobalPlane(globalPlane, digitsToRoundCoordinates);
            XYZ newYDirection = originalYDirection.ProjectOnGlobalPlane(globalPlane, digitsToRoundCoordinates);
            double newXRadius = newXDirection.DistanceTo(newCenter);
            double newYRadius = newYDirection.DistanceTo(newCenter);
            double normalizedStartParameter = ellipse.ComputeNormalizedParameter(ellipse.GetEndParameter(0));
            double normalizedEndParameter = ellipse.ComputeNormalizedParameter(ellipse.GetEndParameter(1));
            Curve newEllipse = Ellipse.CreateCurve(newCenter, newXRadius, newYRadius, newXDirection, newYDirection, normalizedStartParameter, normalizedEndParameter);

            return newEllipse as Ellipse;
        }

        /*
        NÃO IMPLEMENTADAS
        public static Ellipse ProjectArcOnPlaneByPlanarFace(Ellipse ellipse, PlanarFace planarFace)
        {
            throw new NotImplementedException();
        }
        public static Ellipse ProjectArcOnPlaneByPlanarFace(Ellipse ellipse, PlanarFace planarFace, int digitsToRoundCoordinates)
        {
            throw new NotImplementedException();
        }
        public static Ellipse ProjectArcOnPlaneByOriginAndNormal(Ellipse ellipse, XYZ origin, XYZ normal)
        {
            throw new NotImplementedException();
        }
        public static Ellipse ProjectArcOnPlaneByOriginAndNormal(Ellipse ellipse, XYZ origin, XYZ normal, int digitsToRoundCoordinates)
        {
            throw new NotImplementedException();
        }
        */
    }
}
