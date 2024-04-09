import csv
import numpy as np
import matplotlib.pyplot as plt
import sys

def plot(i, x, y, z):
    plt.plot(x[:56], y[:56], linestyle="-", marker="", color='k', linewidth=1)
    plt.plot(x[:56], z[:56], linestyle="--", marker="", color='k', linewidth=1)

    ax = plt.gca()
    ax.set_ylim([7, 10.5])

    # labels
    plt.xlabel('Time (s)')
    plt.ylabel('HDR-VDP quality')
    plt.grid(axis = 'y')
    plt.legend(["with priority", "without priority"])

    # title
    plt.title(f'{i} Mpbs - HDR-VDP')

    # out
    fig = plt.gcf()
    fig.set_size_inches(7, 5)
    out = f'{i}Mbps-hdr-vdp.png'
    print(f"--> OUTPUT: {out}\n")
    fig.savefig(out, dpi=100)
    plt.clf()

def get_from_csv(filename):
    with open(filename, 'r') as csvfile:
        y = []        
        plots = csv.reader(csvfile, skipinitialspace=True)
        for (_, row) in enumerate(plots):
            y.append(float(row[0]))
        print(y)
    return y

def dir_process(dir: str, dir1: str, i: int):
    csv = dir + "vdp-hdr\\quality.csv"
    csv1 = dir1 + "vdp-hdr\\quality.csv"
    y = get_from_csv(csv)
    y1 = get_from_csv(csv1)


    plot(i, range(len(y)), y, y1)

if __name__ == "__main__":
    for i in [4, 10, 20, 40]:
        dir = f".\\screen-{i}\\"
        dir1 = f".\\screen-{i}-np\\"
        dir_process(dir, dir1, i)