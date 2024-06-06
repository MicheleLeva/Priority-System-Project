@if (@CodeSection == @Batch) @then
@echo off

set SendKeys=CScript //nologo //E:JScript "%~F0"
set time=120
set mtime=30

set exe=%userprofile%\Documents\UNIMI\TESI\Progetto\Builds\Test\PrioritySystemProject.exe
REM set logs=C:\Users\Wolly\AppData\LocalLow\DefaultCompany\PrioritySystemProject\LOGS
set logs="/sdcard/Android/data/com.DefaultCompany.PrioritySystemProject/files/LOGS"
set mlogs="C:\Users\Wolly\AppData\Roaming\odh\device-metrics\2G0YC1ZF9K0C48"

set /p t=Enter the test number:
echo.

echo === TEST NUMBER %t% SCREEN PRESENCE START:
set pt=sp
echo.
echo.

REM Start the server with screen presence priority type
start "ScreenPresenceTestServer" "%exe%" -mode server -priorityType %pt%
sleep 15

set x=40
set k="F4"
echo ======== %x% Mbps ==========
%SendKeys% "^{%k%}"
echo running...
REM start /W "" "%exe%" -mode client -time %time% -priorityType %pt%
adb shell am start -n com.DefaultCompany.PrioritySystemProject/com.unity3d.player.UnityPlayerActivity -e unity "-mode/client/-time/10"
sleep %time%
mkdir ".\Test-%t%-SP\%x%"
adb pull %logs% ".\Test-%t%-SP\%x%"
sleep %mtime%
REM adb pull %mlogs% ".\Test-%t%-SP\%x%"
robocopy "%mlogs%" ".\Test-%t%-SP\%x%" /E /is /it > nul
echo done
%SendKeys% "^{%k%}"
echo.

set x=20
set k="F3"
echo ======== %x% Mbps ==========
%SendKeys% "^{%k%}"
echo running...
adb shell am start -n com.DefaultCompany.PrioritySystemProject/com.unity3d.player.UnityPlayerActivity -e unity "-mode/client/-time/10"
sleep %time%
mkdir ".\Test-%t%-SP\%x%"
adb pull %logs% ".\Test-%t%-SP\%x%"
sleep %mtime%
REM adb pull %mlogs% ".\Test-%t%-SP\%x%"
robocopy "%mlogs%" ".\Test-%t%-SP\%x%" /E /is /it > nul
echo done
%SendKeys% "^{%k%}"
echo.

set x=10
set k="F2"
echo ======== %x% Mbps ==========
%SendKeys% "^{%k%}"
echo running...
adb shell am start -n com.DefaultCompany.PrioritySystemProject/com.unity3d.player.UnityPlayerActivity -e unity "-mode/client/-time/10"
sleep %time%
mkdir ".\Test-%t%-SP\%x%"
adb pull %logs% ".\Test-%t%-SP\%x%"
sleep %mtime%
REM adb pull %mlogs% ".\Test-%t%-SP\%x%"
robocopy "%mlogs%" ".\Test-%t%-SP\%x%" /E /is /it > nul
echo done
%SendKeys% "^{%k%}"
echo.

set x=4
set k="F1"
echo ======== %x% Mbps ==========
%SendKeys% "^{%k%}"
echo running...
adb shell am start -n com.DefaultCompany.PrioritySystemProject/com.unity3d.player.UnityPlayerActivity -e unity "-mode/client/-time/10"
sleep %time%
mkdir ".\Test-%t%-SP\%x%"
adb pull %logs% ".\Test-%t%-SP\%x%"
sleep %mtime%
REM adb pull %mlogs% ".\Test-%t%-SP\%x%"
robocopy "%mlogs%" ".\Test-%t%-SP\%x%" /E /is /it > nul
echo done
%SendKeys% "^{%k%}"
echo.
echo.
echo.

REM Close the server with screen presence priority type
REM TASKKILL /IM %exe%
TASKKILL /FI "Windowtitle eq PrioritySystemProject" /F /T

echo === TEST NUMBER %t% AREAS OF INTEREST START:
set pt="aoi"
echo.
echo.

REM Start the server with areas of interest priority type
start "AreasOfInterestTestServer" "%exe%" -mode server -priorityType %pt%
sleep 15

set x=40
set k="F4"
echo ======== %x% Mbps ==========
%SendKeys% "^{%k%}"
echo running...
adb shell am start -n com.DefaultCompany.PrioritySystemProject/com.unity3d.player.UnityPlayerActivity -e unity "-mode/client/-time/10"
sleep %time%
mkdir ".\Test-%t%-AOI\%x%"
adb pull %logs% ".\Test-%t%-AOI\%x%"
sleep %mtime%
REM adb pull %mlogs% ".\Test-%t%-SP\%x%"
robocopy "%mlogs%" ".\Test-%t%-SP\%x%" /E /is /it > nul
echo done
%SendKeys% "^{%k%}"
echo.

set x=20
set k="F3"
echo ======== %x% Mbps ==========
%SendKeys% "^{%k%}"
echo running...
adb shell am start -n com.DefaultCompany.PrioritySystemProject/com.unity3d.player.UnityPlayerActivity -e unity "-mode/client/-time/10"
sleep %time%
mkdir ".\Test-%t%-AOI\%x%"
adb pull %logs% ".\Test-%t%-AOI\%x%"
sleep %mtime%
REM adb pull %mlogs% ".\Test-%t%-SP\%x%"
robocopy "%mlogs%" ".\Test-%t%-SP\%x%" /E /is /it > nul
echo done
%SendKeys% "^{%k%}"
echo.

set x=10
set k="F2"
echo ======== %x% Mbps ==========
%SendKeys% "^{%k%}"
echo running...
adb shell am start -n com.DefaultCompany.PrioritySystemProject/com.unity3d.player.UnityPlayerActivity -e unity "-mode/client/-time/10"
sleep %time%
mkdir ".\Test-%t%-AOI\%x%"
adb pull %logs% ".\Test-%t%-AOI\%x%"
sleep %mtime%
REM adb pull %mlogs% ".\Test-%t%-SP\%x%"
robocopy "%mlogs%" ".\Test-%t%-SP\%x%" /E /is /it > nul
echo done
%SendKeys% "^{%k%}"
echo.

set x=4
set k="F1"
echo ======== %x% Mbps ==========
%SendKeys% "^{%k%}"
echo running...
adb shell am start -n com.DefaultCompany.PrioritySystemProject/com.unity3d.player.UnityPlayerActivity -e unity "-mode/client/-time/10"
sleep %time%
mkdir ".\Test-%t%-AOI\%x%"
adb pull %logs% ".\Test-%t%-AOI\%x%"
sleep %mtime%
REM adb pull %mlogs% ".\Test-%t%-SP\%x%"
robocopy "%mlogs%" ".\Test-%t%-SP\%x%" /E /is /it > nul
echo done
%SendKeys% "^{%k%}"
echo.

REM Close the server with screen presence priority type
REM TASKKILL /IM %exe%
TASKKILL /FI "Windowtitle eq PrioritySystemProject" /F /T



echo === EVALUATION OF TEST %t% VIEW RATIO:
echo.

python.exe .\view_ratio_priorityTypes.py .\Test-%t%-SP\ .\Test-%t%-AOI\

pause

@end

var WshShell = WScript.CreateObject("WScript.Shell");
WshShell.SendKeys(WScript.Arguments(0));