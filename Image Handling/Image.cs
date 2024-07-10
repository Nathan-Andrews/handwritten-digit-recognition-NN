using System.Runtime.CompilerServices;

namespace simple_network {
    public class Image {
        public int size; // assume image is square

        public double[] pixels;

        public int digit;

        public Image(int size, double[] pixels, int digit = -1) {
            this.size = size;
            this.pixels = pixels;
            this.digit = digit;
        }

        public double GetIndex2d(int x, int y) {
            return x * size + y;
        }

        public void PrintImageAsAsciiArt() {
            for (int i = 0; i < size; i++) {
                if (i % Math.Sqrt(size) == 0) {
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