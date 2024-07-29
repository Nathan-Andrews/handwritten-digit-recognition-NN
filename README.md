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
A brief description of what your project does and why you created it.

## Features
List of features or functionalities your project offers.

## Technologies Used
List of technologies and tools you used in the project (e.g., programming languages, frameworks, libraries).

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
Ways to get in touch with you for further questions or contributions. For example:

css
Copy code
Your Name - andrewsnathan2003@gmail.com
Project Link: https://github.com/Nathan-Andrews/handwritten-digit-recognition-NN
