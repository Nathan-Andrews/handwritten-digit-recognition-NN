

namespace simple_network {
    public class Program {
        public static void Main(String[] args) {
            HashSet<DataPoint> dataPoints = CSVParser.parse("./data/my_madeup_dataset.csv",2,2);

            foreach (DataPoint point in dataPoints) {
                Console.WriteLine(point.feature[0] + ", " + point.feature[1] + ", " + point.label);
            }
            
            using (var window = new Visualize(800, 800, "OpenTK Shaders"))
            {
                window.addPoints(dataPoints);
                window.Run();
            }
        }
    }
}
