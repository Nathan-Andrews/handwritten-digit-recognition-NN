namespace simple_network {
    public class Layer {
        int numInputs, numOutputs;
        public double[,] weights;
        public double[] biases;

        public Layer(int numInputs, int numOutputs) {
            this.numInputs = numInputs;
            this.numOutputs = numOutputs;

            weights = new double[numInputs, numOutputs];
            biases = new double[numOutputs];
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
    }
}