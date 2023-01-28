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
        /// <summary>
        /// Projects this ellipse onto a global plane.
        /// </summary>
        /// <param name="ellipse"></param>
        /// <param name="globalPlane"></param>
        /// <returns>
        /// The ellipse projection on the global plane.
        /// </returns>
        public static Ellipse ProjectOnGlobalPlane(this Ellipse ellipse, PlaneExtensions.GlobalPlane globalPlane)
        {
            XYZ originalCenter = ellipse.Center;
            XYZ originalXDirection = ellipse.XDirection;
            XYZ originalYDirection = ellipse.YDirection;

            XYZ projectedCenter = originalCenter.ProjectOnGlobalPlane(globalPlane);
            XYZ projectedXDirection = originalXDirection.ProjectOnGlobalPlane(globalPlane);
            XYZ projectedYDirection = originalYDirection.ProjectOnGlobalPlane(globalPlane);
            double projectedXRadius = projectedXDirection.DistanceTo(projectedCenter);
            double projectedYRadius = projectedYDirection.DistanceTo(projectedCenter);
            double normalizedStartParameter = ellipse.ComputeNormalizedParameter(ellipse.GetEndParameter(0));
            double normalizedEndParameter = ellipse.ComputeNormalizedParameter(ellipse.GetEndParameter(1));
            
            try
            {
                Curve projectedEllipse = Ellipse.CreateCurve(projectedCenter, projectedXRadius, projectedYRadius, projectedXDirection, projectedYDirection, normalizedStartParameter, normalizedEndParameter);
                return projectedEllipse as Ellipse;
            }
            catch (Exception)
            {
                throw new NotImplementedException("The projection is a line or a curve too small to be created. This case is not yet implemented.");
            }
        }

        /// <summary>
        /// Projects this arc onto a global plane and rounds its center, x-direction and y-direction values.
        /// </summary>
        /// <param name="ellipse"></param>
        /// <param name="globalPlane"></param>
        /// <param name="digitsToRoundCoordinates"></param>
        /// <returns>
        /// The arc projection on the global plane with rounded center, x-direction and y-direction values.
        /// </returns>
        public static Ellipse ProjectOnGlobalPlane(this Ellipse ellipse, PlaneExtensions.GlobalPlane globalPlane, int digitsToRoundCoordinates)
        {
            XYZ originalCenter = ellipse.Center;
            XYZ originalXDirection = ellipse.XDirection;
            XYZ originalYDirection = ellipse.YDirection;

            XYZ projectedCenter = originalCenter.ProjectOnGlobalPlane(globalPlane, digitsToRoundCoordinates);
            XYZ projectedXDirection = originalXDirection.ProjectOnGlobalPlane(globalPlane, digitsToRoundCoordinates);
            XYZ projectedYDirection = originalYDirection.ProjectOnGlobalPlane(globalPlane, digitsToRoundCoordinates);
            double projectedXRadius = projectedXDirection.DistanceTo(projectedCenter);
            double projectedYRadius = projectedYDirection.DistanceTo(projectedCenter);
            double normalizedStartParameter = ellipse.ComputeNormalizedParameter(ellipse.GetEndParameter(0));
            double normalizedEndParameter = ellipse.ComputeNormalizedParameter(ellipse.GetEndParameter(1));
            
            try
            {
                Curve projectedEllipse = Ellipse.CreateCurve(projectedCenter, projectedXRadius, projectedYRadius, projectedXDirection, projectedYDirection, normalizedStartParameter, normalizedEndParameter);
                return projectedEllipse as Ellipse;
            }
            catch (Exception)
            {
                throw new NotImplementedException("The projection is a line or a curve too small to be created. This case is not yet implemented.");
            }
        }

        /*
        NÃO IMPLEMENTADAS
        public static Ellipse ProjectArcOnSamePlaneAsPlanarFace(Ellipse ellipse, PlanarFace planarFace)
        {
            throw new NotImplementedException();
        }
        public static Ellipse ProjectArcOnSamePlaneAsPlanarFace(Ellipse ellipse, PlanarFace planarFace, int digitsToRoundCoordinates)
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
