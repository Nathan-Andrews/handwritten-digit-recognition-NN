# Handwritten Digit Recognition Neural Network in C#
## Table of Contents
[Introduction](#introduction)  
[Features](#features)  
[Technologies Used](#technologies-used)  
[Dataset](#dataset)  
[Getting Started](#getting-started)  
[Prerequisites](#prerequisites)  
[Usage](#usage)  
[Contact](#contact)  

## Introduction
This project is a Neural Network implementation in C#.  
The network implements a threadsafe approach to Gradient Descent, where each datapoint in the batch is run through backpropagation in parallel for faster training.  
After making the framework I used it to train a network to recognize handwritten digits.  
The purpose of this project was to help me learn about Machine Learning and Computer Vision.

## Features
- Neural Network implementation
   - Parallel gradient descent
- Training Visualization
   - A training visualization for 2d datasets, that shows the classification boundry changing as the network trains
- Handwritten Digit Recognition
   - Trained Using the [MNIST Dataset of Handwritten Digits](http://yann.lecun.com/exdb/mnist/)
      - A class to handle the custom idx file format of the dataset.
      - A visualization of the imported images.
      - An image processor that randomly scales, transforms and rotates the images in the dataset so that the neural network doesn't overfit.
   - A simple drawing program that runs your drawing into the neural network to predict what digit it is.

## Technologies Used

Languages: C#, GLSL  
Libraries: [OpenTK](https://opentk.net/)

## Dataset
[MNIST Dataset of Handwritten Digits](http://yann.lecun.com/exdb/mnist/)

Training set: 60000 images  
Testing set: 10000 images

The images are 28x28 grayscale pixels each, stored in `./data/training/MNIST_ORG/{set}-images.idx3-ubyte`.  
Labels are `0-9`, stored in `./data/training/MNIST_ORG/{set}-labels.idx1-ubyte`.

Details on the idx file format: [http://yann.lecun.com/exdb/mnist/](http://yann.lecun.com/exdb/mnist/)

## Prerequisites
[dotnet 7.0.1](https://dotnet.microsoft.com/en-us/download/dotnet/7.0)

## Getting Started

```
1. Clone the repository
   git clone https://github.com/Nathan-Andrews/handwritten-digit-recognition-NN.git
2. Navigate to the project directory
   cd handwritten-digit-recognition-NN
3. Install dependencies
   dotnet restore
4. Start the project
   dotnet run
```

## Usage
To access different built in features you must edit `config.json`  
- `Program`
   - `ImageClassification`
      - `DoClassification : bool` controls whether a pretrained network should be loaded and used for classification
      - `StoredNetworkFile : string` the name of the pretrained network file found in `./data/stored networks/`
      - `DoDrawingMode : bool` controls whether the simple drawing program is opened, and the drawing run through the pretrained network
      - `DoAccuracyCheck : bool` contols whether the dataset found in `Program.Dataset.TestingSetPath` is used to determine the % accuracy of the pretrained network
   - `Training`
      - `DoTraining : bool` control whether training is done
      - `Hyperparameters`
         - `LayerSizes : int[]` the number of nodes in each layer, the first element is the input layer, and the last element is the output layer, any layers between are the hidden layers.  For the image dataset input should be `784` and output should be `10`
         - `MiniBatchSize : int` the size of the minibatch used in training
         - `LearningRate : double` the learning rate of the neural network
      - `Storage`
         - `DoNetworkFile : bool` controls whether the neural network is stored as a file once training is finished
         - `NetworkFile : string` name of the file that the neural network will be stored in
         - `DoOverwrite : bool` controls whether any previous network files with the same name should be overwritten, or if it should create a new file by adding a number to the end of the filename
      - `DoAccuracyCheck : bool` controls whether the network prints out the accuracy of the neural network every 10 epochs
   - `Dataset`
      - `TrainingSetPath : string` path to the dataset used to train the neural network
      - `TestingSetPath : string` path to the dataset used to check the accuracy of the neural network
      - `Format`
         - `IsImage : bool` whether the data is in the idx format or not
         - `IsCSV : bool` whether the data is in the csv format (mutually exclusive with IsImage)
         - `DoImagePreview : bool` controls whether the image dataset is visualized (only considered if IsImage is also true)
         - `ImagePreviewCount : int` the amount of images to load in the image preview
       
About the DataSet class:  
Anyone wishing to modify the code to train off of a different dataset, should make the dataset load into a DataSet class.  
The DataSet class functions similar to a List.  A list of DataPoint objects
   DataPoint(double[] feature, int label, int numLabels):
      - `double[] feature` an array of doubles which will be the inputs to the neural network.  Should be the same dimension as the neural network input
      - `int label` the class of this datapoint
      - `int numLabels` the number of classes in the neural network.  Should be the max value of `label`.  Should be the same dimension as the neural network output
`DataSet.Add(DataPoint dataPoint)` adds a datapoint to the end of the dataset
`DataPoint GetElement(int n)` gets the nth element in the dataset
`int size` the number of elements in the dataset

## Contact

Nathan Andrews - andrewsnathan2003@gmail.com  
Project Link: [https://github.com/Nathan-Andrews/handwritten-digit-recognition-NN](https://github.com/Nathan-Andrews/handwritten-digit-recognition-NN)

