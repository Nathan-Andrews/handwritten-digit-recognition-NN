
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

        public int Classify(double[] inputs) {
            double[] outputs = GetOutputs(inputs);

            return PickClass(outputs);
        }
    }
}