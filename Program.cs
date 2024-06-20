

namespace simple_network {
    public class Program {
        public static void Main(String[] args) {
            HashSet<DataPoint> dataPoints = CSVParser.Parse("./data/my_madeup_dataset.csv",2,2);
            
            using (var window = new Visualize(800, 800, "OpenTK Shaders"))
            {
                window.AddPoints(dataPoints);
                window.Run();
            }
        }
    }
}
