using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VZTagMaterial.Auxiliar
{
    public static class DirectShapeCreator
    {
        public static HashSet<Type> AcceptedTypesOfFaces = new HashSet<Type>() 
        { 
            typeof(ConicalFace), 
            typeof(CylindricalFace), 
            typeof(HermiteFace), 
            typeof(PlanarFace), 
            typeof(RevolvedFace), 
            typeof(RuledFace)
        };

        public static DirectShape CreateGenericDirectShape(GeometryObject geometryObject)
        {
            if (AcceptedTypesOfFaces.Contains(geometryObject.GetType()))
            {
                Face faceObject = geometryObject as Face;
                geometryObject = faceObject.Triangulate();
            }

            DirectShape genericDirectShape = DirectShape.CreateElement(DocInfo.Doc, new ElementId(BuiltInCategory.OST_GenericModel));
            try
            {
                genericDirectShape.SetShape(new GeometryObject[] { geometryObject });
            }
            catch (Exception)
            {
                Solid newGeometryObject = geometryObject as Solid;
                List<PlanarFace> solidFaces = iCGeometry.TransformFaceArrayIntoList(newGeometryObject);
                Mesh[] triangulatedFaces = solidFaces.Select(x => x.Triangulate()).ToArray();

                genericDirectShape.SetShape(triangulatedFaces);
            }
            

            return genericDirectShape;
        }
        
    }
}
