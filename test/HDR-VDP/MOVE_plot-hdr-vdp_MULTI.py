import os
from os import listdir
from natsort import os_sorted
from os.path import *
import numpy as np
import matplotlib.pyplot as plt
import matplotlib.image as mpimg
import pandas as pd

root = f"hdrvdp-3.0.7\\test_move"
test_number = 4

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

def multiplot(dirSP, dirAOI, i):
    fullFrameList = os_sorted([f for f in listdir(root + "\\FullMoving") if isfile(join(root + "\\FullMoving", f))])
    SPFrameList = os_sorted([f for f in listdir(dirSP + "\\SCREEN") if isfile(join(dirSP + "\\SCREEN", f))])
    AOIFrameList = os_sorted([f for f in listdir(dirAOI + "\\SCREEN") if isfile(join(dirAOI + "\\SCREEN", f))])

    detectionSP = get_from_xlsx(dirSP + "vdp-hdr-detection.xlsx")
    detectionAOI = get_from_xlsx(dirAOI + "vdp-hdr-detection.xlsx")

    qualitySP = get_from_xlsx(dirSP + "vdp-hdr-quality.xlsx")
    qualityAOI = get_from_xlsx(dirAOI + "vdp-hdr-quality.xlsx")

    de2000SP = get_from_xlsx(dirSP + "deltaE2000.xlsx")
    de2000AOI = get_from_xlsx(dirAOI + "deltaE2000.xlsx")

    detectionMapSPList = os_sorted([f for f in listdir(dirSP + "\\vdp-hdr-detection") if isfile(join(dirSP + "\\vdp-hdr-detection", f))])
    detectionMapAOIList = os_sorted([f for f in listdir(dirAOI + "\\vdp-hdr-detection") if isfile(join(dirAOI + "\\vdp-hdr-detection", f))])

    counter = 0
    for f, s, a, ds, da in zip(fullFrameList, SPFrameList, AOIFrameList, detectionMapSPList, detectionMapAOIList):
        
        fig, axs = plt.subplots(3, 3)
        fig.tight_layout()

        img1 = mpimg.imread(dirSP + "\\SCREEN\\" + s)
        axs[0, 0].imshow(img1)
        axs[0, 0].set_title('Screen Presence')
        axs[0, 0].label_outer()
        axs[0, 0].set_xlabel(s)
        axs[0, 0].get_xaxis().set_ticks([])
        axs[0, 0].get_yaxis().set_ticks([])

        img2 = mpimg.imread(dirAOI + "\\SCREEN\\" + a)
        axs[0, 1].imshow(img2)
        axs[0, 1].set_title('Areas of Interest')
        axs[0, 1].label_outer()
        axs[0, 1].set_xlabel(a)
        axs[0, 1].get_xaxis().set_ticks([])
        axs[0, 1].get_yaxis().set_ticks([])

        img3 = mpimg.imread(root + "\\FullMoving\\" + f)
        axs[0, 2].imshow(img3)
        axs[0, 2].set_title('Full Moving')
        axs[0, 2].label_outer()
        axs[0, 2].set_xlabel(f)
        axs[0, 2].get_xaxis().set_ticks([])
        axs[0, 2].get_yaxis().set_ticks([])

        barWidth = 0.2

        axs[1, 0].bar([0],        [detectionSP[counter] - detectionAOI[counter] if detectionSP[counter] >= detectionAOI[counter] else 0], color ='r', width = barWidth, edgecolor ='grey', label ='SP') 
        axs[1, 0].bar([barWidth], [detectionAOI[counter] - detectionSP[counter] if detectionAOI[counter] >= detectionSP[counter] else 0], color ='b', width = barWidth, edgecolor ='grey', label ='AOI')
        axs[1, 0].set_title('Detection Difference')
        axs[1, 0].set_ylim([0, 1])

        axs[1, 1].bar([0],        [qualitySP[counter] - qualityAOI[counter] if qualitySP[counter] >= qualityAOI[counter] else 0], color ='r', width = barWidth, edgecolor ='grey', label ='SP') 
        axs[1, 1].bar([barWidth], [qualityAOI[counter] - qualitySP[counter] if qualityAOI[counter] >= qualitySP[counter] else 0], color ='b', width = barWidth, edgecolor ='grey', label ='AOI')
        axs[1, 1].set_title('Quality Difference')
        axs[1, 1].set_ylim([0, 10])  

        axs[1, 2].bar([0],        [de2000SP[counter] - de2000AOI[counter] if de2000SP[counter] >= de2000AOI[counter] else 0], color ='r', width = barWidth, edgecolor ='grey', label ='SP') 
        axs[1, 2].bar([barWidth], [de2000AOI[counter] - de2000SP[counter] if de2000AOI[counter] >= de2000SP[counter] else 0], color ='b', width = barWidth, edgecolor ='grey', label ='AOI')
        axs[1, 2].set_title('∆E2000 Difference')
        axs[1, 2].set_ylim([0, 100])    

        img4 = mpimg.imread(dirSP + "vdp-hdr-detection\\" + ds)
        axs[2, 0].imshow(img4)
        axs[2, 0].set_title('Screen Presence Pmap')
        axs[2, 0].label_outer()
        axs[2, 0].set_xlabel(s)
        axs[2, 0].get_xaxis().set_ticks([])
        axs[2, 0].get_yaxis().set_ticks([])

        img5 = mpimg.imread(dirAOI + "vdp-hdr-detection\\" + da)
        axs[2, 1].imshow(img5)
        axs[2, 1].set_title('Areas of Interest Pmap')
        axs[2, 1].label_outer()
        axs[2, 1].set_xlabel(a)
        axs[2, 1].get_xaxis().set_ticks([])
        axs[2, 1].get_yaxis().set_ticks([])

        img6 = mpimg.imread(root + "\\FullMoving\\" + f)
        axs[2, 2].imshow(img6)
        axs[2, 2].set_title('Full Moving')
        axs[2, 2].label_outer()
        axs[2, 2].set_xlabel(f)
        axs[2, 2].get_xaxis().set_ticks([])
        axs[2, 2].get_yaxis().set_ticks([])

        # out
        if not os.path.isdir(f'Test{test_number}\\{i}'):
            os.mkdir(f'Test{test_number}\\{i}')
        out = f'Test{test_number}\\{i}\\frame_{counter+1}'
        print(f"--> OUTPUT: {out}")
        fig.savefig(out, dpi=1200)
        plt.clf()
        plt.close()    

        counter += 1

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

    for i in [4, 10, 20, 40]:
        dirSP = root + f"\\Test-{test_number}-SP\\{i}\\"
        dirAOI = root + f"\\Test-{test_number}-AOI\\{i}\\"
        multiplot(dirSP, dirAOI, i)
        print("\n")
    
    