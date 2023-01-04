using Autodesk.Revit.DB;
//using Autodesk.Revit.DB.IFC;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using RevitGeometryUtils;
using Xunit;
using xUnitRevitUtils;

namespace xUnitTests
{

    public static class Utils
    {
        /// <summary>
        /// Utility method to get models from local folder rather than an absolute path
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static string GetTestModel(string filename)
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "TestModels", filename);
            return path;

        }
    }
    

    public class GeometryTests
    {
        //public static bool CompareExactPointCoordinates(XYZ firstPoint, XYZ secondPoint)
        //{
        //    bool xEqual = firstPoint.X == secondPoint.X;
        //    bool yEqual = firstPoint.Y == secondPoint.Y;
        //    bool zEqual = firstPoint.Z == secondPoint.Z;
        //    return xEqual && yEqual && zEqual;
        //}

        [Fact]
        public void RoundPointCoordinatesTests()
        {
            var testModel = Utils.GetTestModel("walls.rvt");
            var doc = xru.OpenDoc(testModel);

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
                    */
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

            //}
            
        }


        ///// <summary>
        ///// Checks whether all walls in the model have a valid volume
        ///// </summary>
        //[Fact]
        //public void WallsHaveVolume()
        //{
        //    var testModel = Utils.GetTestModel("walls.rvt");
        //    var doc = xru.OpenDoc(testModel);

        //    var walls = new FilteredElementCollector(doc).WhereElementIsNotElementType().OfCategory(BuiltInCategory.OST_Walls).ToElements();

        //    foreach (var wall in walls)
        //    {
        //        var volumeParam = wall.get_Parameter(BuiltInParameter.HOST_VOLUME_COMPUTED);
        //        Assert.NotNull(volumeParam);
        //        Assert.True(volumeParam.AsDouble() > 0);
        //    }
        //}

        //    [Fact]
        //    public void SampleFail()
        //    {
        //        var feet = UnitUtils.ConvertToInternalUnits(3000, DisplayUnitType.DUT_MILLIMETERS);
        //        Assert.Equal(5, feet);
        //    }

        //    //[Fact]
        //    //public void GetWallGrossAreaAndRollBack()
        //    //{
        //    //    var testModel = Utils.GetTestModel("walls.rvt");
        //    //    var doc = xru.OpenDoc(testModel);
        //    //    var walls = new FilteredElementCollector(doc).WhereElementIsNotElementType().OfCategory(BuiltInCategory.OST_Walls).ToElements();
        //    //    var wall = walls[0] as Wall;
        //    //    double grossArea = 0;

        //    //    var inserts = wall.FindInserts(true, true, true, true);
        //    //    xru.Run(() =>
        //    //    {
        //    //        using (Transaction transaction = new Transaction(doc, "Temporary - only to get gross area"))
        //    //        {
        //    //            transaction.Start();
        //    //            foreach (ElementId insertId in inserts) { doc.Delete(insertId); }
        //    //            doc.Regenerate();
        //    //            var wallFaceReference = HostObjectUtils.GetSideFaces(wall, ShellLayerType.Exterior);
        //    //            var face = doc.GetElement(wallFaceReference.First()).GetGeometryObjectFromReference(wallFaceReference.First()) as PlanarFace;
        //    //            var wallFaceEdges = face.GetEdgesAsCurveLoops();
        //    //            grossArea = ExporterIFCUtils.ComputeAreaOfCurveLoops(wallFaceEdges);
        //    //            transaction.RollBack();

        //    //        }
        //    //    }, doc).Wait();



        //    //    Assert.True(grossArea > 0);
        //    //}


        //}




        //public class DocFixture : IDisposable
        //{
        //    public Document Doc { get; set; }
        //    public IList<Element> Walls { get; set; }


        //    public DocFixture()
        //    {
        //        var testModel = Utils.GetTestModel("walls.rvt");
        //        Doc = xru.OpenDoc(testModel);

        //        Walls = new FilteredElementCollector(Doc).WhereElementIsNotElementType().OfCategory(BuiltInCategory.OST_Walls).ToElements();
        //    }

        //    public void Dispose()
        //    {
        //    }
        //}
        //public class TestWithFixture : IClassFixture<DocFixture>
        //{
        //    DocFixture fixture;
        //    public TestWithFixture(DocFixture fixture)
        //    {
        //        this.fixture = fixture;
        //    }

        //    [Fact]
        //    public void CountWalls()
        //    {
        //        Assert.Equal(4, fixture.Walls.Count);
        //    }

        //    [Fact]
        //    public void WallOffset()
        //    {
        //        var wall = fixture.Doc.GetElement(new ElementId(346573));
        //        var param = wall.get_Parameter(BuiltInParameter.WALL_BASE_OFFSET);
        //        var baseOffset = UnitUtils.ConvertFromInternalUnits(param.AsDouble(), param.DisplayUnitType);

        //        Assert.Equal(2000, baseOffset);
        //    }

        //    [Fact]
        //    public void MoveWallsUp()
        //    {
        //        var walls = fixture.Walls.Where(x => x.Id.IntegerValue != 346573);

        //        xru.RunInTransaction(() =>
        //        {
        //            foreach (var wall in walls)
        //            {
        //                var param = wall.get_Parameter(BuiltInParameter.WALL_BASE_OFFSET);
        //                var baseOffset = UnitUtils.ConvertToInternalUnits(2000, param.DisplayUnitType);
        //                param.Set(baseOffset);
        //            }
        //        }, fixture.Doc)
        //        .Wait(); // Important! Wait for action to finish

        //        foreach (var wall in walls)
        //        {
        //            var param = wall.get_Parameter(BuiltInParameter.WALL_BASE_OFFSET);
        //            var baseOffset = UnitUtils.ConvertFromInternalUnits(param.AsDouble(), param.DisplayUnitType);
        //            Assert.Equal(2000, baseOffset);
        //        }
        //    }
    }
}
