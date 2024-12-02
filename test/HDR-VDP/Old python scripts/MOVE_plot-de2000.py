import csv
import numpy as np
import matplotlib.pyplot as plt
import pandas as pd

root = f"hdrvdp-3.0.7\\test_move"
test_number = 1

def plot(i, x, SP, AOI):
    plt.plot(x, SP, linestyle="-", marker="", color='r', linewidth=1, label="SP")
    plt.plot(x, AOI, linestyle="--", marker="", color='b', linewidth=1, label="AOI")

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
    out = f'DE2000-{i}Mbps.png'
    print(f"--> OUTPUT: {out}\n")
    fig.savefig(out, dpi=100)
    plt.clf()

def barplot(i, SP, AOI):
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
    plt.ylabel('∆E 2000', fontweight ='bold', fontsize = 15) 
    plt.xticks([r + barWidth for r in range(len(SP))], [a for a in range(len(SP))])
     
    #plt.legend()
    #plt.show()

    # out
    fig = plt.gcf()
    #fig.set_size_inches(7, 5)
    out = f'DE2000-{i}Mbps-hdr-vdp.png'
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

def get_from_xlsx(filename):
    xlsx_file = pd.read_excel(filename, header=None)
    data = xlsx_file.iloc[:,0].values #take the first column
    return data

def dir_process(dir: str, dir1: str, i: int):
    #csv = dir + "deltaE2000.csv"
    #csv1 = dir1 + "deltaE2000.csv"
    #y = get_from_csv(csv)[1]
    #y1 = get_from_csv(csv1)[1]
    xlsx = dir + "deltaE2000.xlsx"
    xlsx1 = dir1 + "deltaE2000.xlsx"
    y = get_from_xlsx(xlsx)
    y1 = get_from_xlsx(xlsx1)

    #print(y)
    #print('\n')
    #print(y1)
    #print('\n')

    barplot(i, y, y1)

if __name__ == "__main__":
    for i in [4, 10, 20, 40]:
        dir = root + f"\\Test-{test_number}-SP\\{i}\\"
        dir1 = root + f"\\Test-{test_number}-AOI\\{i}\\"
        dir_process(dir, dir1, i)