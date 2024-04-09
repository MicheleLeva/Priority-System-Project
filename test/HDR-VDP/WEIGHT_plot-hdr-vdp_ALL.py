import os
from os import listdir
from natsort import os_sorted
from os.path import *
import numpy as np
import matplotlib.pyplot as plt
import matplotlib.image as mpimg
import pandas as pd

root = f"hdrvdp-3.0.7\\test_move_weights"
test_number = 1

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
    plt.ylabel('Quality Difference', fontweight ='bold', fontsize = 15) 
    plt.xticks([r + barWidth for r in range(len(SP))], [a for a in range(len(SP))])
     
    #plt.legend()
    #plt.show()

    # out
    out = f'QualityDifference-{i}Mbps-hdr-vdp.png'
    print(f"--> OUTPUT: {out}\n")
    fig.savefig(out, dpi=100)
    plt.clf()

def detectionPlot(dirSP, SPweights, dirAOI, speed):
    #######detection

    #get AOI values
    detectionAOI = get_from_xlsx(dirAOI + "vdp-hdr-detection.xlsx")

    detectionSPLists = []
    for i in range(0, len(SPweights)):
        detectionSPLists.append(get_from_xlsx(f"{dirSP}\\{SPweights[i]}\\{speed}\\vdp-hdr-detection.xlsx"))
    weightsDetectionsDict = {k : v for k, v in zip(SPweights, detectionSPLists)}
    
    #start plot
    fig, axs = plt.subplots(3, 3)
    fig.tight_layout()

    fig.suptitle("Detection difference")

    barWidth = 0.2

    for x in range(0, 3):
        for y in range(0, 3):
            if ((2*y + x) > len(weightsDetectionsDict)): break

            weight = list(weightsDetectionsDict.keys())[2*y + x]
            detectionSP = weightsDetectionsDict[weight]

            #we only want the differences in the graph for better visual clarity
            zSP =  [x - x1 if x >= x1 else 0 for x, x1 in zip(detectionSP, detectionAOI)]
            zAOI = [x1 - x if x1 >= x else 0 for x, x1 in zip(detectionSP, detectionAOI)]

            # Set position of bar on X axis 
            br1 = np.arange(len(zSP)) 
            br2 = [x + barWidth for x in br1]

            # Make the plot
            axs[x, y].bar(br1, zSP, color ='r', width = barWidth, label ='SP') 
            axs[x, y].bar(br2 , zAOI, color ='b', width = barWidth, label ='AOI')
            axs[x, y].set_title(f'{weight}')
            axs[x, y].set_ylim([0, 1])

    # out
    if not os.path.isdir(f'Test{test_number}\\{speed}'):
        os.mkdir(f'Test{test_number}\\{speed}')
    out = f'Test{test_number}\\{speed}\\Detection_difference'
    print(f"--> OUTPUT: {out}")
    fig.savefig(out, dpi=1200)
    plt.clf()
    plt.close()

def qualityPlot(dirSP, SPweights, dirAOI, speed):
    #######quality

    #get AOI values
    qualityAOI = get_from_xlsx(dirAOI + "vdp-hdr-quality.xlsx")

    qualitySPLists = []
    for i in range(0, len(SPweights)):
        qualitySPLists.append(get_from_xlsx(f"{dirSP}\\{SPweights[i]}\\{speed}\\vdp-hdr-quality.xlsx"))
    weightsQualityDict = {k : v for k, v in zip(SPweights, qualitySPLists)}
    
    #start plot
    fig, axs = plt.subplots(3, 3)
    fig.tight_layout()

    fig.suptitle("Quality difference", va = 'top')

    barWidth = 0.2

    for x in range(0, 3):
        for y in range(0, 3):
            if ((2*y + x) > len(weightsQualityDict)): break

            weight = list(weightsQualityDict.keys())[2*y + x]
            qualitySP = weightsQualityDict[weight]

            #we only want the differences in the graph for better visual clarity
            zSP =  [x - x1 if x >= x1 else 0 for x, x1 in zip(qualitySP, qualityAOI)]
            zAOI = [x1 - x if x1 >= x else 0 for x, x1 in zip(qualitySP, qualityAOI)]

            # Set position of bar on X axis 
            br1 = np.arange(len(zSP)) 
            br2 = [x + barWidth for x in br1]

            # Make the plot
            axs[x, y].bar(br1, zSP, color ='r', width = barWidth, label ='SP') 
            axs[x, y].bar(br2 , zAOI, color ='b', width = barWidth, label ='AOI')
            axs[x, y].set_title(f'{weight}')
            axs[x, y].set_ylim([0, 10])

    # out
    if not os.path.isdir(f'Test{test_number}\\{speed}'):
        os.mkdir(f'Test{test_number}\\{speed}')
    out = f'Test{test_number}\\{speed}\\Quality_difference'
    print(f"--> OUTPUT: {out}")
    fig.savefig(out, dpi=1200)
    plt.clf()
    plt.close()

