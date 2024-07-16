using OpenTK.Mathematics;


namespace simple_network {
    public struct DataPoint {
        public readonly double[] feature;
        public readonly int label;
        public readonly double[] expectedOutput;

        public DataPoint(double[] feature, int label, int numLabels) {
            this.feature = feature;
            this.label = label;

            this.expectedOutput = new double[numLabels];
            this.expectedOutput[label] = 1.0;
        }

        private static float TransformToCoordinateSpace(double input) {
            return (float)((input - 0.5) * 2.0);
        }

        public readonly Vector2 GetPointAsCoordinates() {
            return new Vector2(TransformToCoordinateSpace(feature[0]), TransformToCoordinateSpace(feature[1]));
        }
    }
}