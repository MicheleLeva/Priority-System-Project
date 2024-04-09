import csv
import numpy as np
import matplotlib.pyplot as plt
import sys

def plot(i, x, y, z):
    plt.plot(x[:56], y[:56], linestyle="-", marker="", color='k', linewidth=1)
    plt.plot(x[:56], z[:56], linestyle="--", marker="", color='k', linewidth=1)

    ax = plt.gca()
    # ax.set_ylim([7, 10.5])

    # labels
    plt.xlabel('Time (s)')
    plt.ylabel('average pixels ∆E')
    plt.grid(axis = 'y')
    plt.legend(["with priority", "without priority"])

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

def dir_process(file: str):
    y = get_from_csv(file)
    print(y)
    print('\n')
    print('\n')

    for i, x in enumerate([4, 10, 20, 40]):
        print(i)
        print('\n')
        plot(x, range(len(y[0])), y[i*2] , y[i*2+1])

if __name__ == "__main__":
    dir_process("deltaE2000.csv")