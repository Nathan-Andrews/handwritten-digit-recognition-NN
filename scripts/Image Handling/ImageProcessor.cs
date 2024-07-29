namespace DigitRecognition.ImageHandling {
    public static class ImageProcessor{
        public static Image RandomizeImage(Image old) {
            Image image = Scale(old,GenerateNormalRandom(1.0,0.2) - 0.1);
            image = Transform(image,GenerateNormalRandom(0.0,2.5), GenerateNormalRandom(0.0,1.5));
            image = Rotate(image,GenerateNormalRandom(0.0,0.25));

            return image;
        }


        public static Image Transform(Image old, double xShift,double yShift) {
            Image image= new(old.size,old.pixels,old.digit);

            Parallel.For(0,old.width, (x) => { // no massive performance gain from having this parallel, but why not?
                Parallel.For(0,old.width, (y) => {
                    double xTransformed = x - xShift;
                    double yTransformed = y - yShift;

                    image.pixels[image.GetIndex2d(x,y)] = InterpolatePixel(old,xTransformed,yTransformed);
                });
            });

            return image;
        }

        public static Image Rotate(Image old, double theta) {
            Image image= new(old.size,old.pixels,old.digit);

            Parallel.For(0,old.width, (x) => { // no massive performance gain from having this parallel, but why not?
                Parallel.For(0,old.width, (y) => {

                    // https://www.mathworks.com/help/visionhdl/ug/small-angle-rotation-ext-mem.html
                    double xTransformed = x - (old.width / 2);
                    double yTransformed = y - (old.width / 2);
                    double xRotated= (xTransformed * Math.Cos(theta)) - (yTransformed * Math.Sin(theta));
                    double yRotated = (xTransformed * Math.Sin(theta)) + (yTransformed * Math.Cos(theta));
                    xRotated += old.width / 2;
                    yRotated += old.width / 2;

                    image.pixels[image.GetIndex2d(x,y)] = InterpolatePixel(old,xRotated,yRotated);
                });
            });

            return image;
        }

        public static Image Scale(Image old, double scalingFactor) {
            Image image= new(old.size,old.pixels,old.digit);

            Parallel.For(0,old.width, (x) => { // no massive performance gain from having this parallel, but why not?
                Parallel.For(0,old.width, (y) => {
                    double xTransformed = x - (old.width / 2);
                    double yTransformed = y - (old.width / 2);

                    xTransformed /= scalingFactor;
                    yTransformed /= scalingFactor;

                    xTransformed += old.width / 2;
                    yTransformed += old.width / 2;

                    image.pixels[image.GetIndex2d(x,y)] = InterpolatePixel(old,xTransformed,yTransformed);
                });
            });

            return image;
        }
        // private static Image Blur()
        // private static Image Noiseify()

        public static Image Downsize(Image old, int size) {
            Image image = new(size);

            double ratio = (double) image.width / old.width;

            for (int x = 0; x <  image.width; x++) {
                for (int y = 0; y < image.width; y++) {
                    int x0 = (int) Math.Floor(x / ratio);
                    int y0 = (int) Math.Floor(y / ratio);
                    int xMax = (int) Math.Ceiling(1 / ratio) + x0;
                    int yMax = (int) Math.Ceiling(1 / ratio) + y0;

                    double pixel = 0.0;
                    int count = 0;

                    for (int xi = x0; xi < xMax; xi++) {
                        for (int yi = y0; yi < yMax; yi++) {
                            count++;
                            pixel += old.pixels[old.GetIndex2d(xi,yi)];
                        }
                    }

                    image.pixels[image.GetIndex2d(-(y - image.width + 1),x)] = Math.Min(pixel / count,1);
                }
            }

            return image;
        }

        static double InterpolatePixel(Image image, double xTransformed, double yTransformed) {
            double pixel = 0.0;

            int xBase = (int) Math.Floor(xTransformed);
            int yBase = (int) Math.Floor(yTransformed);

            double xFractional = xTransformed - xBase;
            double yFractional = yTransformed - yBase;
            
            if (image.PointInsideImage(xBase,yBase) && image.PointInsideImage(xBase + 1,yBase + 1)) {
                // pick the four closest points to interpolate
                Tuple<double,double,double,double> surroundingPixels = new(
                    image.pixels[image.GetIndex2d(xBase,yBase)],
                    image.pixels[image.GetIndex2d(xBase+1,yBase)],
                    image.pixels[image.GetIndex2d(xBase,yBase+1)],
                    image.pixels[image.GetIndex2d(xBase+1,yBase+1)]
                ); 

                double topInterpolation = surroundingPixels.Item1 * (1 - xFractional) + surroundingPixels.Item2 * xFractional;
                double bottomInterpolation = surroundingPixels.Item3 * (1 - xFractional) + surroundingPixels.Item4 * xFractional;
                pixel = topInterpolation * (1 - yFractional) + bottomInterpolation * yFractional;

                if (double.IsNaN(pixel)) pixel = 0.0;
            }

            return pixel;
        }

        private static double GenerateNormalRandom(double mean, double stdDev)
        {
            Random random = new Random();

            // Box-Muller transform
            // https://mathworld.wolfram.com/Box-MullerTransformation.html
            double u1 = 1.0 - random.NextDouble();
            double u2 = 1.0 - random.NextDouble();
            double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2);
            double randNormal = mean + stdDev * randStdNormal;

            return randNormal;
        }
    }
}