from array import array
from turtle import color
import numpy as np
import cv2
import matplotlib.pyplot as plt
import sys
from os import walk

FILENAME = 'screen_X.png'
TITLE = 'View Ratio'
LABELS = 'Time (s)', 'View Ratio (%)'

FULLY_LOADED = 1852374
NUM_IMG = 29
WAIT = False


def plot(x, y, x1, y1, dir: str):
    # fig, ax = plt.subplots();

    plt.plot(x, y, linestyle="-", marker="", color='r', linewidth=1)
    plt.plot(x1, y1, linestyle="--", marker="", color='b', linewidth=1)

    # legend
    plt.legend(['with priority', 'without priority'], loc="lower right")
    # x rotation
    # plt.xticks(rotation = 90)
    # axis size
    ax = plt.gca()
    # ax.set_yscale('log')
    ax.set_ylim([0, 110])
    # ax.set_xlim([0, NUM_IMG//2 + 2])
    # labels
    plt.xlabel(LABELS[0])
    plt.ylabel(LABELS[1])
    # title
    plt.title(dir.split('\\')[2] + ' Mpbs - ' + TITLE)
    # out
    fig = plt.gcf()
    fig.set_size_inches(5, 4)
    out = dir + TITLE.replace(' ','_').lower() + '.png'
    print(f"--> OUTPUT: {out}\n")
    fig.savefig(out, dpi=100)
    plt.clf()

def img_process(filename: str, base):
    img = cv2.imread(filename)
    return np.sum(img != base)

def get_percent(x, low):
    return (low-x)/low * 100

def get_files_in_dir(mypath):
    f = []
    for (dirpath, dirnames, filenames) in walk(mypath):
        f.extend(filenames)
        break

    for i, x in enumerate(f):
        #regional settings mean . instead of ,
        f[i] = x.strip("screen_").strip(".png").replace(",", ".")        
        # print(x)
    f.sort(key=float)
    #for i, x in enumerate(f):
        #f[i] = x.replace(".", ",") # put , back
    return f

def dir_process(dir: str, dir1:str):
    last = cv2.imread("Test-0\\40\\SCREEN\\screen_29,973.png")
    
    x = [0]
    x1 = [0]
    y = [0]
    y1 = [0]

    filenames: array[str] = get_files_in_dir(dir)
    filenames1: array[str] = get_files_in_dir(dir1)

    # print("-"*20, dir, dir1)
    k0 = filenames[0].replace(".", ",")
    lowest = img_process(f"{dir}screen_{k0}.png", last)

    for f in filenames[1:]:
        # process
        k = f.replace(".", ",")
        a = img_process(f"{dir}screen_{k}.png", last)
        x.append(float(f))
        y.append(get_percent(a, lowest))

    for f in filenames1[1:]:
        # process
        k = f.replace(".", ",")
        b = img_process(f"{dir1}screen_{k}.png", last)
        x1.append(float(f))
        y1.append(get_percent(b, lowest))

    plot(x, y, x1, y1, dir.strip("SCREEN\\"))

if __name__ == "__main__":
    dirs = sys.argv[1:]
    for x in [4,10,20,40]:
        dir_process(f"{dirs[0]}{x}\\SCREEN\\", f"{dirs[1]}{x}\\SCREEN\\")
