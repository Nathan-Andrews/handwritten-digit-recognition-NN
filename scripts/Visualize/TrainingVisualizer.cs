using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using DigitRecognition.NeuralNetwork;
using DigitRecognition.DataHandling;


namespace DigitRecognition.Visualization {
    public class TrainingVisualizer : Visualize
    {
        private int _boundryShaderProgram;
        private int _pointShaderProgram;
        private int _boundryVertexArrayObject;
        private int _boundryTexture;
        private int _pointVertexArrayObject;
        private readonly Vector2[] _pointsArray = new Vector2[100];
        private readonly int[] _pointsClasses = new int[100];
        private readonly float _pointRadius = 0.02f;
        private int _pointCount = 0;

        public Network network;
        public DataSet? _dataPoints;
        private bool _continueTraining = true;
        // private object _lock = new object();
        private int _epochCounter = 0;
        private readonly Thread _trainingThread;

        public TrainingVisualizer(int width, int height, string title, int[] layerSizes) : base(width, height, title) {
            network = new(layerSizes);

            _trainingThread = new Thread(TrainNeuralNetwork);
        }

        protected override void OnLoad() {
            base.OnLoad();

            // Load and compile shaders
            _boundryShaderProgram = CreateShaderProgram("scripts/Visualize/Shaders/texture_vertex_shader.glsl", "scripts/Visualize/Shaders/texture_fragment_shader.glsl");
            _pointShaderProgram = CreateShaderProgram("scripts/Visualize/Shaders/point_vertex_shader.glsl", "scripts/Visualize/Shaders/point_fragment_shader.glsl");

            _boundryVertexArrayObject = CreateAndBindVAO();

            _pointVertexArrayObject = CreateAndBindVAO();

            _boundryTexture = CreateAndBindTexture();

            UpdateDecisionBoundaryTexture();

            UnbindTexture();

            SetClearColor();

            // Start training thread
            // _trainingThread.Start();

        }

        public void AddPoints(DataSet dataPoints) {
            _dataPoints = dataPoints;

            for (int i = 0; i < dataPoints.size; i++) {
                if (i >= 100) break; // adds max

                _pointsArray[i] = dataPoints.GetElement(i).GetPointAsCoordinates();
    
                _pointsClasses[i] = dataPoints.GetElement(i).label;
            }

            _pointCount = dataPoints.size;
        }

        private void UpdateDecisionBoundaryTexture()
        {
            Vector3 redColor = new(0.98f, 0.6f, 0.592f);
            Vector3 blueColor = new(0.659f, 0.769f, 0.988f);

            int width = 800;
            int height = 800;
            float[] data = new float[width * height * 3];

            // Network networkCopy;

            // lock (_lock) {
            //     networkCopy = new Network(network);
            // }
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    // double inputX = (double)x / width * 2.0f - 1.0f;
                    // double inputY = (double)y / height * 2.0f - 1.0f;
                    double[] input = new double[2];
                    input[0] = (double)x / width;
                    input[1] = (double)y / height;
                    int classification = network.Classify(input);

                    int index = (y * width + x) * 3;
                    if (classification == 0)
                    {
                        data[index] = blueColor.X;
                        data[index + 1] = blueColor.Y;
                        data[index + 2] = blueColor.Z;
                    }
                    else
                    {
                        data[index] = redColor.X;
                        data[index + 1] = redColor.Y;
                        data[index + 2] = redColor.Z;
                    }
                }
            }

            GL.BindTexture(TextureTarget.Texture2D, _boundryTexture);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb, width, height, 0, PixelFormat.Rgb, PixelType.Float, data);
            GL.BindTexture(TextureTarget.Texture2D, 0);
        }

        private void UpdateUniforms () {
            // Set point uniform variables
            int centerLocation = GL.GetUniformLocation(_pointShaderProgram, "pointCenters");
            GL.Uniform2(centerLocation, _pointsArray.Length, ref _pointsArray[0].X);

            int radiusLocation = GL.GetUniformLocation(_pointShaderProgram, "pointRadius");
            GL.Uniform1(radiusLocation, _pointRadius);

            int classLocation = GL.GetUniformLocation(_pointShaderProgram,"pointClasses");
            GL.Uniform1(classLocation,_pointsClasses.Length,_pointsClasses);

            int countLocation = GL.GetUniformLocation(_pointShaderProgram,"pointCount");
            GL.Uniform1(countLocation,_pointCount);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            ClearScreen();

            UpdateDecisionBoundaryTexture();

            // Render boundry background
            GL.UseProgram(_boundryShaderProgram);
            GL.BindVertexArray(_boundryVertexArrayObject);
            GL.BindTexture(TextureTarget.Texture2D,_boundryTexture);
            GL.DrawArrays(PrimitiveType.TriangleStrip, 0, 4);

            // Render points on top of the boundry background
            GL.UseProgram(_pointShaderProgram);
            GL.BindVertexArray(_pointVertexArrayObject);

            UpdateUniforms();

            GL.DrawArrays(PrimitiveType.TriangleStrip, 0, 4);

            SwapBuffers();
        }

        private void TrainNeuralNetwork()
        {
            while (_continueTraining)
            {
                // lock (_lock)
                {
                    if (_dataPoints != null) network.Fit(_dataPoints,0.1);
                    _epochCounter++;
                }

                // Update the decision boundary texture occasionally to reduce CPU usage
                // if (_epochCounter % 10 == 0)
                // {
                //     UpdateDecisionBoundaryTexture();
                // }
            }
        }

        protected override void KeyPressedSpace() {
            if (!_trainingThread.IsAlive) {
                Console.WriteLine("Training Started");
                _trainingThread.Start();
            }
        }

        protected override void OnUnload()
        {
            base.OnUnload();
            _continueTraining = false;
            _trainingThread.Join();
            GL.DeleteVertexArray(_boundryVertexArrayObject);
            GL.DeleteVertexArray(_pointVertexArrayObject);
            GL.DeleteProgram(_boundryShaderProgram);
            GL.DeleteProgram(_pointShaderProgram);
        }
    }
}