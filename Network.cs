
namespace simple_network {

    public class Network {
        Layer[] layers;

        public Network(params int[] layerSizes) {
            layers = new Layer[layerSizes.Length - 1];

            for (int i = 0; i < layers.Length; i++) {
                layers[i] = new Layer(layerSizes[i],layerSizes[i+1]);
            }
        }
        // public Network(params int[] layerSizes) {
        //     layers = new Layer[layerSizes.Length - 1];

        //     // Console.WriteLine(layerSizes.Length);

        //     // for (int i = 0; i < layers.Length; i++) {
        //     //     layers[i] = new Layer(layerSizes[i],layerSizes[i+1]);
        //     // }
        //     layers[0] = new Layer(layerSizes[0],layerSizes[1]);
        //     double[] b = {0.3f,-0.6f,0.5f};
        //     layers[0].biases = b;
        //     double[,] w = new double[2,3]
        //     w = {{0.0f,-0.3f},{-2.4f,-0.2f,0.7f}};
        //     layers[0].weights = w;
        // }
        
        public static int PickClass(double[] inputs) {
            int maxIndex = 0;
            for (int i = 0; i < inputs.Length; i++) {
                if (inputs[i] > inputs[maxIndex]) maxIndex = i;
            }
            return maxIndex;
        }

        public static double ActivationFunction(double input) {
            return 1 / (1 + (double)Math.Exp(input));
        }

        double[] GetOutputs(double[] inputs) {
            foreach (Layer layer in layers) {
                inputs = layer.GetLayerOutputs(inputs);
            }

            return inputs;
        }

        double Cost(DataPoint point) { // loss function for a single datapoint
            double[] outputs = GetOutputs(point.feature);
            Layer outputLayer = layers[layers.Length - 1]; // the last layer
            double cost = 0;

            for (int i = 0; i < outputs.Length; i++) {
                cost += outputLayer.NodeCost(outputs[i],point.expectedOutput[i]); // add up the cost of each node in the output layer
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

        public void Fit(HashSet<DataPoint> trainingData, double learningRate) {
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

            foreach (Layer layer in layers) {
                layer.ApplyGradients(learningRate);
            }

            Console.WriteLine("Cost: " + Cost(trainingData));
        }

        public int Classify(double[] inputs) {
            double[] outputs = GetOutputs(inputs);

            return PickClass(outputs);
        }
    }
}