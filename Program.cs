

using System.Diagnostics;

namespace simple_network {
    public class Program {
        // train the network on a set of 2 dimensonal data and visualize how the network changes on a graph
        static void Run2dDataSetVisualization() {
            // HashSet<DataPoint> dataPoints = CSVParser.Parse("/Users/nathanandrews/Desktop/c#_projects/neural_network/untrained-simple-network/data//my_madeup_dataset.csv",2,2);
            HashSet<DataPoint> dataPoints = CSVParser.Parse("/Users/nathanandrews/Desktop/c#_projects/neural_network/untrained-simple-network/data//data.csv",2,2);
            
            using (var window = new Visualize(800, 800, "Gradient Descent Visualization"))
            {
                window.AddPoints(dataPoints);
                window.Run();
            }
        }

        public static void Main(String[] args) {
            // Run2dDataSetVisualization();

            ImageSet set = new(10);

            using (var window = new ImageVisualizer(800, 800, "Digit Visualization",set)) {
                window.Run();
            }
        }
    }
}
