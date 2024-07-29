namespace DigitRecognition.NeuralNetwork {
    public class Layer {
        public int numInputs, numOutputs;
        public double[,] weights;
        public double[] biases;
        public double[,] costGradientWeight;
        public double[] costGradientBias;

        // public double[] inputs;
        // public double[] weightedInputs;
        // public double[] activationValues;

        public Layer(int numInputs, int numOutputs) {
            this.numInputs = numInputs;
            this.numOutputs = numOutputs;

            weights = new double[numInputs, numOutputs];
            biases = new double[numOutputs];

            costGradientWeight = new double[numInputs, numOutputs];
            costGradientBias = new double[numOutputs];

            // weightedInputs = new double[numOutputs];
            // inputs = new double[numInputs];
            // activationValues = new double[numOutputs];

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

        // activation function
        // used in GetLayerOutputs(...) to calculate the output of a node based on the sum of the weighted inputs
        // lets the network have more complex descision boundries
        // sigmoid activation function
        //  1 / (1 + e^-x)
        static double ActivationFunction(double weightedInput) {
            return 1 / (1 + Math.Exp(-weightedInput));
        }

        // https://towardsdatascience.com/derivative-of-the-sigmoid-function-536880cf918e
        static double ActivationDerivative(double weightedInput) {
            double activationValue = ActivationFunction(weightedInput);
            return activationValue * (1 - activationValue);
        }

        // used in gradient descent
        // cost function of a single node
        // also called the loss function
        // the cost of all output nodes are summed up in the network
        //  to determine whether a small change to the weights is benificial or not
        // the difference between the value that the node returns vs the value we want it to be
        // squared to ensure return value is always positive
        public double NodeCost(double outputRecieved, double outputExpected) {
            double error = outputRecieved - outputExpected;
            return error * error;
        }

        // used in gradient descent
        // the partial derivative of the cost function
        // used in CalculateOutputLayerNodeValues(...) to find the cost derivative of an output node
        public double NodeCostDerivative(double outputRecieved, double outputExpected) {
            return 2 * (outputRecieved -outputExpected);
        }

        // for classification and calculating cost
        // gets the output of the layer
        // that either gets passed on as the input to the next layer
        //  or is used as the output of the network if its the last layer
        public double[] GetLayerOutputs(double[] inputs) {
            double[] activations = new double[numOutputs];

            for (int outputNode = 0; outputNode < numOutputs; outputNode++) {
                double output = 0;

                for (int inputNode = 0; inputNode < numInputs; inputNode++) {
                    output += inputs[inputNode] * weights[inputNode,outputNode];
                }

                output += biases[outputNode];

                activations[outputNode] = ActivationFunction(output);
            }

            return activations;
        }

        public double[] PerformLayerPass(double[] inputs, LayerData layerData) {
            layerData.activationValues = new double[numOutputs];
            layerData.inputs = inputs;

            for (int outputNode = 0; outputNode < numOutputs; outputNode++) {
                double output = 0;

                for (int inputNode = 0; inputNode < numInputs; inputNode++) {
                    output += inputs[inputNode] * weights[inputNode,outputNode];
                }

                output += biases[outputNode];

                layerData.weightedInputs[outputNode] = output;
                layerData.activationValues[outputNode] = ActivationFunction(output);
            }

            return layerData.activationValues;
        }


        // used in gradient descent
        // uses the gradients calculated from Fit(...) to update the weights and biases
        // the gradients are essentially an estimate of the slope of the cost landscape at that point
        // and we want to shift the weights/biases in a downward slopeing direction
        public void ApplyGradients(double learningRate) {
            for (int outputNode = 0; outputNode < numOutputs; outputNode++) {
                biases[outputNode] -= costGradientBias[outputNode] * learningRate;

                for (int inputNode = 0; inputNode < numInputs; inputNode++) {
                    weights[inputNode,outputNode] -= costGradientWeight[inputNode,outputNode] * learningRate;
                }
            }
        }

        // used in backpropagation
        // updates the values of the gradients for the current layer based on the node values for the layer
        // run by Backpropagate(...)
        public void UpdateGradients(double[] nodeValues, LayerData layerData) {
            lock (costGradientWeight) {
                for (int outputNode = 0; outputNode < numOutputs; outputNode++) {
                    for (int inputNode = 0; inputNode < numInputs; inputNode++) {
                        // adds the cost / weight partial derivative to the weight cost gradient for that connection
                        costGradientWeight[inputNode,outputNode] += nodeValues[outputNode] * layerData.inputs[inputNode];
                    }
                    costGradientBias[outputNode] += nodeValues[outputNode];
                }
            }
            lock (costGradientBias) {
                for (int outputNode = 0; outputNode < numOutputs; outputNode++) {
                    costGradientBias[outputNode] += nodeValues[outputNode];
                }
            }
        } 

        // used in backpropagation
        // used by the output layer to calculate the 'node values'
        // run by Backpropagate(...)
        // the partial derivative of the cost, with respect to the activation of the output
        // times the partial derivative of the activation with respect to the weight
        public double[] CalculateOutputLayerNodeValues(double[] expectedValues, LayerData layerData) {
            double[] nodeValues = new double[expectedValues.Length];

            for (int i = 0; i < expectedValues.Length; i++) {
                double nodeCostDerivative = NodeCostDerivative(layerData.activationValues[i],expectedValues[i]);
                double activationDerivative = ActivationDerivative(layerData.weightedInputs[i]);

                nodeValues[i] = activationDerivative * nodeCostDerivative;
            }

            return nodeValues;
        }

        // used in backpropagation
        // node values of the hidden layers
        // uses the node values of the layer before it
        public double[] CalculateHiddenLayerNodeValues(Layer oldLayer, LayerData layerData, double[] oldNodeValues) {
            double [] nodeValues = new double[numOutputs];

            for (int newNode = 0; newNode < numOutputs; newNode++) {
                double nodeValue = 0;

                for (int oldNode = 0; oldNode < oldNodeValues.Length; oldNode++) {
                    double weightedInputDerivative = oldLayer.weights[newNode,oldNode];
                    nodeValue += weightedInputDerivative * oldNodeValues[oldNode];
                }
                // Console.WriteLine(inputs.Length + " " + newNode);
                nodeValue *= ActivationDerivative(layerData.weightedInputs[newNode]);
                nodeValues[newNode] = nodeValue;
            }


            return nodeValues;
        }
    }
}