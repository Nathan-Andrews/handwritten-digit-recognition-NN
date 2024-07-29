namespace DigitRecognition.NeuralNetwork {
    public class NetworkData {
        public LayerData[] layerData;

        public NetworkData(Layer[] layers) {
            layerData = new LayerData[layers.Length];
            for (int i = 0; i < layers.Length; i++) {
                layerData[i] = new LayerData(layers[i]);
            }
        }
    }

    public class LayerData {
        public double[] inputs;
        public double[] weightedInputs;
        public double[] activationValues;

        public LayerData(Layer layer) {
            weightedInputs = new double[layer.numOutputs];
            inputs = new double[layer.numInputs];
            activationValues = new double[layer.numOutputs];
        }
    }
}