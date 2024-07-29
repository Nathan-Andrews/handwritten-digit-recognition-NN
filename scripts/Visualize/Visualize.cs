using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;


namespace DigitRecognition.Visualization {
    public class Visualize : GameWindow
    {

        protected readonly float[] vertices = {
            -1.0f, -1.0f, 0.0f,
            1.0f, -1.0f, 0.0f,
            -1.0f,  1.0f, 0.0f,
            1.0f,  1.0f, 0.0f,
        };

        public Visualize(int width, int height, string title) : base(GameWindowSettings.Default, new NativeWindowSettings() { ClientSize = new Vector2i(width, height), Title = title }) {}

        protected override void OnResize(ResizeEventArgs e) {
            base.OnResize(e);
            GL.Viewport(0, 0, ClientSize.X, ClientSize.Y);
        }

        protected override void OnUnload() {
            base.OnUnload();
        }

        protected int CreateAndBindVAO() {
            int vertexArrayObject = GL.GenVertexArray();

            GL.BindVertexArray(vertexArrayObject);

            int vertexBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            GL.BindVertexArray(0);

            return vertexArrayObject;
        }

        protected static int CreateAndBindTexture() {
            int texture = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, texture);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

            return texture;
        }

        protected static void SetClearColor() {
            GL.ClearColor(Color4.CornflowerBlue);
        }

        protected static void UnbindTexture() {
            GL.BindTexture(TextureTarget.Texture2D, 0);
        }

        protected static void ClearScreen() {
            GL.Clear(ClearBufferMask.ColorBufferBit);
        }

        protected static int CreateShaderProgram(string vertexPath, string fragmentPath) {
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

        protected static void CheckShaderCompileStatus(int shader) {
            GL.GetShader(shader, ShaderParameter.CompileStatus, out int success);
            if (success == 0)
            {
                string infoLog = GL.GetShaderInfoLog(shader);
                throw new Exception($"Error compiling shader: {infoLog}");
            }
        }

        protected static void CheckProgramLinkStatus(int program) {
            GL.GetProgram(program, GetProgramParameterName.LinkStatus, out int success);
            if (success == 0)
            {
                string infoLog = GL.GetProgramInfoLog(program);
                throw new Exception($"Error linking program: {infoLog}");
            }
        }

        protected override void OnKeyDown(KeyboardKeyEventArgs e) {
            if (e.Key == Keys.Escape)
            {
                KeyPressedEscape();
            }
            else if (e.Key == Keys.Space)
            {
                KeyPressedSpace();
            }
            else if (e.Key == Keys.Enter)
            {
                KeyPressedEnter();
            }
        }
        protected override void OnKeyUp(KeyboardKeyEventArgs e) {}

        protected virtual void KeyPressedEscape () {
            Close(); 
        }
        
        protected virtual void KeyPressedSpace () {}

        protected virtual void KeyPressedEnter () {}
    }
}