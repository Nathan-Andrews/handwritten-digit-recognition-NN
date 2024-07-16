using System.Diagnostics;

namespace simple_network {
    public class ImageSet {
        // public static 
        public Image[] images;

        public DataSet dataPoints;

        public int setSize; // number of images in the set

        public Tuple<int, int> imageDimensions;

        public ImageSet(int size = -1,string path = "/Users/nathanandrews/Desktop/c#_projects/neural_network/untrained-simple-network/data/MNIST_ORG/t10k") {
            IDXReader imagesReader = new($"{path}-images.idx3-ubyte");
            IDXReader labelsReader = new($"{path}-labels.idx1-ubyte");

            Debug.Assert(imagesReader.GetImageCount() == labelsReader.GetImageCount(),$"the number of images and labels in the dataset should match, but ({imagesReader.GetImageCount()},{labelsReader.GetImageCount()})");

            if (size == -1) setSize = imagesReader.GetImageCount();
            else setSize = Math.Min(size,imagesReader.GetImageCount());

            imageDimensions = imagesReader.GetDimensions();

            images = ImageLoader.GetImageSet(imagesReader,labelsReader,setSize);

            dataPoints = new();

            for (int i = 0; i < images.Length; i++) {
                Image image = ImageProcessor.RandomizeImage(images[i]);
                dataPoints.Add(new DataPoint(image.pixels,image.digit,10));
            }

            // foreach (Image image in images) {
            //     image.PrintImageAsAsciiArt();
            // }
        }
    }
}