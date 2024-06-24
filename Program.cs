

namespace simple_network {
    public class Program {
        public static void Main(String[] args) {
            // HashSet<DataPoint> dataPoints = CSVParser.Parse("/Users/nathanandrews/Desktop/c#_projects/neural_network/untrained-simple-network/data//my_madeup_dataset.csv",2,2);
            HashSet<DataPoint> dataPoints = CSVParser.Parse("/Users/nathanandrews/Desktop/c#_projects/neural_network/untrained-simple-network/data//data.csv",2,2);
            
            using (var window = new Visualize(800, 800, "OpenTK Shaders"))
            {
                Console.WriteLine("Training");
                for (int i = 0; i < 10000; i++) {
                    window.network.Fit(dataPoints,0.1);
                }
                Console.WriteLine("Finished Training");
                window.AddPoints(dataPoints);
                window.Run();
            }
        }
    }
}
