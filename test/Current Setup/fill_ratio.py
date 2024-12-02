import csv
from turtle import color, title
import numpy as np
import cv2
import matplotlib.pyplot as plt
import sys

FILENAME = 'obj_count.csv'
TITLE = 'Fill Ratio'
LABELS = 'Time (s)', 'Bytes (B)'
AXIS = 0, 2
TIME = True
BARS = False

def plot(x, y, x1, y1, dir: str):
    if BARS:
        plt.bar(x, y, color='r', linewidth=.1)
        plt.bar(x1, y1, color='b', linewidth=.1)
    else:
        plt.plot(x, np.cumsum(y), linestyle="-", marker="x", color='r', linewidth=1)
        plt.plot(x1, np.cumsum(y1), linestyle="--", marker="^", color='b', linewidth=1)
    # legend
    plt.legend(['with priority', 'without priority'], loc="upper right")
    # x rotation
    # plt.xticks(rotation = 90)
    # axis size
    ax = plt.gca()
    # ax.set_yscale('log')
    # plt.ticklabel_format(useOffset=False, style='plain', axis='y')
    # ax.set_ylim([0, 133])
    # labels
    plt.xlabel(LABELS[0])
    plt.ylabel(LABELS[1])
    # title
    plt.title(dir[-3:-1] + ' Mbps - ' + TITLE)
    # out
    fig = plt.gcf()
    fig.set_size_inches(9, 6)
    out = dir + TITLE.replace(' ','_').lower() + '.png'
    print(f"--> OUTPUT: {out}\n")
    fig.savefig(out, dpi=100)
    plt.clf()


def get_from_csv(filename):
    with open(filename, 'r') as csvfile:
        x = []
        y = []
        plots = csv.reader(csvfile, skipinitialspace=True)
        init = 0
        # sum = 0
        for (i, row) in enumerate(plots):
            try:
                
                t = float(row[AXIS[0]])
                if TIME and init == 0:
                    init = float(t)
                
                x.append(float(t) - init)
                y.append(float(row[AXIS[1]])/1000)

            except: pass
        return x, y
    

def dir_process(dir: str, dir1:str):
    for i in [4, 10, 20, 40]:
        csv = f"{dir}{i}\\{FILENAME}"
        print(csv)
        csv1 = f"{dir1}{i}\\{FILENAME}"

        x, y = get_from_csv(csv)
        x1, y1 = get_from_csv(csv1)

        plot(x, y, x1, y1, f"{dir}{i}\\")

if __name__ == "__main__":
    dirs = sys.argv[1:]
    dir_process(dirs[0], dirs[1])
