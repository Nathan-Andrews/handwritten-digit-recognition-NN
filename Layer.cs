namespace simple_network {
    public class Layer {
        public int numInputs, numOutputs;
        public double[,] weights;
        public double[] biases;
        public double[,] costGradientWeight;
        public double[] costGradientBias;

        public Layer(int numInputs, int numOutputs) {
            this.numInputs = numInputs;
            this.numOutputs = numOutputs;

            weights = new double[numInputs, numOutputs];
            biases = new double[numOutputs];

            costGradientWeight = new double[numInputs, numOutputs];
            costGradientBias = new double[numOutputs];

            // initialize random weights
            {
                Random rng = new();
                
                for (int outputNode = 0; outputNode < numOutputs; outputNode++) {

                    for (int inputNode = 0; inputNode < numInputs; inputNode++) {
                        double randomValue = (rng.NextDouble() * 2) - 1; // initalize value between zero and 1
                        weights[inputNode,outputNode] = randomValue / Math.Sqrt(numInputs); 
                        // Xavier/Glorot Initialization
                        // We divide by the sqrt of the number of input nodes
                        // This ensures that the gradents are not too large, which would result in slow learning
                    }
 
                }
            }
        }

        public double[] GetLayerOutputs(double[] inputs) {
            double[] outputs = new double[numOutputs];

            for (int outputNode = 0; outputNode < numOutputs; outputNode++) {
                double output = 0;

                for (int inputNode = 0; inputNode < numInputs; inputNode++) {
                    output += inputs[inputNode] * weights[inputNode,outputNode];
                }

                output += biases[outputNode];

                outputs[outputNode] = Network.ActivationFunction(output);
            }

            return outputs;
        }

        // cost function of a single node
        public double NodeCost(double outputRecieved, double outputExpected) {
            double error = outputRecieved - outputExpected; // the different between the value that the node returns vs the value we want it to be
            return error * error; // to ensure return value is always positive
        }

        public void ApplyGradients(double learningRate) {
            for (int outputNode = 0; outputNode < numOutputs; outputNode++) {
                biases[outputNode] -= costGradientBias[outputNode] * learningRate;

                for (int inputNode = 0; inputNode < numInputs; inputNode++) {
                    weights[inputNode,outputNode] -= costGradientWeight[inputNode,outputNode] * learningRate;
                }
            }
        }
    }
}