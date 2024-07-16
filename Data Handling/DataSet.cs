using System.Drawing;
using OpenTK.Mathematics;


namespace simple_network {
    public class DataSet {
        private DataPoint[] dataArray;
        private int arraySize;
        public int size; 

        public DataSet() {
            size = 0;
            arraySize = 1;

            dataArray = new DataPoint[arraySize];
        }

        public void Add(DataPoint dataPoint) {
            if (size >= arraySize) { // resize
                arraySize = NextFibonacci(arraySize);
                DataPoint[] dataArrayNew = new DataPoint[arraySize];
                
                // Fisher-Yates Shuffle
                for (int i = 0; i < size; i++) {
                    dataArrayNew[i] = dataArray[i];
                }

                dataArray = dataArrayNew;
            }
            
            dataArray[size] = dataPoint;
            size++;
        }

        public DataPoint PopBack() {
            if (size == 0) throw new IndexOutOfRangeException("Cannot remove an element from an empty set.");

            size--;

            return dataArray[size];
        }

        public DataPoint PopElement(int n) {
            if (n >= size) throw new IndexOutOfRangeException($"Index out of range. {n} >= {size}");
            DataPoint point = dataArray[size];

            for (int i = n; i < size; i++) {
                dataArray[i] = dataArray[i+1];
            }
            size--;

            return point;
        }

        public DataPoint GetElement(int n) {
            if (n >= size) throw new IndexOutOfRangeException($"Index out of range. {n} >= {size}");

            return dataArray[n];
        }

        // for minibatch gradient descent
        // selects a subset of the data to be considered in training
        // this speeds up training because the cost function has less points to loop through
        // it can also help the network escape settle points in the cost landscape
        // a new batch is choosen each epoch so that all data is considered
        public DataSet SelectMinibatch(int batchSize) {
            DataSet batch = new();

            Random random = new();

            HashSet<int> elementsInBatch = new();
            while (elementsInBatch.Count < batchSize) {
                int randomIndex = random.Next(0, size);

                if (!elementsInBatch.Contains(randomIndex)) {
                    elementsInBatch.Add(randomIndex);

                    batch.Add(dataArray[randomIndex]);
                }
            }

            return batch; 
        }

        private static int NextFibonacci(int n)
        {
            if (n < 0) throw new ArgumentException("Input must be a non-negative integer.");
            
            int a = 0;
            int b = 1;
            
            while (b <= n)
            {
                int temp = b;
                b = a + b;
                a = temp;
            }
            
            return b;
        }
    }
}