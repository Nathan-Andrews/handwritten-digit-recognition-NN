public class Network {
    private static float weight_1_1 = -0.3f; // sample values
    private static float weight_1_2 = -0.8f; // sample values

    private static float weight_2_1 = 0; // sample values
    private static float weight_2_2 = 0.0f; // sample values

    private static float bias_1 = 0.2f; // sample values
    private static float bias_2 = 0;

    public Network() {
        
    }
    
    public static int activationFunction(float input_1, float input_2) {
        return (input_1 > input_2) ? 0 : 1;
    }

    public static int classify(float input_1, float input_2) {
        float output_1 = input_1 * weight_1_1 + input_2 * weight_2_1 + bias_1;
        float output_2 = input_2 * weight_1_2 + input_2 * weight_2_2 + bias_2;

        return activationFunction(output_1, output_2);
    }
}