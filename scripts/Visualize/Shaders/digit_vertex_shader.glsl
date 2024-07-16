#version 330 core
layout(location = 0) in vec2 aPosition;
uniform float pixels[784];
out vec2 ourPosition;

void main()
{
    gl_Position = vec4(aPosition, 0.0, 1.0);
    ourPosition = aPosition;
}