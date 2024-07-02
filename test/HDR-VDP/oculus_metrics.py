import matplotlib.pyplot as plt
import csv
import pandas as pd
import numpy as np

root = f"hdrvdp-3.0.7\\test_oculus"

def get_from_csv(filename):
    with open(filename, 'r') as csvfile:
        y = []
        plots = pd.read_csv(csvfile, skipinitialspace=True, sep=',')
        for (_, row) in enumerate(plots):
            y.append(float(row[0]))
        #y = y[::-1]
    return y

if __name__ == "__main__":
    path = 