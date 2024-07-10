namespace simple_network {
    public static class ImageLoader {
        public static Image[] GetImageSet(IDXReader imagesReader,IDXReader labelsReader,int imageCount) {
            Image[] images = new Image[imageCount];

            Tuple<int,int> imageDimensions = imagesReader.GetDimensions();
            int rows = imageDimensions.Item1;
            int cols = imageDimensions.Item2;
            int imageSize = rows * cols;

            int offset = 16;

            for (int i = 0; i < imageCount; i++) { // could be improved with a Parallel.For
                images[i] = GenerateImage(imagesReader.GetByteRange(offset + (i * imageSize), imageSize),imageSize);
                images[i].digit = labelsReader.GetByteRangeInt(offset + i, 1);
            }

            return images;
        }

        private static Image GenerateImage(byte[] bytes, int size) {
            double[] pixels = new double[size];

            for (int i = 0; i < size; i++) { // could be improved with a Parallel.For
                pixels[i] = bytes[i] / 255.0;
            }

            return new Image(size,pixels);
        }
    }
}