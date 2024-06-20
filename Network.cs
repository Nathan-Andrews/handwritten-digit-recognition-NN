
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
        //     float[] b = {0.3f,-0.6f,0.5f};
        //     layers[0].biases = b;
        //     float[,] w = new float[2,3]
        //     w = {{0.0f,-0.3f},{-2.4f,-0.2f,0.7f}};
        //     layers[0].weights = w;
        // }
        
        public static int PickClass(float[] inputs) {
            int maxIndex = 0;
            for (int i = 0; i < inputs.Length; i++) {
                if (inputs[i] > inputs[maxIndex]) maxIndex = i;
            }
            return maxIndex;
        }

        public static float ActivationFunction(float input) {
            return 1 / (1 + (float)Math.Exp(input));
        }

        float[] GetOutputs(float[] inputs) {
            foreach (Layer layer in layers) {
                inputs = layer.GetLayerOutputs(inputs);
            }

            return inputs;
        }

        public int Classify(float[] inputs) {
            float[] outputs = GetOutputs(inputs);

            return PickClass(outputs);
        }
    }
}