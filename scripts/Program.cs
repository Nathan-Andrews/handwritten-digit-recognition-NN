

using System.Diagnostics;
using System.Runtime.InteropServices;

namespace simple_network {
    public class Program {
        private static ConsoleKey keyPressed = ConsoleKey.A; 

        // train the network on a set of 2 dimensonal data and visualize how the network changes on a graph
        static void Run2dDataSetVisualization() {
            // HashSet<DataPoint> dataPoints = CSVParser.Parse("/Users/nathanandrews/Desktop/c#_projects/neural_network/untrained-simple-network/data//my_madeup_dataset.csv",2,2);
            DataSet dataPoints = CSVParser.Parse("/Users/nathanandrews/Desktop/c#_projects/neural_network/untrained-simple-network/data/training//data2.csv",2,2);
            
            using (var window = new TrainingVisualizer(800, 800, "Gradient Descent Visualization"))
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
            ImageSet trainingSet = new(-1);
            ImageSet testingSet = new(-1,"./data/training/MNIST_ORG/t10k");

            Network network = new(784,100,10);

            Thread keyThread = new Thread(WaitForKey);
            keyThread.Start();

            int epoch = 0;
            while (true) {
                epoch++;

                network.Fit(trainingSet.dataPoints,0.1);

                if (epoch % 10 == 0) {
                    double trainingSetAccuracy = network.GetAccuracy(trainingSet.dataPoints);
                    double testingSetAccuracy = network.GetAccuracy(testingSet.dataPoints);
                    Console.WriteLine($"[{epoch}] {Math.Round(trainingSetAccuracy,4)} | {Math.Round(testingSetAccuracy,4)}");
                }

                if (keyPressed.Equals(ConsoleKey.Escape)) {
                    Console.WriteLine("exiting");
                    NetworkFile.SaveNetwork(network,"digit_recognition.ubyte",true);
                    break;
                }
            }

            keyThread.Join();
        }

        static void RunPretrainedImageClassification() {
            ImageSet testingSet = new(-1,"./data/training/MNIST_ORG/t10k");

            Network network = NetworkFile.LoadNetwork("test");

            double accuracy = network.GetAccuracy(testingSet.dataPoints);
            Console.WriteLine($"{Math.Round(accuracy,4)}");
        }

        static void RunImageDrawingClassification() {
            Network network = NetworkFile.LoadNetwork("digit_recognition.ubyte");

            using (var window = new DrawingVisualizer(800, "Drawing Visualization")) {
                window._network = network;
                window.Run();
            }
        }

        private static void WaitForKey()
        {
            while (!keyPressed.Equals(ConsoleKey.Escape)) {
                ConsoleKeyInfo keyInfo = Console.ReadKey(); // Wait for a key press
                keyPressed = keyInfo.Key;
            }
        }



        public static void Main(String[] args) {
            // Run2dDataSetVisualization();

            // RunImageDatasetVisualization();

            // RunImageClassificationTraining();

            // RunPretrainedImageClassification();

            RunImageDrawingClassification();
        }
    }
}
