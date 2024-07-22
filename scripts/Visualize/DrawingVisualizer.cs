using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK;
using OpenTK.Input;
using OpenTK.Platform.Windows;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace simple_network {
    public class DrawingVisualizer : GameWindow {
        private int _textureShaderProgram;
        private int _textureVertexArrayObject;
        private int _drawingTexture;

        private Image _drawnDigit;
        private Image _digit;

        private bool _mouseDown = false;
        private float _lastCursorX = 0;
        private float _lastCursorY = 0;

        private Thread _cursorThread;
        private bool _runThread = true;

        private HashSet<Vector2> _cursorPositionBuffer;

        private object _lock = new();

        public DrawingVisualizer(int width, string title) : base(GameWindowSettings.Default, new NativeWindowSettings() { ClientSize = new Vector2i(width, width), Title = title})
        {
            _digit = new(28);
            _drawnDigit = new Image(width*width);
            _cursorPositionBuffer = new HashSet<Vector2>();
            _cursorThread = new Thread(CursorThread);
        }

        protected override void OnLoad()
        {
            base.OnLoad();

            // Load and compile shaders
            _textureShaderProgram = CreateShaderProgram("scripts/Visualize/Shaders/texture_vertex_shader.glsl", "scripts/Visualize/Shaders/texture_fragment_shader.glsl");

            float[] textureVertices = {
                -1.0f, -1.0f, 0.0f,
                1.0f, -1.0f, 0.0f,
                -1.0f,  1.0f, 0.0f,
                1.0f,  1.0f, 0.0f,
            };

            _textureVertexArrayObject = GL.GenVertexArray();
            GL.BindVertexArray(_textureVertexArrayObject);

            int vbo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, textureVertices.Length * sizeof(float), textureVertices, BufferUsageHint.StaticDraw);

            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            GL.BindVertexArray(0);

            _drawingTexture = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, _drawingTexture);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

            // Compute and update texture with decision boundary
            UpdateTexture();

            // Unbind texture
            GL.BindTexture(TextureTarget.Texture2D, 0);

            // Set clear color
            GL.ClearColor(Color4.CornflowerBlue);

            _cursorThread.Start();
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            HandleMouse();

            // Clear the screen
            GL.Clear(ClearBufferMask.ColorBufferBit);

            UpdateTexture();

            GL.UseProgram(_textureShaderProgram);
            GL.BindVertexArray(_textureVertexArrayObject);
            GL.BindTexture(TextureTarget.Texture2D,_drawingTexture);
            GL.DrawArrays(PrimitiveType.TriangleStrip, 0, 4);

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
            int _radius = 20;

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

        private void UpdateTexture() {
            // _digit = ImageProcessor.Downsize(_drawnDigit,784);

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

        protected override void OnResize(ResizeEventArgs e) {
            base.OnResize(e);
            GL.Viewport(0, 0, ClientSize.X, ClientSize.Y);
        }

        protected override void OnUnload() {
            base.OnUnload();
            GL.DeleteVertexArray(_textureVertexArrayObject);
            GL.DeleteProgram(_textureShaderProgram);

            _runThread = false;
            _cursorThread.Join();

        }

        protected override void OnKeyDown(KeyboardKeyEventArgs e) {
            // base.OnKeyDown(e);

            if (e.Key == Keys.Escape)
            {
                Close(); // Close the window when the Escape key is pressed
            }
            else
            {
                if (e.Key == Keys.Space) {
                    // _digit = ImageProcessor.Downsize(_drawnDigit,784);
                    // _digit.PrintImageAsAsciiArt();
                    _drawnDigit = new(_drawnDigit.size);
                }
            }
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

        private static void CheckShaderCompileStatus(int shader)
        {
            GL.GetShader(shader, ShaderParameter.CompileStatus, out int success);
            if (success == 0)
            {
                string infoLog = GL.GetShaderInfoLog(shader);
                throw new Exception($"Error compiling shader: {infoLog}");
            }
        }

        private static void CheckProgramLinkStatus(int program)
        {
            GL.GetProgram(program, GetProgramParameterName.LinkStatus, out int success);
            if (success == 0)
            {
                string infoLog = GL.GetProgramInfoLog(program);
                throw new Exception($"Error linking program: {infoLog}");
            }
        }
    }
}