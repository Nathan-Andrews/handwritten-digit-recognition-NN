using OpenTK;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using System;
using System.IO;

public class Visualize : GameWindow
{
    private int _boundryShaderProgram;
    private int _pointShaderProgram;
    private int _boundryVertexArrayObject;
    private int _pointVertexArrayObject;
    private int _pointVertexBufferObject;
    // private Vector2 _pointCenter = new Vector2(0.4f, 0.1f);
    private Vector2[] _pointsArray = new Vector2[10];
    private int[] _pointsClasses = new int[10];
    private float _pointRadius = 0.02f;

    // private float[,] _boundryTexture = 

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

        _pointsArray[0] = new Vector2(-0.5f, 0.5f);
        _pointsArray[1] = new Vector2(0.5f, 0.5f);
        _pointsArray[2] = new Vector2(0.0f, -0.5f);

        _pointsClasses[0] = 1;
        _pointsClasses[1] = 1;
        _pointsClasses[2] = 0;

        // Set clear color
        GL.ClearColor(Color4.CornflowerBlue);
    }

    public void addPoints(HashSet<DataPoint> dataPoints) {
        int i = 0;
        foreach (DataPoint point in dataPoints) {
            if (i >= 10) break;

            _pointsArray[i] = point.getPointAsCoordinates();
 
            _pointsClasses[i] = point.label;

            i++;
        }
    }

    private void updateUniforms () {
        // Set point uniform variables
        int centerLocation = GL.GetUniformLocation(_pointShaderProgram, "pointCenters");
        GL.Uniform2(centerLocation, _pointsArray.Length, ref _pointsArray[0].X);

        int radiusLocation = GL.GetUniformLocation(_pointShaderProgram, "pointRadius");
        GL.Uniform1(radiusLocation, _pointRadius);

        int classLocation = GL.GetUniformLocation(_pointShaderProgram,"pointClasses");
        GL.Uniform1(classLocation,_pointsClasses.Length,_pointsClasses);
    }

    protected override void OnRenderFrame(FrameEventArgs e)
    {
        base.OnRenderFrame(e);

        // Clear the screen
        GL.Clear(ClearBufferMask.ColorBufferBit);

        // Render boundry background
        GL.UseProgram(_boundryShaderProgram);
        GL.BindVertexArray(_boundryVertexArrayObject);
        GL.DrawArrays(PrimitiveType.TriangleStrip, 0, 4);

        // Render points on top of the boundry background
        GL.UseProgram(_pointShaderProgram);
        GL.BindVertexArray(_pointVertexArrayObject);

        updateUniforms();


        GL.DrawArrays(PrimitiveType.TriangleStrip, 0, 4);

        SwapBuffers();
    }

    protected override void OnResize(ResizeEventArgs e)
    {
        base.OnResize(e);
        GL.Viewport(0, 0, ClientSize.X, ClientSize.Y);
    }

    protected override void OnUnload()
    {
        base.OnUnload();
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