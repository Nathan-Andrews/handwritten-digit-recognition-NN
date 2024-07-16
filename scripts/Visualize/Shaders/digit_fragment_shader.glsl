#version 330 core
in vec2 ourPosition;
out vec4 FragColor;
uniform float pixels[784];
int count = 784;

void main()
{
    int x = int(((ourPosition[0] + 1) / 2) * 28);
    int y = int(((-ourPosition[1] + 1) / 2) * 28);

    float grayscale = pixels[x + (y * 28)];

    FragColor = vec4(grayscale,grayscale,grayscale,1.0);
}