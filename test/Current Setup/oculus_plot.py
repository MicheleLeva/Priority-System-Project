from re import L
from statistics import median
import matplotlib.pyplot as plt
import csv
import sys
import numpy as np

# TO PRINT: python csv_plot.py <...csv_files_to_plot...> [Y-axis height]

def plot(d:str, filename:str, col:int, timecol:int, ysize:int, xax:str, title:str, delimiter:str=",", scatter:bool = False, priority:bool = False):    
    x = []
    y = []
    x1 = []
    y1 = []
    x2 = []
    y2 = []

    with open(filename,'r') as csvfile:
        plots = csv.reader(csvfile, delimiter = delimiter, skipinitialspace=True)
        init = -1
        
        for (i, row) in enumerate(plots):
            try:
                t = row[timecol]

                if (delimiter == " "):
                    t1 = t.split(":")
                    t = float(t1[1]) + float(t1[0])*60 
                    row[col]
                    
                float(t)
                float(row[col])
                if(init == -1):
                    init = float(t)
                
                if (not priority or float(row[3]) == 0):
                    x.append(float(t) - init)
                    y.append(float(row[col]))
                if (float(row[3]) == 1):
                    x1.append(float(t) - init)
                    y1.append(float(row[col]))
                if (float(row[3]) == 2):
                    x2.append(float(t) - init)
                    y2.append(float(row[col]))

            except: pass
        
    # Graph
    s = 5
    if (priority):
        s = 15

    if (scatter):
        plt.scatter(x, y, c='k', s=s, marker='x')
        plt.scatter(x1, y1, c='b', s=s, marker='*')
        plt.scatter(x2, y2, c='r', s=s, marker='^')
    else:
        plt.plot(x, y, color = 'k', linewidth=1)


    # X rotation
    plt.xticks(rotation = 90)

    # Axis size
    ax = plt.gca()
    ax.set_ylim([0, ysize])

    # Labels
    plt.xlabel('Time (s)')
    plt.ylabel(xax)

    if (priority):
        plt.legend([
            'Priority 0', 
            'Priority 1', 
            'Priority 2'
            ], loc = 'upper right')
    
    if ("RTT" in title):

        minn = np.min(y)
        plt.axhline(y=minn, linewidth=1, label=f"Min = {round(minn, 2)}", c="g")

        maxx = np.max(y)
        plt.axhline(y=maxx, linewidth=1, label=f"Max = {round(maxx, 2)}", c="r")

        mean = np.mean(y)
        plt.axhline(y=mean, linewidth=1, label=f"Mean = {round(mean, 2)}", c='b')
        
        # median = np.median(y)
        # plt.axhline(y=median, linewidth=1, label=f"Median = {round(median, 2)}", c="g")
        
        var = np.var(y)
        plt.axhline(y=var, linewidth=0, label=f"Variance = {round(var, 2)}", c="b")

        plt.legend(loc = 'upper right')

    # Title
    plt.title(title)

    # Size
    fig = plt.gcf()
    fig.set_size_inches(9, 6)
    out = d + "\\" + title.lower().replace("-","").replace("  ", " ").replace(" ", "_") + '.png'
    fig.savefig(out, dpi=100)
    print(f"saved {out}")

    plt.clf()



dirs = sys.argv[1:]

for d in dirs:
    d = d.strip("\"")
    test = d.strip(".").strip("\\")
    path = f"{d}"

    # try:
    #     plot(d=d, 
    #         filename= path + "log_objects_1.csv", 
    #         col= 2, 
    #         timecol= 0, 
    #         ysize= 130, 
    #         xax= "Distance (m)", 
    #         title= test + " - Objects Distance - No Zone - Distance Priority",
    #         scatter= True,
    #         priority= True)
    # except: pass

    # ------------------------------------------
    # FPS
    # ------------------------------------------
    plot(d=d, 
        filename=path + "log_client.csv", 
        col=1, 
        timecol=0, 
        ysize=150, 
        xax="FPS", 
        title=test + " - Log Client")

    plot(d=d, 
        filename=path + "log_server.csv", 
        col=1, 
        timecol=0, 
        ysize=1000, 
        xax="FPS", 
        title=test + " - Log Server")

    plot(d=d, 
        filename=path + "log_server.csv", 
        col=2, 
        timecol=0, 
        ysize=1000, 
        xax="RTT (ms)", 
        title=test + " - RTT")

        
    # ------------------------------------------
    # WIRESHARK
    # ------------------------------------------
    # plot(d=d, 
    #     filename=path + "traffic.csv", 
    #     col=5, 
    #     timecol=1, 
    #     ysize=70000, 
    #     xax="Packet Length (B)", 
    #     title=test + " - Packet Length")

    # try:    
    #     plot(d=d, 
    #         filename=path + "traffic.csv", 
    #         col=8, 
    #         timecol=1, 
    #         ysize=100000, 
    #         xax="Bytes (B)", 
    #         title=test + " - Bytes in Flight",
    #         scatter= True)
    # except: pass
        
    # ------------------------------------------
    # CPU - RAM
    # ------------------------------------------
    plot(d=d, 
        filename=path + "cpu.csv", 
        col=8, 
        timecol=10, 
        ysize=150, 
        xax="CPU (%)", 
        title=test + " - CPU",
        delimiter=" ")
        
    plot(d=d, 
        filename=path + "cpu.csv", 
        col=9, 
        timecol=10, 
        ysize=100, 
        xax="MEM (%)", 
        title=test + " - MEM",
        delimiter=" ")