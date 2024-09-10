import csv
import numpy as np
import matplotlib.pyplot as plt
from matplotlib.lines import Line2D
import pandas as pd
import sys
import os

#root = f"hdrvdp-3.0.7\\test_move"
root = f"hdrvdp-3.0.7\\test_oculus"

def barplot(i, SP, AOI):
    # set width of bar 
    barWidth = 0.4
    fig = plt.subplots(figsize =(12, 8)) 

    # set height of bar 
    #SP
    #AOI

    ax = plt.gca()
    ax.set_ylim([0, 10])

    # Set position of bar on X axis 
    br1 = np.arange(len(SP)) 
    #br2 = [x + barWidth for x in br1] 

    #color
    color = ['r' if x > 0 else 'b' for x in SP]

    # Make the plot
    #plt.bar(br1, SP, color ='r', width = barWidth, edgecolor ='grey', label ='SP') 
    #plt.bar(br2, AOI, color ='b', width = barWidth, edgecolor ='grey', label ='AOI')
    both = [SP[i] if SP[i] > 0 else AOI[i] for i in range(len(SP))]
    plt.bar(br1, both, color = color, width = barWidth, edgecolor ='grey')

    #adding title
    plt.title(f"Quality Difference between SP and AOI for {i} Mbps", fontweight ='bold', fontsize = 15)

    #adding labels
    plt.xlabel('Screenshot', fontweight ='bold', fontsize = 15) 
    plt.ylabel('JODs', fontweight ='bold', fontsize = 15) 

    # Adding Xticks 
    #plt.xticks([r + barWidth for r in range(len(SP))], [a for a in range(len(SP))])
    plt.xticks(np.arange(0, len(SP), 5), np.arange(0, len(SP), 5))

    #plt.legend()
    #plt.show()

    #SP_average = sum(SP) / len(SP)
    #AOI_average = sum(AOI) / len(AOI)
    SP_occurrences = len([x for x in SP if x > 0])
    AOI_occurrences = len([x for x in AOI if x > 0])
    SP_average = sum(SP) / SP_occurrences
    AOI_average = sum(AOI) / AOI_occurrences

    data.append([SP_average, AOI_average, SP_occurrences, AOI_occurrences])

    # out
    fig = plt.gcf()
    #fig.set_size_inches(7, 5)
    if (not os.path.isdir(f'Test{test_number}')):
        os.mkdir(f'Test{test_number}')
    out = f'Test{test_number}\\QualityDifference-{i}Mbps-hdr-vdp.png'
    print(f"--> OUTPUT: {out}\n")
    #print(f"    SP Average: {SP_average}  AOI Average: {AOI_average}\n")
    #print(f"    SP Occurrences: {SP_occurrences}  AOI Occurrences: {AOI_occurrences}\n")
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
    data = xlsx_file.iloc[:,0].values #take the first column
    return data


def dir_process(dir: str, dir1: str, i: int):
    csv = dir + "vdp-hdr-quality.xlsx"
    csv1 = dir1 + "vdp-hdr-quality.xlsx"
    y = get_from_xlsx(csv)
    y1 = get_from_xlsx(csv1)

    z = [x - x1 if x >= x1 else 0 for x, x1 in zip(y, y1)]
    z1 = [x1 - x if x1 >= x else 0 for x, x1 in zip(y, y1)]

    barplot(i, z, z1)

if __name__ == "__main__":
    test_number = sys.argv[1]

    bandwidths = [4, 10, 20, 40]
    numbers = ["SP average", "AOI average", "SP occurrences", "AOI occurrences"]
    data = []

    for i in bandwidths:
        dir = root + f"\\Test-{test_number}-SP\\{i}\\"
        dir1 = root + f"\\Test-AOI\\{i}\\"
        dir_process(dir, dir1, i)

    df = pd.DataFrame(data, columns=numbers, index=bandwidths)
    print(data)
    print(df)
    df.to_excel(f"Test{test_number}\\QualityDifference-Table.xlsx")  