using Autodesk.Revit.DB;
using RevitGeometryUtils.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevitGeometryUtils.NewClasses
{
    public class OpenSequentialCurves : CurveLoopCurves
    {
        public OpenSequentialCurves(CurveLoop curveLoop) : base(curveLoop)
        {
        }
    }
}
