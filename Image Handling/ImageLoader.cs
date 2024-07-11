namespace simple_network {
    public static class ImageLoader {
        public static Image[] GetImageSet(IDXReader imagesReader,IDXReader labelsReader,int imageCount) {
            Image[] images = new Image[imageCount];

            Tuple<int,int> imageDimensions = imagesReader.GetDimensions();
            int rows = imageDimensions.Item1;
            int cols = imageDimensions.Item2;
            int imageSize = rows * cols;

            int offset = 16;

            Parallel.For(0,imageCount, (i) => {
                images[i] = GenerateImage(imagesReader.GetByteRange(offset + (i * imageSize), imageSize),imageSize);
                images[i].digit = labelsReader.GetByteRangeInt(offset - 8 + i, 1);
            });

            return images;
        }

        private static Image GenerateImage(byte[] bytes, int size) {
            double[] pixels = new double[size];

            Parallel.For(0,size, (i) => {
                pixels[i] = bytes[i] / 255.0;
            });

            return new Image(size,pixels);
        }
    }
}