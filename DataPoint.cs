using OpenTK.Mathematics;

public class DataPoint {
    public float[] feature;
    public int label;
    public double[] expectedOutput;

    public DataPoint(float[] feature, int label, int numLabels) {
        this.feature = feature;
        this.label = label;

        this.expectedOutput = new double[numLabels];
        this.expectedOutput[label] = 1;
    }

    private static float transformToCoordinateSpace(float input) {
        return (input - 0.5f) * 2.0f;
    }

    public Vector2 getPointAsCoordinates() {
        return new Vector2(transformToCoordinateSpace(feature[0]), transformToCoordinateSpace(feature[1]));
    }
}