

namespace simple_network {
    public class Program {
        public static void Main(String[] args) {
            HashSet<DataPoint> dataPoints = CSVParser.parse("./data/my_madeup_dataset.csv",2,2);
            
            using (var window = new Visualize(800, 800, "OpenTK Shaders"))
            {
                window.addPoints(dataPoints);
                window.Run();
            }
        }
    }
}
