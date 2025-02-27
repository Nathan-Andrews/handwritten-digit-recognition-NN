using Microsoft.VisualBasic.FileIO;


namespace DigitRecognition.DataHandling {
    public static class CSVParser {
        public static DataSet Parse(string path, int numFeatures, int numLabels) {
            Console.WriteLine("Parsing");
            DataSet dataPoints = new();

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
                        features[field] = Convert.ToSingle(fields[field]);
                    }
                    int label = Convert.ToInt16(fields[numFeatures]);

                    dataPoints.Add(new DataPoint(features, label, numLabels));
                }
            }

            Console.WriteLine("Finished Parsing");

            return dataPoints;
        }
    }
}