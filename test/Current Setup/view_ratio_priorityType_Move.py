from array import array
from turtle import color
import numpy as np
import cv2
import matplotlib.pyplot as plt
import sys
from os import walk
import os

FILENAME = 'screen_X.png'
TITLE = 'View Ratio'
LABELS = 'Time (s)', 'View Ratio (%)'

FULLY_LOADED = 1852374
HIGHEST = 1588488
NUM_IMG = 29
WAIT = False

full = ".\\FullMoving\\"

def plot(x, y, x1, y1, dir: str):
    # fig, ax = plt.subplots();

    plt.plot(x, y, linestyle="-", marker="", color='r', linewidth=1)
    plt.plot(x1, y1, linestyle="--", marker="", color='b', linewidth=1)

    # legend
    plt.legend(['screen presence', 'areas of interest'], loc="lower right")
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

def multi_img_process(filename: str, fullname: str):
    img = cv2.imread(filename)
    full = cv2.imread(fullname)
    return np.sum(img == full)

def get_percent(x, low):
    return (low-x)/low * 100

def get_files_in_dir(mypath):
    f = []
    for (dirpath, dirnames, filenames) in walk(mypath):
        f.extend(filenames)
        break

    for i, x in enumerate(f):
        if ',' in x:
            oldname = mypath + x
            newname = mypath + x.replace(",", ".")
            s = x.replace(",", ".")
            os.rename(oldname, newname)
        else:
            s = x
        f[i] = s.strip("screen_").strip(".png")

    f.sort(key=float)
    return f

def dir_process(dir: str, dir1:str):
    #print(f"Dir 1 is {dir}, Dir 2 is {dir1}")
    
    x = [0]
    x1 = [0]
    y = [0]
    y1 = [0]

    filenames: array[str] = get_files_in_dir(dir)
    filenames1: array[str] = get_files_in_dir(dir1)
    fullnames: array[str] = get_files_in_dir(full)
    #print(f"fullnames length = {len(fullnames)}")

    for f, g in zip(filenames[1:], fullnames[1:]):
        # process difference SP
        a = multi_img_process(f"{dir}screen_{f}.png", f"{full}screen_{g}.png")
        #print(a)
        x.append(float(f))
        y.append(get_percent(a, HIGHEST))

    for  f, g in zip(filenames1[1:], fullnames[1:]):
        # process difference AOI
        b = multi_img_process(f"{dir1}screen_{f}.png", f"{full}screen_{g}.png")
        #print(b)
        x1.append(float(f))
        y1.append(get_percent(b, HIGHEST))

    plot(x, y, x1, y1, dir.strip("SCREEN\\"))

if __name__ == "__main__":
    dirs = sys.argv[1:]
    for x in [4,10,20,40]:
        dir_process(f"{dirs[0]}{x}\\SCREEN\\", f"{dirs[1]}{x}\\SCREEN\\")
