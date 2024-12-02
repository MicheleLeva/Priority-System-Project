import csv
import numpy as np
import matplotlib.pyplot as plt
import pandas as pd

root = f"hdrvdp-3.0.7\\test"
test_number = 4

def plot(i, SP, AOI, SPt, AOIt):
    #plt.plot(x[:56], SP[:56], linestyle="-", marker="", color='k', linewidth=1, label="SP")
    #plt.plot(x[:56], AOI[:56], linestyle="--", marker="", color='k', linewidth=1, label ="AOI")

    plt.plot(SPt, SP, linestyle="-", marker="", color='r', linewidth=1, label="SP")
    plt.plot(AOIt, AOI, linestyle="--", marker="", color='b', linewidth=1, label ="AOI")

    #ax = plt.gca()
    #ax.set_ylim([7, 10.5])

    # labels
    plt.xlabel('Time (s)')
    plt.ylabel('HDR-VDP quality')
    plt.grid(axis = 'y')
    plt.legend(loc='upper left')

    # title
    plt.title(f'{i} Mpbs - HDR-VDP')

    # out
    fig = plt.gcf()
    fig.set_size_inches(7, 5)
    out = f'{i}Mbps-hdr-vdp-quality.png'
    print(f"--> OUTPUT: {out}\n")
    fig.savefig(out, dpi=100)
    plt.clf()

def get_from_csv(filename):
    with open(filename, 'r') as csvfile:
        y = []        
        plots = csv.reader(csvfile, skipinitialspace=True)
        for (_, row) in enumerate(plots):
            y.append(float(row[0]))
        #y = y[::-1]
    return y

def get_from_xlsx(filename):
    xlsx_file = pd.read_excel(filename, header=None)
    times = xlsx_file.iloc[:,0].values #take the first column
    quality_values = xlsx_file.iloc[:,1].values #take the second column
    print("times \n")
    print(times)
    print("values \n")
    print(quality_values)
    return quality_values, times


def dir_process(dir: str, dir1: str, i: int):
    csv = dir + "vdp-hdr-quality.xlsx"
    csv1 = dir1 + "vdp-hdr-quality.xlsx"
    SP, SPt = get_from_xlsx(csv)
    AOI, AOIt = get_from_xlsx(csv1)


    #plot(i, [i/2 for i in range(len(y))], y, y1)
    plot(i, SP, AOI, SPt, AOIt)


if __name__ == "__main__":
    for i in [4, 10, 20, 40]:
        dir = root + f"\\Test-{test_number}-SP\\{i}\\"
        dir1 = root + f"\\Test-{test_number}-AOI\\{i}\\"
        dir_process(dir, dir1, i)