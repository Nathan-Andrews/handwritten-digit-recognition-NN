namespace DigitRecognition.ImageHandling {
    public class Image {
        public int size; // assume image is square

        public double[] pixels;

        public int digit;
        public int width;

        public Image(int size, double[] pixels, int digit = -1) {
            this.size = size;
            this.pixels = new double[size];
            Array.Copy(pixels,this.pixels,size);
            this.digit = digit;
            this.width = (int) Math.Sqrt(size);
        }

        public Image(int size) { // creates a completely black image
            this.size = size;
            this.pixels = new double[size];
            for (int i = 0; i < size; i++) {
                this.pixels[i] = 0;
            }
            this.digit = -1;
            this.width = (int) Math.Sqrt(size);
        }

        public int GetIndex2d(int x, int y) {
            return x * width + y;
        }

        public bool PointInsideImage(int x, int y) {
            return 0 <= x && 0 <= y&& x < width && y < width;
        }

        public void PrintImageAsAsciiArt() {
            for (int i = 0; i < size; i++) {
                if (i % width == 0) {
                    Console.WriteLine("");
                }

                if (pixels[i] == 0) {
                    Console.Write(" ");
                }
                else {
                    Console.Write("0");
                }
            }
        }
        
    }
}