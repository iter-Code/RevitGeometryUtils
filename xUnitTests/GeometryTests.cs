using Autodesk.Revit.DB;
using Autodesk.Revit.DB.IFC;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
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
        /// <summary>
        /// Checks whether all walls in the model have a valid volume
        /// </summary>
        [Fact]
        public void WallsHaveVolume()
        {
            var testModel = Utils.GetTestModel("walls.rvt");
            var doc = xru.OpenDoc(testModel);

            var walls = new FilteredElementCollector(doc).WhereElementIsNotElementType().OfCategory(BuiltInCategory.OST_Walls).ToElements();

            foreach (var wall in walls)
            {
                var volumeParam = wall.get_Parameter(BuiltInParameter.HOST_VOLUME_COMPUTED);
                Assert.NotNull(volumeParam);
                Assert.True(volumeParam.AsDouble() > 0);
            }
        }

        [Fact]
        public void SampleFail()
        {
            var feet = UnitUtils.ConvertToInternalUnits(3000, DisplayUnitType.DUT_MILLIMETERS);
            Assert.Equal(5, feet);
        }

        //[Fact]
        //public void GetWallGrossAreaAndRollBack()
        //{
        //    var testModel = Utils.GetTestModel("walls.rvt");
        //    var doc = xru.OpenDoc(testModel);
        //    var walls = new FilteredElementCollector(doc).WhereElementIsNotElementType().OfCategory(BuiltInCategory.OST_Walls).ToElements();
        //    var wall = walls[0] as Wall;
        //    double grossArea = 0;

        //    var inserts = wall.FindInserts(true, true, true, true);
        //    xru.Run(() =>
        //    {
        //        using (Transaction transaction = new Transaction(doc, "Temporary - only to get gross area"))
        //        {
        //            transaction.Start();
        //            foreach (ElementId insertId in inserts) { doc.Delete(insertId); }
        //            doc.Regenerate();
        //            var wallFaceReference = HostObjectUtils.GetSideFaces(wall, ShellLayerType.Exterior);
        //            var face = doc.GetElement(wallFaceReference.First()).GetGeometryObjectFromReference(wallFaceReference.First()) as PlanarFace;
        //            var wallFaceEdges = face.GetEdgesAsCurveLoops();
        //            grossArea = ExporterIFCUtils.ComputeAreaOfCurveLoops(wallFaceEdges);
        //            transaction.RollBack();

        //        }
        //    }, doc).Wait();



        //    Assert.True(grossArea > 0);
        //}


    }




    public class DocFixture : IDisposable
    {
        public Document Doc { get; set; }
        public IList<Element> Walls { get; set; }


        public DocFixture()
        {
            var testModel = Utils.GetTestModel("walls.rvt");
            Doc = xru.OpenDoc(testModel);

            Walls = new FilteredElementCollector(Doc).WhereElementIsNotElementType().OfCategory(BuiltInCategory.OST_Walls).ToElements();
        }

        public void Dispose()
        {
        }
    }
    public class TestWithFixture : IClassFixture<DocFixture>
    {
        DocFixture fixture;
        public TestWithFixture(DocFixture fixture)
        {
            this.fixture = fixture;
        }

        [Fact]
        public void CountWalls()
        {
            Assert.Equal(4, fixture.Walls.Count);
        }

        [Fact]
        public void WallOffset()
        {
            var wall = fixture.Doc.GetElement(new ElementId(346573));
            var param = wall.get_Parameter(BuiltInParameter.WALL_BASE_OFFSET);
            var baseOffset = UnitUtils.ConvertFromInternalUnits(param.AsDouble(), param.DisplayUnitType);

            Assert.Equal(2000, baseOffset);
        }

        [Fact]
        public void MoveWallsUp()
        {
            var walls = fixture.Walls.Where(x => x.Id.IntegerValue != 346573);

            xru.RunInTransaction(() =>
            {
                foreach (var wall in walls)
                {
                    var param = wall.get_Parameter(BuiltInParameter.WALL_BASE_OFFSET);
                    var baseOffset = UnitUtils.ConvertToInternalUnits(2000, param.DisplayUnitType);
                    param.Set(baseOffset);
                }
            }, fixture.Doc)
            .Wait(); // Important! Wait for action to finish

            foreach (var wall in walls)
            {
                var param = wall.get_Parameter(BuiltInParameter.WALL_BASE_OFFSET);
                var baseOffset = UnitUtils.ConvertFromInternalUnits(param.AsDouble(), param.DisplayUnitType);
                Assert.Equal(2000, baseOffset);
            }
        }
    }
}
