using Microsoft.Extensions.Configuration;

namespace DigitRecognition {
    class TrainingLayerConfig {
        public List<int>? LayerSizes { get; set; }
    }

    struct Config {

        public TrainingConfig Training;
        public ImageClassificationConfig ImageClassification;
        public DatasetConfig Dataset;

        public Config() {
            Training = new();
            ImageClassification = new();
            Dataset = new();
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
        public bool DoAccuracyCheck;

        public ImageClassificationConfig() {
            DoClassification = Check("DoClassification");
            StoredNetworkFile = Get("StoredNetworkFile");
            DoDrawingMode = Check("DoDrawingMode");
            DoAccuracyCheck = Check("DoAccuracyCheck");
        }

        static bool Check(string jsonPath) {
            return Config.Check($"ImageClassification:{jsonPath}");
        }

        static string Get(string jsonPath) {
            return Config.Get($"ImageClassification:{jsonPath}");
        }
    }

    struct TrainingConfig {
        public bool DoTraining;
        public int[] LayerSizes;
        public int MiniBatchSize;
        public double LearningRate;
        public bool DoNetworkFile;
        public string NetworkFile;
        public bool DoOverwrite;
        public bool DoAccuracyCheck;

        public TrainingConfig() {
            DoTraining = Check("DoTraining");
            LayerSizes = GetArray("Hyperparameters");
            MiniBatchSize = int.Parse(Get("Hyperparameters:MiniBatchSize"));
            LearningRate = double.Parse(Get("Hyperparameters:LearningRate"));
            DoNetworkFile = Check("Storage:DoNetworkFile");
            NetworkFile = Get("Storage:NetworkFile");
            DoOverwrite = Check("Storage:DoOverwrite");
            DoAccuracyCheck = Check("DoAccuracyCheck");
        }

        static bool Check(string jsonPath) {
            return Config.Check($"Training:{jsonPath}");
        }

        static string Get(string jsonPath) {
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

    struct DatasetConfig {
        public string TrainingSetPath;
        public string TestingSetPath;
        public bool IsImage;
        public bool IsCSV;
        public bool DoImagePreview;
        public int ImagePreviewCount;

        public DatasetConfig() {
            TrainingSetPath = Get("TrainingSetPath");
            TestingSetPath = Get("TestingSetPath");
            IsImage = Check("Format:IsImage");
            IsCSV = Check("Format:IsCSV");
            DoImagePreview = Check("Format:DoImagePreview");
            ImagePreviewCount = int.Parse(Get("Format:ImagePreviewCount"));
        }

        static bool Check(string jsonPath) {
            return Config.Check($"Dataset:{jsonPath}");
        }

        static string Get(string jsonPath) {
            return Config.Get($"Dataset:{jsonPath}");
        }
    }
}