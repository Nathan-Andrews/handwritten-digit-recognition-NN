using System;
using System.Diagnostics;
using System.IO;
using System.Numerics;
using Microsoft.VisualBasic;
using OpenTK.Graphics.OpenGL;


namespace DigitRecognition.ImageHandling {
    public class IDXReader {
        // made to read the idx file format used in the mnist handwrited digits database
        // http://yann.lecun.com/exdb/mnist/
        byte[] bytes;

        public IDXReader(string filename) {
            if (filename == null || !File.Exists(filename)) {
                Console.Out.WriteLine("file does not exist");
                bytes = Array.Empty<byte>();
                return;
            }

            bytes = File.ReadAllBytes(filename);
        }
        
        public byte[] GetBytes() {
            return bytes;
        }

        public byte GetByte(int index) {
            return bytes[index];
        }

        // used to get the number of labels or images
        public int GetImageCount() {
            return GetByteRangeInt(4,4); // 0004 - 0007 // 32 bit integer // number of items
        }

        // used to get the number of rows and columns
        // only to be used on the image dataset not the label dataset
        public Tuple<int,int> GetDimensions() {
            int rows = GetByteRangeInt(8,4); // 0008  // 32 bit integer  // number of rows
            int cols = GetByteRangeInt(12,4); // 0012  // 32 bit integer  // number of columns
            
            Debug.Assert(rows == cols, $"Number of rows and columns should match in the MNIST database, got ({rows},{cols})");

            return Tuple.Create(rows, cols);
        }

        public byte[] GetByteRange(int offset, int length) {
            return bytes.Skip(offset).Take(length).ToArray();
        }

        public int GetByteRangeInt(int offset, int length) {
            byte[] sliced = GetByteRange(offset,length).Reverse().ToArray(); // time complexity could be improved
            int sum = 0;

            for (int i = 0; i < length; i++) {
                sum += sliced[i] << (i * 8);
            }

            return sum;
        }
    }
}