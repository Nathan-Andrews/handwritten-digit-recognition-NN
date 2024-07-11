using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System;

namespace simple_network {
    public class ImageVisualizer : GameWindow {
        private int _shaderProgram;
        private int _vertexArrayObject;
        private int _vertexBufferObject;
        private ImageSet _imageSet;

        private int _renderedDigitIndex = 0;
        private Image _currentDigit;


        public ImageVisualizer(int width, int height, string title, ImageSet imageSet) : base(GameWindowSettings.Default, new NativeWindowSettings() { ClientSize = new Vector2i(width, height), Title = title })
        {
            this._imageSet = imageSet;
            this._currentDigit = imageSet.images[_renderedDigitIndex];
        }

        protected override void OnLoad() {
            base.OnLoad();

            // load keypress detection
            this.KeyDown += OnKeyDown;
            this.KeyUp += OnKeyUp;

            float[] vertices = {
                -1.0f, -1.0f,
                1.0f, -1.0f,
                -1.0f,  1.0f,
                1.0f,  1.0f,
            };

            _shaderProgram = CreateShaderProgram("Visualize/digit_vertex_shader.glsl", "Visualize/digit_fragment_shader.glsl");

            // Create and bind VAO for point
            _vertexArrayObject = GL.GenVertexArray();
            GL.BindVertexArray(_vertexArrayObject);

            _vertexBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

            GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 2 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            GL.BindVertexArray(0);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            // Clear the screen
            GL.Clear(ClearBufferMask.ColorBufferBit);

            // Render boundry background

            // Render points on top of the boundry background
            GL.UseProgram(_shaderProgram);
            GL.BindVertexArray(_vertexArrayObject);

            UpdateUniforms();

            GL.DrawArrays(PrimitiveType.TriangleStrip, 0, 4);

            SwapBuffers();
        }

        private void UpdateUniforms () {
            float[] pixels = new float[784]; 
            Parallel.For(0,784, (i) => {
                pixels[i] = (float)_currentDigit.pixels[i];
            });

            int pixelsLocation = GL.GetUniformLocation(_shaderProgram,"pixels");
            GL.Uniform1(pixelsLocation,pixels.Length,pixels);
        }

        protected override void OnResize(ResizeEventArgs e) {
            base.OnResize(e);
            GL.Viewport(0, 0, ClientSize.X, ClientSize.Y);
        }

        protected override void OnUnload() {
            base.OnUnload();
            GL.DeleteVertexArray(_vertexArrayObject);
            GL.DeleteProgram(_shaderProgram);
        }

        protected override void OnKeyDown(KeyboardKeyEventArgs e) {
            // base.OnKeyDown(e);

            if (e.Key == Keys.Escape)
            {
                Close(); // Close the window when the Escape key is pressed
            }
            else
            {
                // Console.WriteLine($"Key Down: {e.Key}");
                if (e.Key == Keys.Enter) {
                    _renderedDigitIndex++;
                    if(_renderedDigitIndex >= _imageSet.images.Length - 1) {
                        Close();
                    }

                    _currentDigit = _imageSet.images[_renderedDigitIndex];

                    Console.WriteLine(_currentDigit.digit);
                }
                if (e.Key == Keys.Space) {
                    _currentDigit = ImageProcessor.RandomizeImage(_imageSet.images[_renderedDigitIndex]);
                }
            }
        }

        protected override void OnKeyUp(KeyboardKeyEventArgs e) {
            // base.OnKeyUp(e);

            // Console.WriteLine($"Key Up: {e.Key}");
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