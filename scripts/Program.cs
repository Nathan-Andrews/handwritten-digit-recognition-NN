﻿using DigitRecognition.Visualization;
using DigitRecognition.DataHandling;
using DigitRecognition.ImageHandling;
using DigitRecognition.NeuralNetwork;

// using simple_network;

namespace DigitRecognition {
    public class Program {
        private static ConsoleKey keyPressed = ConsoleKey.A; 

        // train the network on a set of 2 dimensonal data and visualize how the network changes on a graph
        static void Run2dDataSetVisualization() {
            Config config= new Config();

            // "/Users/nathanandrews/Desktop/c#_projects/neural_network/untrained-simple-network/data/training//data2.csv"
            int numFeatures = config.Training.LayerSizes[0];
            int numLabels = config.Training.LayerSizes[^1];
            DataSet dataPoints = CSVParser.Parse(config.Dataset.TrainingSetPath,numFeatures,numLabels);
            
            using (var window = new TrainingVisualizer(800, 800, "Gradient Descent Visualization",config.Training.LayerSizes))
            {
                window.AddPoints(dataPoints);
                window.Run();
            }
        }

        static void RunImageDatasetVisualization() {
            Config config = new();

            ImageSet set = new(config.Dataset.ImagePreviewCount);

            using (var window = new ImageVisualizer(800, 800, "Digit Visualization",set)) {
                window.Run();
            }
        }

        static void RunImageClassificationTraining() {
            Config config = new Config();

            ImageSet trainingSet = new(-1,config.Dataset.TrainingSetPath);
            ImageSet testingSet = config.Training.DoAccuracyCheck
                ? new(-1,config.Dataset.TestingSetPath)
                : new();

            Network network = new(config.Training.LayerSizes);
            network.SetBatchSize(config.Training.MiniBatchSize);

            Thread keyThread = new Thread(WaitForKey);
            keyThread.Start();

            int epoch = 0;
            while (true) {
                epoch++;

                network.Fit(trainingSet.dataPoints,config.Training.LearningRate);

                if (epoch % 10 == 0 && config.Training.DoAccuracyCheck) {
                    double trainingSetAccuracy = network.GetAccuracy(trainingSet.dataPoints);
                    double testingSetAccuracy = network.GetAccuracy(testingSet.dataPoints);
                    Console.WriteLine($"[{epoch}] {Math.Round(trainingSetAccuracy,4)} | {Math.Round(testingSetAccuracy,4)}");
                }

                if (keyPressed.Equals(ConsoleKey.Escape)) {
                    if (config.Training.DoNetworkFile) {
                        Console.WriteLine("... saving and exiting");
                        NetworkFile.SaveNetwork(network,config.Training.NetworkFile,config.Training.DoOverwrite);
                    }
                    else {
                        Console.WriteLine("... exiting without saving");
                    }
                    break;
                }
            }

            keyThread.Join();
        }

        static void RunPretrainedImageClassification() {
            Config config = new();

            ImageSet testingSet = new(-1,config.Dataset.TestingSetPath);

            Network network = NetworkFile.LoadNetwork(config.ImageClassification.StoredNetworkFile);

            double accuracy = network.GetAccuracy(testingSet.dataPoints);
            Console.WriteLine($"{Math.Round(accuracy,4)}");
        }

        static void RunImageDrawingClassification() {
            Config config = new();

            Network network = NetworkFile.LoadNetwork(config.ImageClassification.StoredNetworkFile);

            using (var window = new DrawingVisualizer(800, "Drawing Visualization")) {
                window._network = network;
                window.Run();
            }
        }

        private static void WaitForKey() {
            while (!keyPressed.Equals(ConsoleKey.Escape)) {
                ConsoleKeyInfo keyInfo = Console.ReadKey(); // Wait for a key press
                keyPressed = keyInfo.Key;
            }
        }


        public static void Main(String[] args) {
            Config config = new Config();

            // Run2dDataSetVisualization();

            // RunImageDatasetVisualization();

            // RunImageClassificationTraining();

            // RunPretrainedImageClassification();

            // RunImageDrawingClassification();

            if (config.Dataset.IsImage && config.Dataset.DoImagePreview) {
                RunImageDatasetVisualization();
            }

            if (config.Training.DoTraining) {
                if (config.Dataset.IsCSV) {
                    Run2dDataSetVisualization();
                }
                else if (config.Dataset.IsImage) {
                    RunImageClassificationTraining();
                }
                else {
                    Config.PrintConfigIssue("Nothing was run.\n ... Either Program.Training.IsImage or Program.Training.IsCSVDataset should be true");
                }
            }

            if (config.ImageClassification.DoClassification) {
                if (config.ImageClassification.DoAccuracyCheck) {
                    RunPretrainedImageClassification();
                }

                if (config.ImageClassification.DoDrawingMode) {
                    RunImageDrawingClassification();
                }

                if (!config.ImageClassification.DoAccuracyCheck && !config.ImageClassification.DoDrawingMode) {
                    Config.PrintConfigIssue("Nothing was run.\n ... Either Program.ImageClassification.DoDrawingMode or Program.ImageClassification.DatasetAccuracyCheck.DoDatasetAccuracyCheck should be true");
                }
            }
        }
    }
}
