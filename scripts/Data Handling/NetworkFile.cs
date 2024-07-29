using System.Text;
using DigitRecognition.NeuralNetwork;

namespace DigitRecognition.DataHandling {
    public static class NetworkFile {
        // network storage format
        // This format describes how a neural network is stored in a file.
        // [offset]         [type]           [description]
        // 0000             32 bit integer   number of layers (n)
        // 0004             32 bit integer   number of nodes in layer 1
        // 0008             32 bit integer   number of nodes in layer 2
        // .................
        // (0004*(n))       32 bit integer   number of nodes in layer (n)
        // (0004*(n)) + 4   64 bit float     layer[0].weight[0,0]
        // (0004*(n)) + 12  64 bit float     layer[0].weight[0,1]
        // ..............
        // xxxx             64 bit float     layer[0].weight[i,j]
        // xxxx             64 bit float     layer[0].bias[0]
        // ..............
        // xxxx             64 bit float     layer[0].bias[j]
        // ..............
        // xxxx             64 bit float     layer[n].bias[j]
        // layers are stored input-wise, increasing.
        // First weights for the current layer are stored are stored, then biases for the current layer
        //  // for each layer
        //  //  for (inputNode)
        //  //   for (outputNode)
        //  //       write(weight[inputNode,outputNode])
        //  //  for (outputNode)
        //  //   write(bias[outputNode])
        public static void SaveNetwork(Network network, string filename, bool overwrite = false) {
            string directory = "./data/stored networks/";

            if (!overwrite) {
                string extension = Path.GetExtension(filename);

                string newFilename = Path.GetFileNameWithoutExtension(filename);
                int counter = 2;
                while (File.Exists(Path.Combine(directory, newFilename+extension)))
                {
                    newFilename = $"{newFilename}{counter}";
                    counter++;
                }

                filename = newFilename+extension;
            }

            filename = Path.Combine(directory,filename);

            using (FileStream fs = new FileStream(filename, FileMode.Create, FileAccess.Write))
            {
                using (BinaryWriter writer = new BinaryWriter(fs))
                {
                    writer.Write(network.layers.Length + 1);
                    for(int i = 0; i < network.layers.Length; i++) {
                        writer.Write(network.layers[i].numInputs);
                    }
                    writer.Write(network.layers[network.layers.Length - 1].numOutputs);
                    
                    foreach (Layer layer in network.layers) {
                        for (int inputNode = 0; inputNode < layer.numInputs; inputNode++) {
                            for (int outputNode = 0; outputNode < layer.numOutputs; outputNode++) {
                                writer.Write(layer.weights[inputNode,outputNode]);
                            }
                        }

                        for (int outputNode = 0; outputNode < layer.numOutputs; outputNode++) {
                            writer.Write(layer.biases[outputNode]);
                        }
                    }
                }
            }
        }

        public static Network LoadNetwork(string filename) {
            Network network;

            string directory = "./data/stored networks/";

            filename = $"{directory}{filename}";

            Console.WriteLine($"Loading pretrained network file: {filename}");

            using (FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read)) {
                using (BinaryReader reader = new BinaryReader(fs))
                {
                    int numLayers = reader.ReadInt32();
                    int[] layerSizes = new int[numLayers];

                    Console.Write("... Layer sizes:");
                    for (int layer = 0; layer < numLayers; layer++) {
                        layerSizes[layer] = reader.ReadInt32();
                        Console.Write($" {layerSizes[layer]}");
                    }
                    Console.WriteLine("");
                    network = new Network(layerSizes);

                    foreach (Layer layer in network.layers) {
                        for (int inputNode = 0; inputNode < layer.numInputs; inputNode++) {
                            for (int outputNode = 0; outputNode < layer.numOutputs; outputNode++) {
                                layer.weights[inputNode,outputNode] = reader.ReadDouble();
                            }
                        }

                        for (int outputNode = 0; outputNode < layer.numOutputs; outputNode++) {
                            layer.biases[outputNode] = reader.ReadDouble();
                        }
                    }
                }
            }

            return network;
        }
    }

}