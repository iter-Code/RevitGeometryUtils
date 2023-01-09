using Autodesk.Revit.DB;
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
            Curve modelLineCurve = geometryObject as Curve;

            return modelLineCurve;
        }
    }
}
