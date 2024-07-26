using Microsoft.Extensions.Configuration;

namespace simple_network {
    class TrainingLayerConfig {
        public List<int>? LayerSizes { get; set; }
    }

    struct Config {

        public TrainingConfig Training;
        public ImageClassificationConfig ImageClassification;

        public Config() {
            Training = new();
            ImageClassification = new();
        }

        public static string Get(string jsonPath) {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("config.json", optional: true, reloadOnChange: true);

            IConfiguration configuration = builder.Build();
            
            // null-coalescing operator // I love this c# feature
            // https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/operators/null-coalescing-operator
            return configuration[$"Program:{jsonPath}"] ?? "";
        }

        public static bool Check(string jsonPath) {
            return Get(jsonPath) == "True";
        }

        public static void PrintConfigIssue(string message) {
            Console.WriteLine($"Config Issue: {message}");
        }
    }

    struct ImageClassificationConfig {
        public bool DoClassification;
        public string StoredNetworkFile;
        public bool DoDrawingMode;
        public bool DoDatasetAccuracyCheck;
        public string Dataset;

        public ImageClassificationConfig() {
            DoClassification = Check("DoClassification");
            StoredNetworkFile = Get("StoredNetworkFile");
            DoDrawingMode = Check("DoDrawingMode");
            DoDatasetAccuracyCheck = Check("DatasetAccuracyCheck:DoDatasetAccuracyCheck");
            Dataset = Get("DatasetAccuracyCheck:Dataset");
        }

        bool Check(string jsonPath) {
            return Config.Check($"ImageClassification:{jsonPath}");
        }

        string Get(string jsonPath) {
            return Config.Get($"ImageClassification:{jsonPath}");
        }
    }

    struct TrainingConfig {
        public bool DoTraining;
        public bool IsImage;
        public bool IsCSVDataset;
        public int[] LayerSizes;
        public int MiniBatchSize;
        public double LearningRate;
        public string TrainingSetPath;
        public bool DoAccuracyCheck;
        public string TestingSetPath;
        public bool DoNetworkFile;
        public string NetworkFile;
        public bool DoOverwrite;
        public bool DoImagePreview;
        public int ImagePreviewCount;

        public TrainingConfig() {
            DoTraining = Check("DoTraining");
            IsImage = Check("IsImage");
            IsCSVDataset = Check("IsCSVDataset");
            LayerSizes = GetArray("Hyperparameters");
            MiniBatchSize = int.Parse(Get("Hyperparameters:MiniBatchSize"));
            LearningRate = double.Parse(Get("Hyperparameters:LearningRate"));
            TrainingSetPath = Get("Dataset:TrainingSetPath");
            DoAccuracyCheck = Check("Dataset:DoAccuracyCheck");
            TestingSetPath = Get("Dataset:TestingSetPath");
            DoNetworkFile = Check("Storage:DoNetworkFile");
            NetworkFile = Get("Storage:NetworkFile");
            DoOverwrite = Check("Storage:DoOverwrite");
            DoImagePreview = Check("Dataset:DoImagePreview");
            ImagePreviewCount = int.Parse(Get("Dataset:ImagePreviewCount"));
        }

        bool Check(string jsonPath) {
            return Config.Check($"Training:{jsonPath}");
        }

        string Get(string jsonPath) {
            return Config.Get($"Training:{jsonPath}");
        }

        static int[] GetArray(string jsonPath) {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("config.json", optional: true, reloadOnChange: true);

            IConfiguration configuration = builder.Build();

            var trainingConfig = new TrainingLayerConfig();
            configuration.GetSection($"Program:Training:{jsonPath}").Bind(trainingConfig);

            return (trainingConfig.LayerSizes ?? new()).ToArray();
        }
    }
}