def de2000Plot(dirSP, SPweights, dirAOI, speed):
    #######de2000

    #get AOI values
    de2000AOI = get_from_xlsx(dirAOI + "deltaE2000.xlsx")

    de2000SPLists = []
    for i in range(0, len(SPweights)):
        de2000SPLists.append(get_from_xlsx(f"{dirSP}\\{SPweights[i]}\\{speed}\\vdp-hdr-quality.xlsx"))
    weightsDe2000Dict = {k : v for k, v in zip(SPweights, de2000SPLists)}
    
    #start plot
    fig, axs = plt.subplots(3, 3)
    fig.tight_layout()

    fig.suptitle("De2000 difference", va = 'top')

    barWidth = 0.2

    for x in range(0, 3):
        for y in range(0, 3):
            if ((2*y + x) > len(weightsDe2000Dict)): break

            weight = list(weightsDe2000Dict.keys())[2*y + x]
            de2000SP = weightsDe2000Dict[weight]

            #we only want the differences in the graph for better visual clarity
            zSP =  [x - x1 if x >= x1 else 0 for x, x1 in zip(de2000SP, de2000AOI)]
            zAOI = [x1 - x if x1 >= x else 0 for x, x1 in zip(de2000SP, de2000AOI)]

            # Set position of bar on X axis 
            br1 = np.arange(len(zSP)) 
            br2 = [x + barWidth for x in br1]

            # Make the plot
            axs[x, y].bar(br1, zSP, color ='r', width = barWidth, label ='SP') 
            axs[x, y].bar(br2 , zAOI, color ='b', width = barWidth, label ='AOI')
            axs[x, y].set_title(f'{weight}')
            axs[x, y].set_ylim([0, 100])

    # out
    if not os.path.isdir(f'Test{test_number}\\{speed}'):
        os.mkdir(f'Test{test_number}\\{speed}')
    out = f'Test{test_number}\\{speed}\\DE2000_difference'
    print(f"--> OUTPUT: {out}")
    fig.savefig(out, dpi=1200)
    plt.clf()
    plt.close()   

def multiplot(dirSP, SPweights, dirAOI, i):

    detectionPlot(dirSP, SPweights, dirAOI, i)
    qualityPlot(dirSP, SPweights, dirAOI, i)
    de2000Plot(dirSP, SPweights, dirAOI, i)

def get_from_xlsx(filename):
    xlsx_file = pd.read_excel(filename, header=None)
    data = xlsx_file.iloc[:,0].values #take the first column
    return data

def dir_process(dirSP: str, dirAOI: str, i: int):
    xlsxSP = dirSP + "vdp-hdr-quality.xlsx"
    xlsxAOI = dirAOI + "vdp-hdr-quality.xlsx"

    y = get_from_xlsx(xlsxSP)
    y1 = get_from_xlsx(xlsxAOI)

    z = [x - x1 if x >= x1 else 0 for x, x1 in zip(y, y1)]
    z1 = [x1 - x if x1 >= x else 0 for x, x1 in zip(y, y1)]

    barplot(i, z, z1)

if __name__ == "__main__":
    if not os.path.isdir(f'Test{test_number}'):
        os.mkdir(f'Test{test_number}')

    dirSP = root + f"\\Test-{test_number}-SP"
    SPweights = os_sorted([f for f in listdir(dirSP) if isdir(join(dirSP, f))])

    for i in [4, 10, 20, 40]:
        print(f"---{i}")
        dirAOI = root + f"\\Test-{test_number}-AOI\\{i}\\"
        multiplot(dirSP, SPweights, dirAOI, i)
        print("\n")
    
    