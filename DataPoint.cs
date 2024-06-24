using OpenTK.Mathematics;


namespace simple_network {
    public class DataPoint {
        public double[] feature;
        public int label;
        public double[] expectedOutput;

        public DataPoint(double[] feature, int label, int numLabels) {
            this.feature = feature;
            this.label = label;

            this.expectedOutput = new double[numLabels];
            this.expectedOutput[label] = 1;
        }

        private static float transformToCoordinateSpace(double input) {
            return (float)((input - 0.5) * 2.0);
        }

        public Vector2 getPointAsCoordinates() {
            return new Vector2(transformToCoordinateSpace(feature[0]), transformToCoordinateSpace(feature[1]));
        }
    }
}