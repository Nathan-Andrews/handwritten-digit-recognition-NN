#version 330 core
in vec3 pixelPosition;
out vec4 FragColor;

float weight_1_1 = -0.3; // sample values
float weight_1_2 = -0.8; // sample values

float weight_2_1 = 0; // sample values
float weight_2_2 = 0.0; // sample values

float bias_1 = 0.2; // sample values
float bias_2 = 0;

int activation(float input_1, float input_2) {
    return (input_1 > input_2) ? 0 : 1;
}

int classify(float input_1, float input_2) {
    float output_1 = input_1 * weight_1_1 + input_2 * weight_2_1 + bias_1;
    float output_2 = input_2 * weight_1_2 + input_2 * weight_2_2 + bias_2;

    return activation(output_1, output_2);
}

void main()
{
    vec4 redColor = vec4(0.98, 0.6, 0.592, 1.0);
    vec4 blueColor = vec4(0.659, 0.769, 0.988, 1.0);

    // vec3 color = pixelPosition * 0.5 + 0.5; // Transform from range [-1,1] to [0,1]
    if (classify(pixelPosition.x, pixelPosition.y) == 0) {
        FragColor = blueColor;
    }
    else {
        FragColor = redColor;
    }
}