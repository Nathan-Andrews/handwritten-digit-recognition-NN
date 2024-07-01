using OpenTK;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using System;
using System.IO;
using System.Threading;


namespace simple_network {
    public class Visualize : GameWindow
    {
        private int _boundryShaderProgram;
        private int _pointShaderProgram;
        private int _boundryVertexArrayObject;
        private int _boundryTexture;
        private int _pointVertexArrayObject;
        private int _pointVertexBufferObject;
        private Vector2[] _pointsArray = new Vector2[100];
        private int[] _pointsClasses = new int[100];
        private float _pointRadius = 0.02f;
        private int _pointCount = 0;

        public Network network = new Network(2,3,2);
        public HashSet<DataPoint>? _dataPoints;
        private bool _continueTraining = true;
        private object _lock = new object();
        private int _epochCounter = 0;
        private Thread _trainingThread;

        public Visualize(int width, int height, string title) : base(GameWindowSettings.Default, new NativeWindowSettings() { ClientSize = new Vector2i(width, height), Title = title })
        {
        }

        protected override void OnLoad()
        {
            base.OnLoad();

            // Load and compile shaders
            _boundryShaderProgram = CreateShaderProgram("boundry_vertex_shader.glsl", "boundry_fragment_shader.glsl");
            _pointShaderProgram = CreateShaderProgram("point_vertex_shader.glsl", "point_fragment_shader.glsl");

            // Set up boundry vertex data (covering the entire screen)
            float[] boundryVertices = {
                -1.0f, -1.0f, 0.0f,
                1.0f, -1.0f, 0.0f,
                -1.0f,  1.0f, 0.0f,
                1.0f,  1.0f, 0.0f,
            };

            // Create and bind VAO for boundry
            _boundryVertexArrayObject = GL.GenVertexArray();
            GL.BindVertexArray(_boundryVertexArrayObject);

            int boundryVbo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, boundryVbo);
            GL.BufferData(BufferTarget.ArrayBuffer, boundryVertices.Length * sizeof(float), boundryVertices, BufferUsageHint.StaticDraw);

            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            // Set up point vertex data (just a single point, we'll expand it in the shader)
            float[] pointVertices = {
                -1.0f, -1.0f,
                1.0f, -1.0f,
                -1.0f,  1.0f,
                1.0f,  1.0f,
            };

            // Create and bind VAO for point
            _pointVertexArrayObject = GL.GenVertexArray();
            GL.BindVertexArray(_pointVertexArrayObject);

            _pointVertexBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _pointVertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, pointVertices.Length * sizeof(float), pointVertices, BufferUsageHint.StaticDraw);

            GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 2 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            GL.BindVertexArray(0);

            _boundryTexture = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, _boundryTexture);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

            // Compute and update texture with decision boundary
            UpdateDecisionBoundaryTexture();

            // Unbind texture
            GL.BindTexture(TextureTarget.Texture2D, 0);

            // Set clear color
            GL.ClearColor(Color4.CornflowerBlue);

            // Start training thread
            _trainingThread = new Thread(TrainNeuralNetwork);
            _trainingThread.Start();

        }

        public void AddPoints(HashSet<DataPoint> dataPoints) {
            _dataPoints = dataPoints;

            int i = 0;
            foreach (DataPoint point in dataPoints) {
                if (i >= 100) break; // adds max

                _pointsArray[i] = point.getPointAsCoordinates();
    
                _pointsClasses[i] = point.label;

                i++;
            }

            _pointCount = i;
        }

        private void UpdateDecisionBoundaryTexture()
        {
            Vector3 redColor = new(0.98f, 0.6f, 0.592f);
            Vector3 blueColor = new(0.659f, 0.769f, 0.988f);

            int width = 800;
            int height = 800;
            float[] data = new float[width * height * 3];

            Network networkCopy;

            lock (_lock) {
                networkCopy = new Network(network);
            }
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    // double inputX = (double)x / width * 2.0f - 1.0f;
                    // double inputY = (double)y / height * 2.0f - 1.0f;
                    double[] input = new double[2];
                    input[0] = (double)x / width;
                    input[1] = (double)y / height;
                    int classification = networkCopy.Classify(input);

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

            // Clear the screen
            GL.Clear(ClearBufferMask.ColorBufferBit);

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
                lock (_lock)
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

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);
            GL.Viewport(0, 0, ClientSize.X, ClientSize.Y);
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

        private int CreateShaderProgram(string vertexPath, string fragmentPath)
        {
            string vertexShaderSource = File.ReadAllText(vertexPath);
            string fragmentShaderSource = File.ReadAllText(fragmentPath);

            int vertexShader = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vertexShader, vertexShaderSource);
            GL.CompileShader(vertexShader);
            CheckShaderCompileStatus(vertexShader);

            int fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fragmentShader, fragmentShaderSource);
            GL.CompileShader(fragmentShader);
            CheckShaderCompileStatus(fragmentShader);

            int shaderProgram = GL.CreateProgram();
            GL.AttachShader(shaderProgram, vertexShader);
            GL.AttachShader(shaderProgram, fragmentShader);
            GL.LinkProgram(shaderProgram);
            CheckProgramLinkStatus(shaderProgram);

            GL.DeleteShader(vertexShader);
            GL.DeleteShader(fragmentShader);

            return shaderProgram;
        }

        private void CheckShaderCompileStatus(int shader)
        {
            GL.GetShader(shader, ShaderParameter.CompileStatus, out int success);
            if (success == 0)
            {
                string infoLog = GL.GetShaderInfoLog(shader);
                throw new Exception($"Error compiling shader: {infoLog}");
            }
        }

        private void CheckProgramLinkStatus(int program)
        {
            GL.GetProgram(program, GetProgramParameterName.LinkStatus, out int success);
            if (success == 0)
            {
                string infoLog = GL.GetProgramInfoLog(program);
                throw new Exception($"Error linking program: {infoLog}");
            }
        }

        public static void Render()
        {
            using (var window = new Visualize(800, 600, "points on boundry Background"))
            {
                window.Run();
            }
        }
    }
}