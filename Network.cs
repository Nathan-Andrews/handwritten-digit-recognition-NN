
namespace simple_network {

    public class Network {
        Layer[] layers;
        readonly int batchSize = 8; // the size of the minibatch used when calculating the cost
        Random random= new Random();

        public Network(params int[] layerSizes) {
            layers = new Layer[layerSizes.Length - 1];

            for (int i = 0; i < layers.Length; i++) {
                layers[i] = new Layer(layerSizes[i],layerSizes[i+1]);
            }
        }

        public Network(Network old) { // copy constructor
            layers = new Layer[old.layers.Count()];
            old.layers.CopyTo(layers, 0);
            batchSize = old.batchSize;
        }

        // for minibatch gradient descent
        // selects a subset of the data to be considered in training
        // this speeds up training because the cost function has less points to loop through
        // it can also help the network escape settle points in the cost landscape
        // a new batch is choosen each epoch so that all data is considered
        private HashSet<DataPoint> SelectMinibatch(HashSet<DataPoint> points) {
            HashSet<DataPoint> batch = new HashSet<DataPoint>();
            while (batch.Count <= batchSize) {
                batch.Add(points.ToArray()[random.Next(points.Count)]); //    node: should make more memory efficient later
            }
            return batch;
        }
        
        public static int PickClass(double[] inputs) { // returns the class with the highest weight 
            int maxIndex = 0;
            for (int i = 1; i < inputs.Length; i++) {
                if (inputs[i] > inputs[maxIndex]) maxIndex = i;
            }
            return maxIndex;
        }

        public static double ActivationFunction(double input) {
            // sigmoid activation function
            // 1 / (1 + e^-x)
            return 1 / (1 + Math.Exp(-input));
        }

        double[] GetOutputs(double[] inputs) {
            foreach (Layer layer in layers) {
                inputs = layer.GetLayerOutputs(inputs);
            }

            return inputs;
        }

        double Cost(DataPoint point) { // loss function for a single datapoint
            double[] weightedOutputs = GetOutputs(point.feature);
            Layer outputLayer = layers[layers.Length - 1]; // the last layer
            double cost = 0;

            for (int i = 0; i < weightedOutputs.Length; i++) {
                cost += outputLayer.NodeCost(weightedOutputs[i],point.expectedOutput[i]); // add up the cost of each node in the output layer
            }

            return cost;
        }

        double Cost(HashSet<DataPoint> points) { // average loss of multiple datapoints
            double cost = 0;

            foreach (DataPoint point in points) {
                cost += Cost(point);
            }

            return cost / points.Count; // divide by the number of points to get the average
        }

        public void Fit(HashSet<DataPoint> points, double learningRate) {
            HashSet<DataPoint> trainingData = SelectMinibatch(points);

            double h = 0.0001; // amount to jiggle the weights/biases
            double originalCost = Cost(trainingData); // find the initial cost with the previous weights

            foreach (Layer layer in layers) {

                // calculate the cost gradient for the weights
                for (int outputNode = 0; outputNode < layer.numOutputs; outputNode++) {
                    for (int inputNode = 0; inputNode < layer.numInputs; inputNode++) {
                        layer.weights[inputNode,outputNode] += h; // jiggle the weight
                        double deltaCost = Cost(trainingData) - originalCost; // the change in cost // will be negative if the change is adventagious
                        layer.weights[inputNode,outputNode] -= h; // revert the jiggle so that the other node calculations aren't affected
                        layer.costGradientWeight[inputNode,outputNode] = deltaCost / h; 
                    }
                }

                // calculate the cost gradient for the biases
                for (int biasIndex = 0; biasIndex < layer.biases.Length; biasIndex++) {
                    layer.biases[biasIndex] += h; // jiggle the bias
                    double deltaCost = Cost(trainingData) - originalCost; // the change in cost // will be negative if the change is adventagious
                    layer.biases[biasIndex] -= h; // revert the jiggle so that the other node calculations aren't affected
                    layer.costGradientBias[biasIndex] = deltaCost / h; 
                }
            }

            ApplyAllGradients(learningRate);

            Console.WriteLine("Cost: " + Cost(trainingData)); // comment out when actually training because this slows it down
        }

        private void ApplyAllGradients(double learningRate) {
            foreach (Layer layer in layers) {
                layer.ApplyGradients(learningRate);
            }
        }

        public int Classify(double[] inputs) {
            double[] outputs = GetOutputs(inputs);

            return PickClass(outputs);
        }
    }
}