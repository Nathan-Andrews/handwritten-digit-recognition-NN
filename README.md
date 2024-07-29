# Handwritten Digit Recognition Neural Network in C#
## Table of Contents
[Introduction](#introduction)  
[Features](#features)  
[Technologies Used](#technologies-used)  
[Dataset](#dataset)  
[Getting Started](#getting-started)  
[Prerequisites](#prerequisites)  
[Installation](#installation)  
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

## Getting Started
Instructions on how to set up and run your project locally.

## Prerequisites
[dotnet 7.0.1](https://dotnet.microsoft.com/en-us/download/dotnet/7.0)

## Installation

bash
Copy code
1. Clone the repository
   git clone https://github.com/Nathan-Andrews/handwritten-digit-recognition-NN.git
2. Navigate to the project directory
   cd handwritten-digit-recognition-NN
3. Install dependencies
   dotnet restore
4. Start the project
   dotnet run

## Usage
Instructions and examples on how to use your project. Include screenshots or code snippets if applicable.

## Contact
css
Copy code
Your Name - andrewsnathan2003@gmail.com
Project Link: https://github.com/Nathan-Andrews/handwritten-digit-recognition-NN
