using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevitGeometryUtils.Extensions
{
    public static  class PlaneExtensions
    {
        //Plane
        public enum GlobalPlane
        {
            XYPlane,
            XZPlane,
            YZPlane
        }

        /// <summary>
        /// Get the normal vector of a given global plane
        /// </summary>
        /// <param name="globalPlane"></param>
        /// <returns>
        /// The XYZ normal vector of the global plane.
        /// </returns>
        public static XYZ GetGlobalPlaneNormal(GlobalPlane globalPlane)
        {
            switch (globalPlane)
            {
                case GlobalPlane.XYPlane:
                    return XYZ.BasisZ;

                case GlobalPlane.YZPlane:
                    return XYZ.BasisX;

                case GlobalPlane.XZPlane:
                    return XYZ.BasisZ;
            }

            return null;
        }
    }
}
