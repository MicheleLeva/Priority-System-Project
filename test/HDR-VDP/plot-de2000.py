import csv
import numpy as np
import matplotlib.pyplot as plt
import sys

root = f"hdrvdp-3.0.7\\test"
test_number = 4

def plot(i, x, SP, AOI):
    plt.plot(x[:56], SP[:56], linestyle="-", marker="", color='r', linewidth=1, label="SP")
    plt.plot(x[:56], AOI[:56], linestyle="--", marker="", color='b', linewidth=1, label="AOI")

    ax = plt.gca()
    # ax.set_ylim([7, 10.5])

    # labels
    plt.xlabel('Time (s)')
    plt.ylabel('average pixels ∆E')
    plt.grid(axis = 'y')
    #plt.legend(["with priority", "without priority"])
    plt.legend(loc='upper left')

    # title
    plt.title(f'{i} Mpbs - ∆E 2000')

    # out
    fig = plt.gcf()
    fig.set_size_inches(7, 5)
    out = f'{i}Mbps-de2000.png'
    print(f"--> OUTPUT: {out}\n")
    fig.savefig(out, dpi=100)
    plt.clf()

def get_from_csv(filename):
    with open(filename, 'r') as csvfile:
        y = []        
        plots = csv.reader(csvfile, skipinitialspace=True)
        for (_, row) in enumerate(plots):
            y.append([float(a) for a in row])
    return y

def dir_process(dir: str, dir1: str, i: int):
    csv = dir + "deltaE2000.csv"
    csv1 = dir1 + "deltaE2000.csv"
    y = get_from_csv(csv)[1]
    y1 = get_from_csv(csv1)[1]

    print(y)
    print('\n')
    print(y1)
    print('\n')

    plot(i, [i/2 for i in range(len(y))], y, y1)

if __name__ == "__main__":
    for i in [4, 10, 20, 40]:
        dir = root + f"\\Test-{test_number}-SP\\{i}\\"
        dir1 = root + f"\\Test-{test_number}-AOI\\{i}\\"
        dir_process(dir, dir1, i)