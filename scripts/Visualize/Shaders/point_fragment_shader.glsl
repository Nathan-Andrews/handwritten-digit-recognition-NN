#version 330 core
in vec2 ourPosition;
out vec4 FragColor;
uniform vec2 pointCenters[100];
uniform int pointClasses[100];
uniform float pointRadius;
uniform int pointCount;

void main()
{

    for (int i = 0; i < pointCount; i++) {
        float distance = length(ourPosition - pointCenters[i]);

        if (distance < pointRadius) {
            if (pointClasses[i] == 0) {
                FragColor = vec4(0.337, 0.541, 0.949, 1.0); // blue class
            }
            else {
                FragColor = vec4(0.929, 0.357, 0.349, 1.0); // red class
            }

            return;
        }
    }

    discard;
}