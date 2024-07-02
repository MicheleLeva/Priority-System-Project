import csv
import numpy as np
import matplotlib.pyplot as plt
import pandas as pd
import argparse
import os

root = f"hdrvdp-3.0.7\\test_move"
test_number = None

def barplot(i, SP, AOI):
    # set width of bar 
    barWidth = 0.2
    fig = plt.subplots(figsize =(12, 8)) 

    # set height of bar 
    #SP
    #AOI

    ax = plt.gca()
    ax.set_ylim([0, 1])

    # Set position of bar on X axis 
    br1 = np.arange(len(SP)) 
    br2 = [x + barWidth for x in br1] 
     
    # Make the plot
    plt.bar(br1, SP, color ='r', width = barWidth, edgecolor ='grey', label ='SP') 
    plt.bar(br2, AOI, color ='b', width = barWidth, edgecolor ='grey', label ='AOI') 

    # Adding Xticks 
    plt.xlabel('Screenshot', fontweight ='bold', fontsize = 15) 
    plt.ylabel('Detection Difference', fontweight ='bold', fontsize = 15) 
    plt.xticks([r + barWidth for r in range(len(SP))], [a for a in range(len(SP))])
    
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
        parser = argparse.ArgumentParser()
        parser.add_argument("test_number", type=int)
        args = parser.parse_args()
        test_number = args.test_number
        dir = root + f"\\Test-{test_number}-SP\\{i}\\"
        #dir1 = root + f"\\Test-AOI\\{i}\\"
        dir1 = root + f"\\Test-{test_number}-AOI\\{i}\\"
        dir_process(dir, dir1, i)