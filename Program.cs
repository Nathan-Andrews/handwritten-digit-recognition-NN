

using System.Diagnostics;
using System.Runtime.InteropServices;

namespace simple_network {
    public class Program {
        // train the network on a set of 2 dimensonal data and visualize how the network changes on a graph
        static void Run2dDataSetVisualization() {
            // HashSet<DataPoint> dataPoints = CSVParser.Parse("/Users/nathanandrews/Desktop/c#_projects/neural_network/untrained-simple-network/data//my_madeup_dataset.csv",2,2);
            DataSet dataPoints = CSVParser.Parse("/Users/nathanandrews/Desktop/c#_projects/neural_network/untrained-simple-network/data//data.csv",2,2);
            
            using (var window = new Visualize(800, 800, "Gradient Descent Visualization"))
            {
                window.AddPoints(dataPoints);
                window.Run();
            }
        }

        static void RunImageDatasetVisualization() {
            ImageSet set = new(20);

            using (var window = new ImageVisualizer(800, 800, "Digit Visualization",set)) {
                window.Run();
            }
        }

        static void RunImageClassificationTraining() {
            ImageSet trainingSet = new(1000);
            ImageSet testingSet = new(300,"/Users/nathanandrews/Desktop/c#_projects/neural_network/untrained-simple-network/data/MNIST_ORG/train");

            Network network = new(784,100,10);

            int epoch = 0;
            while (true) {
                epoch++;

                network.Fit(trainingSet.dataPoints,0.1);

                if (epoch % 10 == 0) {
                    double trainingSetAccuracy = network.GetAccuracy(trainingSet.dataPoints);
                    double testingSetAccuracy = network.GetAccuracy(testingSet.dataPoints);
                    Console.WriteLine($"[{epoch}] {Math.Round(trainingSetAccuracy,4)} | {Math.Round(testingSetAccuracy,4)}");
                }
            }
        }



        public static void Main(String[] args) {
            // Run2dDataSetVisualization();

            // RunImageDatasetVisualization();

            RunImageClassificationTraining();
        }
    }
}
