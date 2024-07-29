using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using DigitRecognition.NeuralNetwork;
using DigitRecognition.ImageHandling;

namespace DigitRecognition.Visualization {
    public class DrawingVisualizer : Visualize {
        private int _textureShaderProgram;
        private int _textureVertexArrayObject;
        private int _drawingTexture;

        private ImageHandling.Image _drawnDigit;
        private ImageHandling.Image _digit;

        private bool _mouseDown = false;
        private float _lastCursorX = 0;
        private float _lastCursorY = 0;

        private readonly Thread _cursorThread;
        private bool _runThread = true;

        private HashSet<Vector2> _cursorPositionBuffer;

        private readonly object _lock = new();

        public Network? _network;
        private static readonly string[] spelledDigits = {"Zero","One","Two","Three","Four","Five","Six","Seven","Eight","Nine"};

        public DrawingVisualizer(int width, string title) : base(width, width, title)
        {
            _digit = new(28);
            _drawnDigit = new(width*width);
            _cursorPositionBuffer = new HashSet<Vector2>();
            _cursorThread = new Thread(CursorThread);
        }

        protected override void OnLoad()
        {
            base.OnLoad();

            // Load and compile shaders
            _textureShaderProgram = CreateShaderProgram("scripts/Visualize/Shaders/texture_vertex_shader.glsl", "scripts/Visualize/Shaders/texture_fragment_shader.glsl");

            _textureVertexArrayObject = CreateAndBindVAO();

            _drawingTexture = CreateAndBindTexture();

            // Compute and update texture with decision boundary
            UpdateTexture();

            UnbindTexture();

            SetClearColor();

            _cursorThread.Start();
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            HandleMouse();

            ClearScreen();

            UpdateTexture();

            GL.UseProgram(_textureShaderProgram);
            GL.BindVertexArray(_textureVertexArrayObject);
            GL.BindTexture(TextureTarget.Texture2D,_drawingTexture);
            GL.DrawArrays(PrimitiveType.TriangleStrip, 0, 4);


            SwapBuffers();
        }

        private void HandleMouse() {
            lock (_lock) {
                if (_cursorPositionBuffer.Count <= 0) {
                    _mouseDown = false;
                }
                foreach (Vector2 cursorPos in _cursorPositionBuffer) {

                    // Calculate the cursor position relative to the window
                    float cursorX = cursorPos.X;
                    float cursorY = cursorPos.Y;

                    // Console.WriteLine(cursorY);

                    CursorPaint(cursorX, -(cursorY - 800));

                    // if (cursorX > 0 && cursorY > 0 && cursorX < 800 && cursorY < 800 && MouseState.IsAnyButtonDown) {
                    //     CursorPaint(cursorX, -(cursorY - 800));

                    _mouseDown = true;
                    _lastCursorX = cursorX;
                    _lastCursorY = -(cursorY - 800);
                }
                _cursorPositionBuffer = new HashSet<Vector2>();
            }
        }

        private void CursorThread() {
            while (_runThread) {
                lock (_lock) {
                    Vector2 cursorPos = MouseState.Position;
                    if (cursorPos.X > 0 && cursorPos.Y > 0 && cursorPos.X < 800 && cursorPos.Y < 800 && MouseState.IsAnyButtonDown) _cursorPositionBuffer.Add(cursorPos);
                }
                // Thread.Sleep(1);
            }
        }

        private void CursorPaint(double cursorX, double cursorY) {
            if (_mouseDown) {
                DrawInterpolate(cursorX, cursorY);
            }
            else {
                DrawCircle(cursorX,cursorY);
            }
        }

        private void DrawCircle(double cursorX, double cursorY) {
            int _radius = 15;

            int x0 = (int) Math.Max(0,Math.Floor(cursorX - _radius));
            int y0 = (int) Math.Max(0,Math.Floor(cursorY - _radius));
            int xMax = Math.Min(800, x0 + 2 * _radius);
            int yMax = Math.Min(800, y0 + 2 * _radius);

            for (int x = x0; x < xMax; x++) {
                for (int y = y0; y < yMax; y++) {
                    double distance = Math.Sqrt((cursorX - x) * (cursorX - x) + (cursorY - y) * (cursorY - y));
                    
                    if (distance < _radius) {
                        _drawnDigit.pixels[_drawnDigit.GetIndex2d(x, y)] = 1;
                    }
                }
            }
        }

        private void DrawInterpolate(double cursorX, double cursorY) {
            // int _steps = 40;
            int _steps = (int) Math.Ceiling(Math.Sqrt((cursorX - _lastCursorX) * (cursorX - _lastCursorX) + (cursorY - _lastCursorY) * (cursorY - _lastCursorY)));

            double dx = -(cursorX - _lastCursorX) / (_steps - 1);
            double dy = -(cursorY - _lastCursorY) / (_steps - 1);

            for (int i = 0; i < _steps;i++) {
                DrawCircle(cursorX + i * dx, cursorY + i * dy);
            }
         }
        private void Classify() {
            double [] output = _network?.GetOutputs(_digit.pixels) ?? Array.Empty<double>();
            
            int maxIndex = 0;
            double sum = 0;

            for (int i = 0; i < output.Length; i++) {
                if (output[i] > output[maxIndex]) maxIndex = i;
                sum += output[i];
            }

            Console.Write('|');

            for (int i = 0; i < output.Length; i++) {
                if (i == maxIndex) {
                    ConsoleColor originalForegroundColor = Console.ForegroundColor;

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write($" \x1b[1m{spelledDigits[i]} {Math.Round(output[i] * 100 / sum)}%\x1b[0m |");

                    Console.ForegroundColor = originalForegroundColor;
                }
                else {
                    Console.Write($" {spelledDigits[i]} {Math.Round(output[i] * 100 / sum)}% |");
                }
            }
            Console.SetCursorPosition(0, Console.CursorTop);
        }

        private void UpdateTexture() {
            _digit = ImageProcessor.Scale(ImageProcessor.Downsize(_drawnDigit,784),1.1);

            Classify();

            int width = 800;
            // int width = 28;
            float[] data = new float[width * width * 3];

            for (int y = 0; y < width; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    float grayscale = (float) _drawnDigit.pixels[_drawnDigit.GetIndex2d(x,y)];
                    
                    int index = (y * width + x) * 3;
                    data[index] = grayscale;
                    data[index + 1] = grayscale;
                    data[index + 2] = grayscale;
                }
            }

            GL.BindTexture(TextureTarget.Texture2D, _drawingTexture);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb, width, width, 0, PixelFormat.Rgb, PixelType.Float, data);
            // GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            // GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);

            GL.BindTexture(TextureTarget.Texture2D, 0);
        }

        protected override void OnUnload() {
            base.OnUnload();
            GL.DeleteVertexArray(_textureVertexArrayObject);
            GL.DeleteProgram(_textureShaderProgram);

            _runThread = false;
            _cursorThread.Join();

        }

        protected override void KeyPressedSpace()
        {
            // clear the canvas
            _drawnDigit = new(_drawnDigit.size);
        }
    }
}