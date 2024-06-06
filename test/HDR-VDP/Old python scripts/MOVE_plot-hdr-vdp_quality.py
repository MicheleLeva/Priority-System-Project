import csv
import numpy as np
import matplotlib.pyplot as plt
import pandas as pd

root = f"hdrvdp-3.0.7\\test_move"
test_number = 2

def plot(i, x, SP, AOI):
    #plt.plot(x[:56], SP[:56], linestyle="-", marker="", color='k', linewidth=1, label="SP")
    #plt.plot(x[:56], AOI[:56], linestyle="--", marker="", color='k', linewidth=1, label ="AOI")

    plt.plot(x, SP, linestyle="-", marker="", color='r', linewidth=1, label="SP")
    plt.plot(x, AOI, linestyle="--", marker="", color='b', linewidth=1, label ="AOI")

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
    out = f'Quality-{i}Mbps-hdr-vdp.png'
    print(f"--> OUTPUT: {out}\n")
    fig.savefig(out, dpi=100)
    plt.clf()

def barplot(i, x, SP, AOI):
    # set width of bar 
    barWidth = 0.25
    fig = plt.subplots(figsize =(12, 8)) 
     
    # set height of bar 
    #SP
    #AOI
     
    # Set position of bar on X axis 
    br1 = np.arange(len(SP)) 
    br2 = [x + barWidth for x in br1] 
     
    # Make the plot
    plt.bar(br1, SP, color ='r', width = barWidth, edgecolor ='grey', label ='SP') 
    plt.bar(br2, AOI, color ='b', width = barWidth, edgecolor ='grey', label ='AOI') 
     
    # Adding Xticks 
    plt.xlabel('Screenshot', fontweight ='bold', fontsize = 15) 
    plt.ylabel('Quality', fontweight ='bold', fontsize = 15) 
    plt.xticks([r + barWidth for r in range(len(SP))], [a for a in range(len(SP))])
     
    #plt.legend()
    #plt.show()

    # out
    fig = plt.gcf()
    #fig.set_size_inches(7, 5)
    out = f'Quality-{i}Mbps-hdr-vdp.png'
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
    csv = dir + "vdp-hdr-quality.xlsx"
    csv1 = dir1 + "vdp-hdr-quality.xlsx"
    y = get_from_xlsx(csv)
    y1 = get_from_xlsx(csv1)


    barplot(i, [i for i in range(len(y))], y, y1)

if __name__ == "__main__":
    for i in [4, 10, 20, 40]:
        dir = root + f"\\Test-{test_number}-SP\\{i}\\"
        dir1 = root + f"\\Test-{test_number}-AOI\\{i}\\"
        dir_process(dir, dir1, i)