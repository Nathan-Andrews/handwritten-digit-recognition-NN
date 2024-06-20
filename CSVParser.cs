using System.Reflection.Emit;
using Microsoft.VisualBasic.FileIO;

public static class CSVParser {

        public static HashSet<DataPoint> parse(string path, int numFeatures, int numLabels) {
            Console.WriteLine("Parsing");
            HashSet<DataPoint> dataPoints = new HashSet<DataPoint>();

            using (TextFieldParser parser = new TextFieldParser(path))
            {   
                parser.TextFieldType = FieldType.Delimited;
                parser.SetDelimiters(",");
                parser.ReadFields(); // skip first line
                while (!parser.EndOfData)
                {
                    //Process row
                    string[]? fields = parser.ReadFields();
                    if (fields == null || fields.Length != (numFeatures + 1)) continue;

                    double[] features = new double[numFeatures];
                    
                    for (int field = 0; field < fields.Length - 1; field++) {
                        Console.WriteLine(fields[field]);

                        features[field] = Convert.ToDouble(fields[field]);
                    }
                    int label = Convert.ToInt16(fields[numFeatures]);

                    dataPoints.Add(new DataPoint(features, label, numLabels));
                }
            }

            return dataPoints;
        }
    }