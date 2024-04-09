# Framework for Client-Server Objects Streaming in VR
## Current Testbed Setup
Client test runs and log saves have been partially automated.

### On single PC (Server+Client)
1. on the Unity project: select the desired logs in the object `_LOGGER` of the scene
    - select the option `Move` to make the Player automatically move along a predefined path
2. build and open an instance of the project, then select the desired options in the upper-left panel
3. start the Server by pressing the corresponding button
4. run the [`RUN_TESTS.bat`](/PC/Set3%20(With%20Components%20Size)/RUN_TESTS.bat) script. The client instance is run using the following attributes:

        .\VR 3.exe" -mode <client\server> -time <duration in sec>

5. wait for the script to finish (if it shows some errors, check the correctness of the paths)
6. change the name of the folder `.\Test` or move it (otherwise it will be overridden on the next run!)
7. (Optionally) run the python scripts to plot the graphs. For example:

        python .\view_ratio.py .\Test-X\ .\Test-X-np\   
        python .\times.py .\Test-X\ .\Test-Y\

    ⚠️ np = test without priority queue. It is mandatory for the plot of _View Ratio_.

### On Oculus Quest
1. on the Unity project: select the desired logs in the object `_LOGGER` of the scene
    - select the option `Move` to make the Player automatically move along a predefined path
2. build for Android and install the apk on the Oculus Quest
3. start the and instance of the project as Server (on PC)
4. manually perform the test with the Oculus
5. close the app on the Quest
6. the logs are in the directory: `sdcard/Android/data/com.DefaultCompany.VR3/files/LOGS`
7. (Optionally) save also the Server Logs on the same folder (they can be found in `%userprofile%\AppData\LocalLow\DefaultCompany\VR 3\LOGS`)
7. (Optionally) the python scripts can be used to plot the graphs. For example:

        python oculus_plot.py .\4Mbps\ .\10Mbps\ ...

    ⚠️ Note that by default the script tries to plot Client and Server FPS, RTT, CPU and RAM. Plots can be easily enabled or disable by modifying the python script


## Older Tests
- [`/Oculus`](/Oculus) - Tests perfromed on Oculus Quest 2 | 120 Hz
    - CPU
    - RAM
    - Client FPS
    - Server FPS
    - Wireshark scan
    - Demo Video Captures

- [`/PC`](/PC) - Tests performed on PC, using the software [NetLimiter 4](https://www.netlimiter.com/products/nl4)
    - [`/PC/Set1/`](/PC/Set1/)
        - 5 ~ 60 ~ 80 screen
        - Fill Ratio
        - Spawn Latency
    - [`/PC/Set2 (Divided by Priority)/`](/PC/Set2%20(Divided%20by%20Priority)/)
        - 80 screen
        - View Ratio
        - Fill Ratio
        - Spawn Latency
    - [`/PC/Set3 (With Components Size)`](/PC/Set3%20(With%20Components%20Size)/)
        - 80 ~ 60 screen
        - View Ratio
        - Fill Ratio
        - Spawn Latency

## Author
- Federico Carboni - UniPD

## Supervisors
- Claudio Enrico Palazzi - UniPD
- Dario Maggiornini - UniMI