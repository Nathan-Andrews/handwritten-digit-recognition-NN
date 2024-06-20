namespace simple_network {
    public class Layer {
        int numInputs, numOutputs;
        public float[,] weights;
        public float[] biases;

        public Layer(int numInputs, int numOutputs) {
            this.numInputs = numInputs;
            this.numOutputs = numOutputs;

            weights = new float[numInputs, numOutputs];
            biases = new float[numOutputs];
        }

        public float[] GetLayerOutputs(float[] inputs) {
            float[] outputs = new float[numOutputs];

            for (int outputNode = 0; outputNode < numOutputs; outputNode++) {
                float output = 0;

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