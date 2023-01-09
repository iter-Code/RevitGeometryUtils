using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using xUnitRevitUtils;
using RevitGeometryUtils;
using RevitGeometryUtils.Extensions;

namespace xUnitTests
{
    public class LineExtensionsTests
    {

        [Fact]
        public void LineTests()
        {
            var testModel = Utils.GetTestModel("GeometryProjectionTests.rvt");
            var doc = xru.OpenDoc(testModel);
            
            Line singleLine45Degrees = CurveExtensionsTests.GetCurveFromModelLineId(300773, doc) as Line;
            Line singleLineminus45Degrees = CurveExtensionsTests.GetCurveFromModelLineId(300848, doc) as Line;
            Line singleLine0Degrees = CurveExtensionsTests.GetCurveFromModelLineId(302251, doc) as Line;
            
            Assert.True(singleLine45Degrees.ProjectOnGlobalPlane(PlaneExtensions.GlobalPlane.XYPlane).Equals(singleLine0Degrees));
            Assert.True(singleLineminus45Degrees.ProjectOnGlobalPlane(PlaneExtensions.GlobalPlane.XYPlane).Equals(singleLine0Degrees));
            
            doc.Close(false);

            /*
            xru.Run(() =>
            {
                using (Transaction transaction = new Transaction(doc, "Temporary - only to get gross area"))
                {
                    transaction.Start();

                    XYZ newPoint = new XYZ(37.7019109770625, 11.2429266158782, 15.3215437475464);

                    //XYZ round0Digits = iCGeometry.RoundPointCoordinates(newPoint, 0);
                    //Assert.True(round0Digits.Equals(new XYZ(38, 11, 15)));
                    //Assert.Equal(round0Digits.X, new XYZ(38, 11, 15).X);
                    //Assert.Equal(round0Digits.Y, new XYZ(38, 11, 15).Y);
                    //Assert.Equal(round0Digits.Z, new XYZ(38, 11, 15).Z);
                    /*
                    XYZ round0Digits = iCGeometry.RoundPointCoordinates(newPoint, 0);
                    XYZ round1Digits = iCGeometry.RoundPointCoordinates(newPoint, 1);
                    XYZ round3Digits = iCGeometry.RoundPointCoordinates(newPoint, 3);
                    XYZ round4Digits = iCGeometry.RoundPointCoordinates(newPoint, 4);
                    XYZ round2Digits = iCGeometry.RoundPointCoordinates(newPoint, 2);
                    XYZ round5Digits = iCGeometry.RoundPointCoordinates(newPoint, 5);
                    XYZ round6Digits = iCGeometry.RoundPointCoordinates(newPoint, 6);
                    XYZ round7Digits = iCGeometry.RoundPointCoordinates(newPoint, 7);
                    XYZ round8Digits = iCGeometry.RoundPointCoordinates(newPoint, 8);
                    XYZ round9Digits = iCGeometry.RoundPointCoordinates(newPoint, 9);
                    XYZ round10Digits = iCGeometry.RoundPointCoordinates(newPoint, 10);
                    XYZ round11Digits = iCGeometry.RoundPointCoordinates(newPoint, 11);
                    XYZ round12Digits = iCGeometry.RoundPointCoordinates(newPoint, 12);
                    XYZ round13Digits = iCGeometry.RoundPointCoordinates(newPoint, 13);

                    Assert.True(round0Digits.Equals(new XYZ(38, 11, 15)));
                    //Assert.True(round1Digits.Equals(new XYZ(37.7, 11.2, 15.3)));
                    //Assert.True(round2Digits.Equals(new XYZ(37.70, 11.24, 15.32)));
                    //Assert.True(round3Digits.Equals(new XYZ(37.701, 11.242, 15.321)));
                    //Assert.True(round4Digits.Equals(new XYZ(37.7019, 11.2429, 15.3215)));
                    //Assert.True(round5Digits.Equals(new XYZ(37.7019109770625, 11.2429266158782, 15.3215437475464)));
                    //Assert.True(round6Digits.Equals(new XYZ(37.7019109770625, 11.2429266158782, 15.3215437475464)));
                    //Assert.True(round7Digits.Equals(new XYZ(37.7019109770625, 11.2429266158782, 15.3215437475464)));
                    //Assert.True(round8Digits.Equals(new XYZ(37.7019109770625, 11.2429266158782, 15.3215437475464)));
                    //Assert.True(round9Digits.Equals(new XYZ(37.7019109770625, 11.2429266158782, 15.3215437475464)));
                    //Assert.True(round10Digits.Equals(new XYZ(37.7019109770625, 11.2429266158782, 15.3215437475464)));
                    //Assert.True(round11Digits.Equals(new XYZ(37.70191097706, 11.24292661588, 15.32154374755)));
                    //Assert.True(round12Digits.Equals(new XYZ(37.701910977063, 11.242926615878, 15.321543747546)));
                    //Assert.True(round13Digits.Equals(new XYZ(37.7019109770625, 11.2429266158782, 15.3215437475464)));
                    
                    transaction.RollBack();
                }
            }, doc).Wait();



            //using (Transaction transaction = new Transaction(doc, "Temporary - only to get gross area"))
            //{
            //    transaction.Start();

            //    XYZ newPoint = new XYZ(37.7019109770625, 11.2429266158782, 15.3215437475464);

            //    XYZ round0Digits = iCGeometry.RoundPointCoordinates(newPoint, 0);
            //    XYZ round1Digits = iCGeometry.RoundPointCoordinates(newPoint, 1);
            //    XYZ round3Digits = iCGeometry.RoundPointCoordinates(newPoint, 3);
            //    XYZ round4Digits = iCGeometry.RoundPointCoordinates(newPoint, 4);
            //    XYZ round2Digits = iCGeometry.RoundPointCoordinates(newPoint, 2);
            //    XYZ round5Digits = iCGeometry.RoundPointCoordinates(newPoint, 5);
            //    XYZ round6Digits = iCGeometry.RoundPointCoordinates(newPoint, 6);
            //    XYZ round7Digits = iCGeometry.RoundPointCoordinates(newPoint, 7);
            //    XYZ round8Digits = iCGeometry.RoundPointCoordinates(newPoint, 8);
            //    XYZ round9Digits = iCGeometry.RoundPointCoordinates(newPoint, 9);
            //    XYZ round10Digits = iCGeometry.RoundPointCoordinates(newPoint, 10);
            //    XYZ round11Digits = iCGeometry.RoundPointCoordinates(newPoint, 11);
            //    XYZ round12Digits = iCGeometry.RoundPointCoordinates(newPoint, 12);
            //    XYZ round13Digits = iCGeometry.RoundPointCoordinates(newPoint, 13);

            //    Assert.Equal(round0Digits, new XYZ(37, 11, 15));
            //    //Assert.Equal(round1Digits, new XYZ(37.7, 11.2, 15.3));
            //    //Assert.Equal(round0Digits, new XYZ(37.70, 11.24, 15.32));
            //    //    //Assert.Equal(round0Digits, new XYZ(37.7019109770625, 11.2429266158782, 15.3215437475464));
            //    //    //Assert.Equal(round0Digits, new XYZ(37.7019109770625, 11.2429266158782, 15.3215437475464));
            //    //    //Assert.Equal(round0Digits, new XYZ(37.7019109770625, 11.2429266158782, 15.3215437475464));
            //    //    //Assert.Equal(round0Digits, new XYZ(37.7019109770625, 11.2429266158782, 15.3215437475464));
            //    //    //Assert.Equal(round0Digits, new XYZ(37.7019109770625, 11.2429266158782, 15.3215437475464));
            //    //    //Assert.Equal(round0Digits, new XYZ(37.7019109770625, 11.2429266158782, 15.3215437475464));
            //    //    //Assert.Equal(round0Digits, new XYZ(37.7019109770625, 11.2429266158782, 15.3215437475464));
            //    //    //Assert.Equal(round0Digits, new XYZ(37.7019109770625, 11.2429266158782, 15.3215437475464));


            //    //    //ModelLine modelLine = 

            //    //    //double x = -37.7019109770625;
            //    //    //double y = -11.2429266158782;
            //    //    //double z = 0;

            //    transaction.RollBack();

            //}*/

        }
    }
}
