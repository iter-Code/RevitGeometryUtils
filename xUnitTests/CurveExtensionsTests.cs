using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using xUnitRevitUtils;

namespace xUnitTests
{
    public class CurveExtensionsTests
    {
        public static Curve GetCurveFromModelLineId(int modelLineIdAsInteger, Document document)
        {
            ElementId modelLineId = new ElementId(modelLineIdAsInteger);
            Element modelLine = document.GetElement(modelLineId);
            GeometryObject geometryObject = modelLine.get_Geometry(new Options());
            GeometryElement geometryElement = geometryObject as GeometryElement;
            Curve modelLineCurve = geometryElement.First() as Curve;

            return modelLineCurve;
        }

        public class CurveComparer : IEqualityComparer<Curve>
        {
            public bool Equals(Curve x, Curve y)
            {
                bool sameLength = x.Length == y.Length;
                bool sameStartPoint = x.GetEndPoint(0).IsAlmostEqualTo(y.GetEndPoint(0));
                bool sameEndPoint = x.GetEndPoint(1).IsAlmostEqualTo(y.GetEndPoint(1));

                return (sameLength && sameStartPoint && sameEndPoint) ? true : false;
            }

            public int GetHashCode(Curve obj)
            {
                return 0;
            }
        }


    }
}
