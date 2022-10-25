using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevitGeometryUtils
{
    public class DynamoLineOutput
    {
        XYZ StartPoint { get; set; }
        XYZ EndPoint { get; set; }
        string DynamoStringLine { get; set; }
        string DynamoStringStartPoints { get; set; }
        string DynamoStringEndPoints { get; set; }
        Line RevitInstance { get; set; }
        public DynamoLineOutput(Line line)
        {
            RevitInstance = line;
            StartPoint = RevitInstance.GetEndPoint(0);
            EndPoint = RevitInstance.GetEndPoint(1);
            DynamoStringStartPoints = $"Point.ByCoordinates({StartPoint.X}, {StartPoint.Y}, {StartPoint.Z})";
            DynamoStringEndPoints = $"Point.ByCoordinates({EndPoint.X}, {EndPoint.Y}, {EndPoint.Z})";
            DynamoStringLine = $"Line.ByStartPointEndPoint(Point.ByCoordinates({StartPoint.X}, {StartPoint.Y}, {StartPoint.Z}), Point.ByCoordinates({EndPoint.X}, {EndPoint.Y}, {EndPoint.Z}))";
        }

        public static string[] CreateDynamoOutputFromListOfCurves(List<Curve> curves)
        {
            List<DynamoLineOutput> output = new List<DynamoLineOutput>();

            foreach (Curve curve in curves)
            {
                string curveTypeName = curve.GetType().Name;

                if (curveTypeName == "Line")
                {
                    DynamoLineOutput newDynamoOutput = new DynamoLineOutput(curve as Line);
                    output.Add(newDynamoOutput);
                }
            }

            string listOfLines = GetLineStringForListOfLines(output);
            string listOfStartPoints = GetLineStringForListOfLineStartPoints(output);
            string listOfEndPoints = GetLineStringForListOfLineEndPoints(output);

            string[] outputStrings = new string[3]{ listOfLines, listOfStartPoints, listOfEndPoints };

            return outputStrings;
        }
        public static string GetLineStringForListOfLines(List<DynamoLineOutput> dynamoLines)
        {
            List<string> dynamoLineStrings = new List<string>();

            foreach (DynamoLineOutput dynamoLine in dynamoLines)
            {
                dynamoLineStrings.Add(dynamoLine.DynamoStringLine);
            }

            string joinedStrings = string.Join(",", dynamoLineStrings);

            return "[" + joinedStrings + "]";
        }
        public static string GetLineStringForListOfLineStartPoints(List<DynamoLineOutput> dynamoLines)
        {
            List<string> dynamoPointsStrings = new List<string>();

            foreach (DynamoLineOutput dynamoLine in dynamoLines)
            {
                dynamoPointsStrings.Add(dynamoLine.DynamoStringStartPoints);
            }

            string joinedStrings = string.Join(",", dynamoPointsStrings);

            return "[" + joinedStrings + "]";
        }
        public static string GetLineStringForListOfLineEndPoints(List<DynamoLineOutput> dynamoLines)
        {
            List<string> dynamoPointsStrings = new List<string>();

            foreach (DynamoLineOutput dynamoLine in dynamoLines)
            {
                dynamoPointsStrings.Add(dynamoLine.DynamoStringEndPoints);
            }

            string joinedStrings = string.Join(",", dynamoPointsStrings);

            return "[" + joinedStrings + "]";
        }

    }
}
