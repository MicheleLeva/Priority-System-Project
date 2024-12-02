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

def plot(sub, x, y, z, dir: str, num):
    # plt.bar(x, y, color='r', width=.1)
    # plt.bar(x1, y1, color='b', width=.1)
    sub.plot(x, y, linestyle="-", marker="", color='k', linewidth=1)
    sub.plot(x, z, linestyle="--", marker="", color='k', linewidth=1)
    # plt.plot(x, y1, linestyle="--", marker="", color='k', linewidth=1)
    # trend
    # z = np.polyfit(x, y, 1)
    # p = np.poly1d(z)
    # plt.plot(x,p(x),"r--")
    # ax = plt.gca()
    # # legend
    sub.legend(['mesh', 'material'], loc="upper right")
    sub.axhline(y=np.mean(y), xmin=0, xmax=3, c="red", linewidth=0.5, zorder=0)
    # # ax.plot([0, 1], [0, 1], transform=ax.transAxes, ls="--", color="k", linewidth=1)
    # # x rotation
    # plt.xticks(rotation = 90)
    # # axis size

    # plt.axis('square')
    sub.set_ylim([0, 50])
    # ax.set_xlim([0, 100])
    
    # # labels
    sub.set_xlabel('Objects Number')
    sub.set_ylabel('Latency (s)')
    
    sub.grid(axis = 'y')
    # # title
    sub.set_title(f'{num} Mpbs')
    # # out
    # fig = plt.gcf()
    # fig.set_size_inches(9, 6)
    # out = dir + 'spawn_times.png'
    # print(f"--> OUTPUT: {out}\n")
    # fig.savefig(out, dpi=100)
    # plt.clf()


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
            if (latency == 0): continue
            
            if name not in obj.keys():
                obj[name] = [0,0, float(row[0])]
            if t == "Serializers.SMesh":
                obj[name][0] = latency
            if t == "Serializers.SMaterial":
                obj[name][1] = latency

        l = list(obj.items())
        s = l # sorted(l, key=lambda t: t[1][2])
        # print(obj)
        
        
        for j, w in enumerate(s):
            # s[i] = i, w[1], w[2]
            # if (w[1][0] != 0 and w[1][1] != 0):
            x.append(j)
            y.append(w[1][0])
            z.append(w[1][1])


        return x, y, z
    

def dir_process(dir: str):
    fig, sub = plt.subplots(2, 2)
    for d in [4,10,20,40]:
        csv = dir + str(d) + "\\obj_times.csv"
        # csv1 = dir1 + "obj_times.csv"

        x, y, z = get_from_csv(csv)
        # x1, y1 = get_from_csv(csv)
        # x1, y1, _, _ = get_from_csv(csv1)
        s = sub[0, 0]
        if (d==10): s = sub[0,1]
        if (d==20): s = sub[1,0]
        if (d==40): s = sub[1,1]
        plot(s, x, y, z, dir, d)

        plt.subplots_adjust(wspace=0.2, hspace=0.4)

    # out
    plt.suptitle("Spawn Times")
    fig = plt.gcf()
    fig.set_size_inches(12, 8)
    out = dir + 'spawn_times.png'
    print(f"--> OUTPUT: {out}\n")
    fig.savefig(out, dpi=100)
    plt.clf()

if __name__ == "__main__":
    dirs = sys.argv[1:]
    for d in dirs:
        dir_process(f"{d}")
