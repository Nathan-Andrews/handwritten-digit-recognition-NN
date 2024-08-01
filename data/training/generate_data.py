import csv
import random
import math


# def classify(x,y):
#     if (0.5 * (y ** 0.7) + 2.3 * y + 1.8) > ((x ** 2) - 0.5 * x + 3.2):
#         return 1
#     return 0

def classify(x, y):
    # Complex boundary using a combination of sine and cosine functions
    boundary = 0.6 + 0.4 * math.sin(3.2 * math.pi * x) + 0.38 * math.cos(2.8 * math.pi * y)
    
    if y < boundary:
        return 0
    else:
        return 1

def jiggle(x):
    return (x / 10) + (random.random() / 10 / 2)

rows = []

for x in range(0,10):
    for y in range(0,10):
        row = []

        row.append(jiggle(x))
        row.append(jiggle(y))

        row.append(classify(row[0],row[1]))

        rows.append(row)


# field names
fields = ['feature_1','feature_2', 'class']

# name of csv file
filename = "./data/training/data3.csv"

# writing to csv file
with open(filename, 'w') as csvfile:
    # creating a csv writer object
    csvwriter = csv.writer(csvfile)

    # writing the fields
    csvwriter.writerow(fields)

    # writing the data rows
    csvwriter.writerows(rows)