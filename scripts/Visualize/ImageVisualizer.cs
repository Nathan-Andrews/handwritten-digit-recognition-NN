using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System;
using DigitRecognition.ImageHandling;

namespace DigitRecognition.Visualization {
    public class ImageVisualizer : Visualize {
        private int _shaderProgram;
        private int _vertexArrayObject;
        private ImageSet _imageSet;

        private int _renderedDigitIndex = 0;
        private ImageHandling.Image _currentDigit;


        public ImageVisualizer(int width, int height, string title, ImageSet imageSet) : base(width, height,title)
        {
            this._imageSet = imageSet;
            this._currentDigit = imageSet.images[_renderedDigitIndex];
        }

        protected override void OnLoad() {
            base.OnLoad();

            _shaderProgram = CreateShaderProgram("scripts/Visualize/Shaders/digit_vertex_shader.glsl", "scripts/Visualize/Shaders/digit_fragment_shader.glsl");

            _vertexArrayObject = CreateAndBindVAO();
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            ClearScreen();

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

        protected override void OnUnload() {
            base.OnUnload();
            GL.DeleteVertexArray(_vertexArrayObject);
            GL.DeleteProgram(_shaderProgram);
        }

        protected override void KeyPressedEnter()
        {
            _renderedDigitIndex++;
            if(_renderedDigitIndex >= _imageSet.images.Length - 1) {
                Close();
            }

            _currentDigit = _imageSet.images[_renderedDigitIndex];

            Console.WriteLine(_currentDigit.digit);
        }

        protected override void KeyPressedSpace()
        {
            _currentDigit = ImageProcessor.RandomizeImage(_imageSet.images[_renderedDigitIndex]);
        }
    }
}