import csv
import numpy as np
import matplotlib.pyplot as plt
from matplotlib.lines import Line2D
import pandas as pd
import sys
import os

root = f"hdrvdp-3.0.7\\test_move"
#root = f"hdrvdp-3.0.7\\test_oculus"

def barplot(i, SP, AOI):
    # set width of bar 
    barWidth = 0.4
    fig = plt.subplots(figsize =(12, 8)) 

    # set height of bar 
    #SP
    #AOI

    ax = plt.gca()
    ax.set_ylim([0.01, 1])
    ax.set_yscale('log')

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
    plt.title(f"Detection Difference between SP and AOI for {i} Mbps", fontweight ='bold', fontsize = 15)

    #adding labels
    plt.xlabel('Screenshot', fontweight ='bold', fontsize = 15) 
    plt.ylabel('Percentage Points', fontweight ='bold', fontsize = 15) 

    # Adding Xticks 
    #plt.xticks([r + barWidth for r in range(len(SP))], [a for a in range(len(SP))])
    plt.xticks(np.arange(0, len(SP), 5), np.arange(0, len(SP), 5))

    #creating the legend
    legend_elements = [Line2D([0], [0], color='r', lw=4, label='SP'),
                       Line2D([0], [0], color='b', lw=4, label='AOI')] 
    
    ax.legend(handles=legend_elements, loc='upper right')
    
    #plt.legend()
    #plt.show()

    # out
    fig = plt.gcf()
    #fig.set_size_inches(7, 5)
    if (not os.path.isdir(f'Test{test_number}')):
        os.mkdir(f'Test{test_number}')
    out = f'Test{test_number}\\DetectionDifference-{i}Mbps-hdr-vdp.png'
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
    data = xlsx_file.iloc[:,0].values #take the first column
    return data


def dir_process(dir: str, dir1: str, i: int):
    csv = dir + "vdp-hdr-detection.xlsx"
    csv1 = dir1 + "vdp-hdr-detection.xlsx"
    y = get_from_xlsx(csv)
    y1 = get_from_xlsx(csv1)

    #print(zip(y, y1))
    z = [x - x1 if x >= x1 else 0 for x, x1 in zip(y, y1)]
    z1 = [x1 - x if x1 >= x else 0 for x, x1 in zip(y, y1)]



    barplot(i, z, z1)

if __name__ == "__main__":
    for i in [4, 10, 20, 40]:
        test_number = sys.argv[1]
        dir = root + f"\\Test-{test_number}-SP\\{i}\\"
        #dir1 = root + f"\\Test-AOI\\{i}\\"
        dir1 = root + f"\\Test-AOI\\{i}\\"
        dir_process(dir, dir1, i)