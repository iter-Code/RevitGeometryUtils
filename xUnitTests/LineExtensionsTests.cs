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
        public void SingleLineTests()
        {
            var testModel = Utils.GetTestModel("GeometryProjectionTests.rvt");
            var doc = xru.OpenDoc(testModel);

            Line singleLine45Degrees = CurveExtensionsTests.GetCurveFromModelLineId(302898, doc) as Line;
            Line singleLineminus45Degrees = CurveExtensionsTests.GetCurveFromModelLineId(302908, doc) as Line;
            Line singleLine0Degrees = CurveExtensionsTests.GetCurveFromModelLineId(302251, doc) as Line;

            Assert.True(singleLine45Degrees.ProjectOnGlobalPlane(PlaneExtensions.GlobalPlane.XYPlane).IsGeometricallyAlmostEqualTo(singleLine0Degrees, true));
            Assert.True(singleLineminus45Degrees.ProjectOnGlobalPlane(PlaneExtensions.GlobalPlane.XYPlane).IsGeometricallyAlmostEqualTo(singleLine0Degrees, true));
            
            //doc.Close(false);

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

        [Fact]
        public void RectangleTests()
        {
            var testModel = Utils.GetTestModel("GeometryProjectionTests.rvt");
            var doc = xru.OpenDoc(testModel);

            // Rectangle inclined 45 degrees
            Line leftLine45Degrees = CurveExtensionsTests.GetCurveFromModelLineId(302899, doc) as Line;
            Line upperLine45Degrees = CurveExtensionsTests.GetCurveFromModelLineId(302901, doc) as Line;
            Line rightLine45Degrees = CurveExtensionsTests.GetCurveFromModelLineId(302902, doc) as Line;
            Line lowerLine45Degrees = CurveExtensionsTests.GetCurveFromModelLineId(302900, doc) as Line;

            // Rectangle inclined -45 degrees
            Line leftLineMinus45Degrees = CurveExtensionsTests.GetCurveFromModelLineId(302912, doc) as Line;
            Line upperLineMinus45Degrees = CurveExtensionsTests.GetCurveFromModelLineId(302911, doc) as Line;
            Line rightLineMinus45Degrees = CurveExtensionsTests.GetCurveFromModelLineId(302910, doc) as Line;
            Line lowerLineMinus45Degrees = CurveExtensionsTests.GetCurveFromModelLineId(302909, doc) as Line;

            // Rectangle on XY plane
            Line leftLine0Degrees = CurveExtensionsTests.GetCurveFromModelLineId(302362, doc) as Line;
            Line upperLine0Degrees = CurveExtensionsTests.GetCurveFromModelLineId(302367, doc) as Line;
            Line rightLine0Degrees = CurveExtensionsTests.GetCurveFromModelLineId(302372, doc) as Line;
            Line lowerLine0Degrees = CurveExtensionsTests.GetCurveFromModelLineId(302377, doc) as Line;

            // Test lines of the rectangle inclined 45 degrees
            Assert.True(leftLine45Degrees.ProjectOnGlobalPlane(PlaneExtensions.GlobalPlane.XYPlane).IsGeometricallyAlmostEqualTo(leftLine0Degrees, true));
            Assert.True(upperLine45Degrees.ProjectOnGlobalPlane(PlaneExtensions.GlobalPlane.XYPlane).IsGeometricallyAlmostEqualTo(upperLine0Degrees, true));
            Assert.True(rightLine45Degrees.ProjectOnGlobalPlane(PlaneExtensions.GlobalPlane.XYPlane).IsGeometricallyAlmostEqualTo(rightLine0Degrees, true));
            Assert.True(lowerLine45Degrees.ProjectOnGlobalPlane(PlaneExtensions.GlobalPlane.XYPlane).IsGeometricallyAlmostEqualTo(lowerLine0Degrees, true));


            // Test lines of the rectangle inclined -45 degrees
            Assert.True(leftLineMinus45Degrees.ProjectOnGlobalPlane(PlaneExtensions.GlobalPlane.XYPlane).IsGeometricallyAlmostEqualTo(leftLine0Degrees, true));
            Assert.True(upperLineMinus45Degrees.ProjectOnGlobalPlane(PlaneExtensions.GlobalPlane.XYPlane).IsGeometricallyAlmostEqualTo(upperLine0Degrees, true));
            Assert.True(rightLineMinus45Degrees.ProjectOnGlobalPlane(PlaneExtensions.GlobalPlane.XYPlane).IsGeometricallyAlmostEqualTo(rightLine0Degrees, true));
            Assert.True(lowerLineMinus45Degrees.ProjectOnGlobalPlane(PlaneExtensions.GlobalPlane.XYPlane).IsGeometricallyAlmostEqualTo(lowerLine0Degrees, true));
        }

        [Fact]
        public void HexagonTests()
        {
            var testModel = Utils.GetTestModel("GeometryProjectionTests.rvt");
            var doc = xru.OpenDoc(testModel);

            // Hexagon inclined 45 degrees
            Line leftCentralLine45Degrees = CurveExtensionsTests.GetCurveFromModelLineId(302945, doc) as Line;
            Line leftUpperLine45Degrees = CurveExtensionsTests.GetCurveFromModelLineId(302904, doc) as Line;
            Line leftLowerLine45Degrees = CurveExtensionsTests.GetCurveFromModelLineId(302903, doc) as Line;
            Line rightCentralLine45Degrees = CurveExtensionsTests.GetCurveFromModelLineId(302946, doc) as Line;
            Line rightUpperLine45Degrees = CurveExtensionsTests.GetCurveFromModelLineId(302905, doc) as Line;
            Line rightLowerLine45Degrees = CurveExtensionsTests.GetCurveFromModelLineId(302906, doc) as Line;

            // Hexagon inclined -45 degrees
            Line leftCentralLineMinus45Degrees = CurveExtensionsTests.GetCurveFromModelLineId(302915, doc) as Line;
            Line leftUpperLineMinus45Degrees = CurveExtensionsTests.GetCurveFromModelLineId(302916, doc) as Line;
            Line leftLowerLineMinus45Degrees = CurveExtensionsTests.GetCurveFromModelLineId(302914, doc) as Line;
            Line rightCentralLineMinus45Degrees = CurveExtensionsTests.GetCurveFromModelLineId(302918, doc) as Line;
            Line rightUpperLineMinus45Degrees = CurveExtensionsTests.GetCurveFromModelLineId(302917, doc) as Line;
            Line rightLowerLineMinus45Degrees = CurveExtensionsTests.GetCurveFromModelLineId(302913, doc) as Line;

            // Hexagon on XY plane
            Line leftCentralLine0Degrees = CurveExtensionsTests.GetCurveFromModelLineId(302444, doc) as Line;
            Line leftUpperLine0Degrees = CurveExtensionsTests.GetCurveFromModelLineId(302449, doc) as Line;
            Line leftLowerLine0Degrees = CurveExtensionsTests.GetCurveFromModelLineId(302439, doc) as Line;
            Line rightCentralLine0Degrees = CurveExtensionsTests.GetCurveFromModelLineId(302459, doc) as Line;
            Line rightUpperLine0Degrees = CurveExtensionsTests.GetCurveFromModelLineId(302454, doc) as Line;
            Line rightLowerLine0Degrees = CurveExtensionsTests.GetCurveFromModelLineId(302434, doc) as Line;

            // Test lines of the hexagon inclined 45 degrees
            Assert.True(leftCentralLine45Degrees.ProjectOnGlobalPlane(PlaneExtensions.GlobalPlane.XYPlane).IsGeometricallyAlmostEqualTo(leftCentralLine0Degrees, true));
            Assert.True(leftUpperLine45Degrees.ProjectOnGlobalPlane(PlaneExtensions.GlobalPlane.XYPlane).IsGeometricallyAlmostEqualTo(leftUpperLine0Degrees, true));
            Assert.True(leftLowerLine45Degrees.ProjectOnGlobalPlane(PlaneExtensions.GlobalPlane.XYPlane).IsGeometricallyAlmostEqualTo(leftLowerLine0Degrees, true));
            Assert.True(rightCentralLine45Degrees.ProjectOnGlobalPlane(PlaneExtensions.GlobalPlane.XYPlane).IsGeometricallyAlmostEqualTo(rightCentralLine0Degrees, true));
            Assert.True(rightUpperLine45Degrees.ProjectOnGlobalPlane(PlaneExtensions.GlobalPlane.XYPlane).IsGeometricallyAlmostEqualTo(rightUpperLine0Degrees, true));
            Assert.True(rightLowerLine45Degrees.ProjectOnGlobalPlane(PlaneExtensions.GlobalPlane.XYPlane).IsGeometricallyAlmostEqualTo(rightLowerLine0Degrees, true));

            // Test lines of the hexagon inclined -45 degrees
            Assert.True(leftCentralLineMinus45Degrees.ProjectOnGlobalPlane(PlaneExtensions.GlobalPlane.XYPlane).IsGeometricallyAlmostEqualTo(leftCentralLine0Degrees, true));
            Assert.True(leftUpperLineMinus45Degrees.ProjectOnGlobalPlane(PlaneExtensions.GlobalPlane.XYPlane).IsGeometricallyAlmostEqualTo(leftUpperLine0Degrees, true));
            Assert.True(leftLowerLineMinus45Degrees.ProjectOnGlobalPlane(PlaneExtensions.GlobalPlane.XYPlane).IsGeometricallyAlmostEqualTo(leftLowerLine0Degrees, true));
            Assert.True(rightCentralLineMinus45Degrees.ProjectOnGlobalPlane(PlaneExtensions.GlobalPlane.XYPlane).IsGeometricallyAlmostEqualTo(rightCentralLine0Degrees, true));
            Assert.True(rightUpperLineMinus45Degrees.ProjectOnGlobalPlane(PlaneExtensions.GlobalPlane.XYPlane).IsGeometricallyAlmostEqualTo(rightUpperLine0Degrees, true));
            Assert.True(rightLowerLineMinus45Degrees.ProjectOnGlobalPlane(PlaneExtensions.GlobalPlane.XYPlane).IsGeometricallyAlmostEqualTo(rightLowerLine0Degrees, true));
        }

    }
    }
