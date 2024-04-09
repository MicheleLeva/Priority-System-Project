from array import array
from copy import copy
import csv
from re import L
from statistics import mean
from turtle import color
import numpy as np
import cv2
import matplotlib.pyplot as plt
import sys
import pandas as pd

FULLY_LOADED = 3455802
NUM_IMG = 5
WAIT = False

def plot(x, y, z, dir: str):
    # plt.bar(x, y, color='r', width=.1)
    # plt.bar(x1, y1, color='b', width=.1)
    plt.plot(x, y, linestyle="-", marker="", color='k', linewidth=1)
    plt.plot(x, z, linestyle="--", marker="", color='k', linewidth=1)
    plt.plot([], [], color='w')
    plt.plot([], [], color='w')
    # plt.plot(x, y1, linestyle="--", marker="", color='k', linewidth=1)
    # trend
    # z = np.polyfit(x, y, 1)
    # p = np.poly1d(z)
    # plt.plot(x,p(x),"r--")
    ax = plt.gca()
    # legend
    plt.legend(['mesh', 'material'], loc="upper right")
    # ax.plot([0, 1], [0, 1], transform=ax.transAxes, ls="--", color="k", linewidth=1)
    # x rotation
    # plt.xticks(rotation = 90)
    plt.axhline(y=np.mean(y), xmin=0, xmax=3, c="red", linewidth=0.5, zorder=0)
    # axis size

    # plt.axis('square')
    ax.set_ylim([0, 50])
    # ax.set_xlim([0, 100])
    
    # labels
    plt.xlabel('Objects Number')
    plt.ylabel('Download Latency (s)')
    
    plt.grid(axis = 'y')
    # title
    num = int(dir.split('\\')[-2])
    plt.title(f'{num} Mpbs - Spawn Times')
    # out
    fig = plt.gcf()
    fig.set_size_inches(5, 5)
    out = dir + 'spawn_times.png'
    print(f"--> OUTPUT: {out}\n")
    fig.savefig(out, dpi=100)
    plt.clf()


def get_from_csv(filename):
    with open(filename, 'r') as csvfile:
        x = []
        y = []
        z = []
        
        plots = csv.reader(csvfile, skipinitialspace=True)

        obj = {}
        for (_, row) in enumerate(plots):
            name = row[2]
            t = row[4]
            latency = float(row[1])-float(row[0])
            
            if name not in obj.keys():
                obj[name] = [0,0]
            if t == "Serializers.SMesh":
                obj[name][0] = latency
            if t == "Serializers.SMaterial":
                obj[name][1] = latency

        l = list(obj.items())
        s = sorted(l, key=lambda t: t[1])
        # print(obj)
        
        
        for j, w in enumerate(s):
            # s[i] = i, w[1], w[2]
            # if (w[1][0] != 0 and w[1][1] != 0):
            x.append(j)
            y.append(w[1][0])
            z.append(w[1][1])


        return x, y, z
    

def dir_process(dir: str):
    csv = dir + "obj_times.csv"
    # csv1 = dir1 + "obj_times.csv"

    x, y, z = get_from_csv(csv)
    # x1, y1 = get_from_csv(csv)
    # x1, y1, _, _ = get_from_csv(csv1)
    plot(x, y, z, dir)

if __name__ == "__main__":
    dirs = sys.argv[1:]
    for d in dirs:
        for x in [4,10,20,40]:
            dir_process(f"{d}{x}\\")
