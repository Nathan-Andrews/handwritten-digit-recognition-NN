
using System.Runtime.CompilerServices;
using DigitRecognition.DataHandling;

namespace DigitRecognition.NeuralNetwork {

    public class Network {
        public Layer[] layers;
        int batchSize = 16; // the size of the minibatch used when calculating the cost
        NetworkData[] batchNetworkData;

        public Network(params int[] layerSizes) {
            layers = new Layer[layerSizes.Length - 1];

            for (int i = 0; i < layers.Length; i++) {
                layers[i] = new Layer(layerSizes[i],layerSizes[i+1]);
            }

            batchNetworkData = new NetworkData[batchSize];
        }

        public Network(Network old) { // copy constructor (needs work)
            layers = new Layer[old.layers.Count()];
            old.layers.CopyTo(layers, 0);
            batchSize = old.batchSize;

            batchNetworkData = new NetworkData[batchSize];
        }

        public void SetBatchSize(int batchSize) {
            this.batchSize = batchSize;

            batchNetworkData = new NetworkData[batchSize];
        }

        
        // for classifcation
        // returns the class with the highest weightedInput
        // is run by Classify(...) to determine the class of a data point
        //  after that point has been run through the network
        public static int PickClass(double[] weightedInputs) {
            int maxIndex = 0;
            for (int i = 1; i < weightedInputs.Length; i++) {
                if (weightedInputs[i] > weightedInputs[maxIndex]) maxIndex = i;
            }
            return maxIndex;
        }

        public double[] GetOutputs(double[] inputs) {
            foreach (Layer layer in layers) {
                inputs = layer.GetLayerOutputs(inputs);
            }

            return inputs;
        }

        void FowardPass(double[] inputs, NetworkData networkData) { // updates the internal values for the funcitons
            for (int i = 0; i < layers.Length; i++) {
                inputs = layers[i].PerformLayerPass(inputs,networkData.layerData[i]);
            }
        }

        // used in gradient descent
        // cost function for a single point
        // used in big Cost(...) which adds this output to the sum of the cost of all points
        // determines if a change to the weights causes the network to classify this point better or worse
        double Cost(DataPoint point) { // loss function for a single datapoint
            double[] networkOutputs = GetOutputs(point.feature);
            Layer outputLayer = layers[layers.Length - 1]; // the last layer
            double cost = 0;

            for (int i = 0; i < networkOutputs.Length; i++) {
                cost += outputLayer.NodeCost(networkOutputs[i],point.expectedOutput[i]); // add up the cost of each node in the output layer
            }

            return cost;
        }

        // used in gradient descent
        // cost function for the whole network
        // sum of the cost of each dataPoint being considered
        // used to determine whether a small change to the network is positive or negative
        double Cost(DataSet points) {
            double cost = 0;

            for(int i = 0; i < points.size; i++) {
                cost += Cost(points.GetElement(i));
            }

            return cost / points.size; // divide by the number of points to get the average
        }

        // used in training
        // performs one epoch of training on the network
        // called in a loop outside of the network
        // uses backprogagation gradient descent to train the network
        // uses a random mini-batch subset so that not every point is considered each epoch
        // works by using the derivatives to find the slop of the cost landscape at a certain point
        //  which is the gradient at that point
        //  after considering each layer, the gradients are used to apply changes
        //  to the network
        public void Fit(DataSet points, double learningRate) {
            DataSet trainingData = points.SelectMinibatch(batchSize);

            ResetGradients();


			for (int i = 0; i < batchSize; i++)
			{
				batchNetworkData[i] = new NetworkData(layers);
			}

            Parallel.For(0, trainingData.size, (i) => {
                Backpropagate(trainingData.GetElement(i),batchNetworkData[i]);
            });

            // ApplyAllGradients(learningRate / batchSize);
            ApplyAllGradients(learningRate);

            // Console.WriteLine("Cost: " + Cost(trainingData)); // comment out when actually training because this slows it down
        }

        // for gradient descent
        // used in Fit(...)
        // updates the weights and biases using the calculated gradients for each layer
        private void ApplyAllGradients(double learningRate) {
            foreach (Layer layer in layers) {
                layer.ApplyGradients(learningRate);
            }
        }

        // used in training
        // backpropagation for a certian point
        // used in Fit(...) to run each point through the network
        // goes backwards through the network to update the gradients for each layer
        // the goal is to find how each weight and bias effects the cost
        // each layer uses the node values of the layer before it, which is why it needs to go backwards
        private void Backpropagate(DataPoint point, NetworkData pointNetworkData) {
            // runs the point through the layers so that they store relevent values like the input and activations
            FowardPass(point.feature,pointNetworkData);

            Layer outputLayer = layers[^1];
            double[] nodeValues = outputLayer.CalculateOutputLayerNodeValues(point.expectedOutput,pointNetworkData.layerData[^1]);
            outputLayer.UpdateGradients(nodeValues,pointNetworkData.layerData[^1]);

            for (int layerIndex = layers.Length - 2; layerIndex >= 0; layerIndex--) { // loop backwards through the layers
                Layer hiddenLayer = layers[layerIndex];
                nodeValues = hiddenLayer.CalculateHiddenLayerNodeValues(layers[layerIndex + 1],pointNetworkData.layerData[layerIndex],nodeValues);
                hiddenLayer.UpdateGradients(nodeValues,pointNetworkData.layerData[layerIndex]);
            }
        }

        // for training
        // resets the gradients for each layer to 0
        // used in Fit(...) to reset the gradients before each epoch
        private void ResetGradients() {
            foreach (Layer layer in layers) {
                for (int outputNode = 0; outputNode < layer.numOutputs; outputNode++) {
                    for (int inputNode = 0; inputNode < layer.numInputs; inputNode++) {
                        layer.costGradientWeight[inputNode,outputNode] = 0.0;
                    }
                    layer.costGradientBias[outputNode] = 0.0;
                }
            }
        }

        // for classification
        // runs a multi-dimensional point through the network to determine the output
        // used when determining the class of a point, such as when visualizing the texture
        public int Classify(double[] inputs) {
            double[] weightedInputs = GetOutputs(inputs);

            return PickClass(weightedInputs);
        }

        public double GetAccuracy(DataSet points) {
            int amountCorrect = 0;
            object _lock = new();

            Parallel.For(0, points.size, (i) => {
                int result = Classify(points.GetElement(i).feature);

                lock (_lock) {
                    if (result == points.GetElement(i).label) amountCorrect++;
                }
            });

            return ((double) amountCorrect) / points.size;
        }
    }
}