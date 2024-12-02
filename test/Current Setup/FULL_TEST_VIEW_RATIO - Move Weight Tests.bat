@if (@CodeSection == @Batch) @then
@echo off

set SendKeys=CScript //nologo //E:JScript "%~F0"
set time=30

REM set exe=%userprofile%\Documents\UNIMI\TESI\Progetto\Builds\Test\PrioritySystemProject.exe
set exe=F:\MicheleLeva\Progetto\TestBuild\PrioritySystemProject.exe
REM set logs=C:\Users\Wolly\AppData\LocalLow\DefaultCompany\PrioritySystemProject\LOGS
set logs=C:\Users\Michele Leva\AppData\LocalLow\DefaultCompany\PrioritySystemProject\LOGS

set /p t=Enter the test number:
echo.



REM FIRST ====================================================
echo === TEST NUMBER %t% SCREEN PRESENCE START:
set pt=sp
set d1=0.33
set d2=0.34
set d2=0.33
echo === WEIGHTS d1: %d1%, d2: %d2%, d3: %d3%
echo.
echo.

REM Start the server with screen presence priority type
start "ScreenPresenceTestServer" "%exe%" -mode server -priorityType %pt% -SetLocalIP true
powershell -Command "& {sleep 15}"

set x=40
set k="F4"
echo ======== %x% Mbps ==========
%SendKeys% "^{%k%}"
echo running...
start /W "" "%exe%" -mode client -time %time% -priorityType %pt% -SetLocalIP true -d1 %d1% -d2 %d2% -d3 %d3%
echo copying...
robocopy "%logs%" ".\Test-%t%-SP\%d1%_%d2%_%d3%\%x%" /E /is /it > nul
echo done
%SendKeys% "^{%k%}"
echo.

set x=20
set k="F3"
echo ======== %x% Mbps ==========
%SendKeys% "^{%k%}"
echo running...
start /W "" "%exe%" -mode client -time %time% -priorityType %pt% -SetLocalIP true -d1 %d1% -d2 %d2% -d3 %d3%
echo copying...
robocopy "%logs%" ".\Test-%t%-SP\%d1%_%d2%_%d3%\%x%" /E /is /it > nul
echo done
%SendKeys% "^{%k%}"
echo.

set x=10
set k="F2"
echo ======== %x% Mbps ==========
%SendKeys% "^{%k%}"
echo running...
start /W "" "%exe%" -mode client -time %time% -priorityType %pt% -SetLocalIP true -d1 %d1% -d2 %d2% -d3 %d3%
echo copying...
robocopy "%logs%" ".\Test-%t%-SP\%d1%_%d2%_%d3%\%x%" /E /is /it > nul
echo done
%SendKeys% "^{%k%}"
echo.

set x=4
set k="F1"
echo ======== %x% Mbps ==========
%SendKeys% "^{%k%}"
echo running...
start /W "" "%exe%" -mode client -time %time% -priorityType %pt% -SetLocalIP true -d1 %d1% -d2 %d2% -d3 %d3%
echo copying...
robocopy "%logs%" ".\Test-%t%-SP\%d1%_%d2%_%d3%\%x%" /E /is /it > nul
echo done
%SendKeys% "^{%k%}"
echo.
echo.
echo.

REM Close the server with screen presence priority type
REM TASKKILL /IM %exe%
TASKKILL /FI "Windowtitle eq PrioritySystemProject" /F /T


REM SECOND ====================================================
echo === TEST NUMBER %t% SCREEN PRESENCE START:
set pt=sp
set d1=1
set d2=0
set d2=0
echo === WEIGHTS d1: %d1%, d2: %d2%, d3: %d3%
echo.
echo.

REM Start the server with screen presence priority type
start "ScreenPresenceTestServer" "%exe%" -mode server -priorityType %pt% -SetLocalIP true
powershell -Command "& {sleep 15}"

set x=40
set k="F4"
echo ======== %x% Mbps ==========
%SendKeys% "^{%k%}"
echo running...
start /W "" "%exe%" -mode client -time %time% -priorityType %pt% -SetLocalIP true -d1 %d1% -d2 %d2% -d3 %d3%
echo copying...
robocopy "%logs%" ".\Test-%t%-SP\%d1%_%d2%_%d3%\%x%" /E /is /it > nul
echo done
%SendKeys% "^{%k%}"
echo.

set x=20
set k="F3"
echo ======== %x% Mbps ==========
%SendKeys% "^{%k%}"
echo running...
start /W "" "%exe%" -mode client -time %time% -priorityType %pt% -SetLocalIP true -d1 %d1% -d2 %d2% -d3 %d3%
echo copying...
robocopy "%logs%" ".\Test-%t%-SP\%d1%_%d2%_%d3%\%x%" /E /is /it > nul
echo done
%SendKeys% "^{%k%}"
echo.

set x=10
set k="F2"
echo ======== %x% Mbps ==========
%SendKeys% "^{%k%}"
echo running...
start /W "" "%exe%" -mode client -time %time% -priorityType %pt% -SetLocalIP true -d1 %d1% -d2 %d2% -d3 %d3%
echo copying...
robocopy "%logs%" ".\Test-%t%-SP\%d1%_%d2%_%d3%\%x%" /E /is /it > nul
echo done
%SendKeys% "^{%k%}"
echo.

set x=4
set k="F1"
echo ======== %x% Mbps ==========
%SendKeys% "^{%k%}"
echo running...
start /W "" "%exe%" -mode client -time %time% -priorityType %pt% -SetLocalIP true -d1 %d1% -d2 %d2% -d3 %d3%
echo copying...
robocopy "%logs%" ".\Test-%t%-SP\%d1%_%d2%_%d3%\%x%" /E /is /it > nul
echo done
%SendKeys% "^{%k%}"
echo.
echo.
echo.

REM Close the server with screen presence priority type
REM TASKKILL /IM %exe%
TASKKILL /FI "Windowtitle eq PrioritySystemProject" /F /T



REM THIRD ====================================================
echo === TEST NUMBER %t% SCREEN PRESENCE START:
set pt=sp
set d1=0
set d2=1
set d2=0
echo === WEIGHTS d1: %d1%, d2: %d2%, d3: %d3%
echo.
echo.

REM Start the server with screen presence priority type
start "ScreenPresenceTestServer" "%exe%" -mode server -priorityType %pt% -SetLocalIP true
powershell -Command "& {sleep 15}"

set x=40
set k="F4"
echo ======== %x% Mbps ==========
%SendKeys% "^{%k%}"
echo running...
start /W "" "%exe%" -mode client -time %time% -priorityType %pt% -SetLocalIP true -d1 %d1% -d2 %d2% -d3 %d3%
echo copying...
robocopy "%logs%" ".\Test-%t%-SP\%d1%_%d2%_%d3%\%x%" /E /is /it > nul
echo done
%SendKeys% "^{%k%}"
echo.

set x=20
set k="F3"
echo ======== %x% Mbps ==========
%SendKeys% "^{%k%}"
echo running...
start /W "" "%exe%" -mode client -time %time% -priorityType %pt% -SetLocalIP true -d1 %d1% -d2 %d2% -d3 %d3%
echo copying...
robocopy "%logs%" ".\Test-%t%-SP\%d1%_%d2%_%d3%\%x%" /E /is /it > nul
echo done
%SendKeys% "^{%k%}"
echo.

set x=10
set k="F2"
echo ======== %x% Mbps ==========
%SendKeys% "^{%k%}"
echo running...
start /W "" "%exe%" -mode client -time %time% -priorityType %pt% -SetLocalIP true -d1 %d1% -d2 %d2% -d3 %d3%
echo copying...
robocopy "%logs%" ".\Test-%t%-SP\%d1%_%d2%_%d3%\%x%" /E /is /it > nul
echo done
%SendKeys% "^{%k%}"
echo.

set x=4
set k="F1"
echo ======== %x% Mbps ==========
%SendKeys% "^{%k%}"
echo running...
start /W "" "%exe%" -mode client -time %time% -priorityType %pt% -SetLocalIP true -d1 %d1% -d2 %d2% -d3 %d3%
echo copying...
robocopy "%logs%" ".\Test-%t%-SP\%d1%_%d2%_%d3%\%x%" /E /is /it > nul
echo done
%SendKeys% "^{%k%}"
echo.
echo.
echo.

REM Close the server with screen presence priority type
REM TASKKILL /IM %exe%
TASKKILL /FI "Windowtitle eq PrioritySystemProject" /F /T



REM FOURTH ====================================================
echo === TEST NUMBER %t% SCREEN PRESENCE START:
set pt=sp
set d1=0
set d2=0
set d2=1
echo === WEIGHTS d1: %d1%, d2: %d2%, d3: %d3%
echo.
echo.

REM Start the server with screen presence priority type
start "ScreenPresenceTestServer" "%exe%" -mode server -priorityType %pt% -SetLocalIP true
powershell -Command "& {sleep 15}"

set x=40
set k="F4"
echo ======== %x% Mbps ==========
%SendKeys% "^{%k%}"
echo running...
start /W "" "%exe%" -mode client -time %time% -priorityType %pt% -SetLocalIP true -d1 %d1% -d2 %d2% -d3 %d3%
echo copying...
robocopy "%logs%" ".\Test-%t%-SP\%d1%_%d2%_%d3%\%x%" /E /is /it > nul
echo done
%SendKeys% "^{%k%}"
echo.

set x=20
set k="F3"
echo ======== %x% Mbps ==========
%SendKeys% "^{%k%}"
echo running...
start /W "" "%exe%" -mode client -time %time% -priorityType %pt% -SetLocalIP true -d1 %d1% -d2 %d2% -d3 %d3%
echo copying...
robocopy "%logs%" ".\Test-%t%-SP\%d1%_%d2%_%d3%\%x%" /E /is /it > nul
echo done
%SendKeys% "^{%k%}"
echo.

set x=10
set k="F2"
echo ======== %x% Mbps ==========
%SendKeys% "^{%k%}"
echo running...
start /W "" "%exe%" -mode client -time %time% -priorityType %pt% -SetLocalIP true -d1 %d1% -d2 %d2% -d3 %d3%
echo copying...
robocopy "%logs%" ".\Test-%t%-SP\%d1%_%d2%_%d3%\%x%" /E /is /it > nul
echo done
%SendKeys% "^{%k%}"
echo.

set x=4
set k="F1"
echo ======== %x% Mbps ==========
%SendKeys% "^{%k%}"
echo running...
start /W "" "%exe%" -mode client -time %time% -priorityType %pt% -SetLocalIP true -d1 %d1% -d2 %d2% -d3 %d3%
echo copying...
robocopy "%logs%" ".\Test-%t%-SP\%d1%_%d2%_%d3%\%x%" /E /is /it > nul
echo done
%SendKeys% "^{%k%}"
echo.
echo.
echo.

REM Close the server with screen presence priority type
REM TASKKILL /IM %exe%
TASKKILL /FI "Windowtitle eq PrioritySystemProject" /F /T



REM FIFTH ====================================================
echo === TEST NUMBER %t% SCREEN PRESENCE START:
set pt=sp
set d1=0.5
set d2=0.5
set d2=0
echo === WEIGHTS d1: %d1%, d2: %d2%, d3: %d3%
echo.
echo.

REM Start the server with screen presence priority type
start "ScreenPresenceTestServer" "%exe%" -mode server -priorityType %pt% -SetLocalIP true
powershell -Command "& {sleep 15}"

set x=40
set k="F4"
echo ======== %x% Mbps ==========
%SendKeys% "^{%k%}"
echo running...
start /W "" "%exe%" -mode client -time %time% -priorityType %pt% -SetLocalIP true -d1 %d1% -d2 %d2% -d3 %d3%
echo copying...
robocopy "%logs%" ".\Test-%t%-SP\%d1%_%d2%_%d3%\%x%" /E /is /it > nul
echo done
%SendKeys% "^{%k%}"
echo.

set x=20
set k="F3"
echo ======== %x% Mbps ==========
%SendKeys% "^{%k%}"
echo running...
start /W "" "%exe%" -mode client -time %time% -priorityType %pt% -SetLocalIP true -d1 %d1% -d2 %d2% -d3 %d3%
echo copying...
robocopy "%logs%" ".\Test-%t%-SP\%d1%_%d2%_%d3%\%x%" /E /is /it > nul
echo done
%SendKeys% "^{%k%}"
echo.

set x=10
set k="F2"
echo ======== %x% Mbps ==========
%SendKeys% "^{%k%}"
echo running...
start /W "" "%exe%" -mode client -time %time% -priorityType %pt% -SetLocalIP true -d1 %d1% -d2 %d2% -d3 %d3%
echo copying...
robocopy "%logs%" ".\Test-%t%-SP\%d1%_%d2%_%d3%\%x%" /E /is /it > nul
echo done
%SendKeys% "^{%k%}"
echo.

set x=4
set k="F1"
echo ======== %x% Mbps ==========
%SendKeys% "^{%k%}"
echo running...
start /W "" "%exe%" -mode client -time %time% -priorityType %pt% -SetLocalIP true -d1 %d1% -d2 %d2% -d3 %d3%
echo copying...
robocopy "%logs%" ".\Test-%t%-SP\%d1%_%d2%_%d3%\%x%" /E /is /it > nul
echo done
%SendKeys% "^{%k%}"
echo.
echo.
echo.

REM Close the server with screen presence priority type
REM TASKKILL /IM %exe%
TASKKILL /FI "Windowtitle eq PrioritySystemProject" /F /T



REM SIXTH ====================================================
echo === TEST NUMBER %t% SCREEN PRESENCE START:
set pt=sp
set d1=0.5
set d2=0
set d2=0.5
echo === WEIGHTS d1: %d1%, d2: %d2%, d3: %d3%
echo.
echo.

REM Start the server with screen presence priority type
start "ScreenPresenceTestServer" "%exe%" -mode server -priorityType %pt% -SetLocalIP true
powershell -Command "& {sleep 15}"

set x=40
set k="F4"
echo ======== %x% Mbps ==========
%SendKeys% "^{%k%}"
echo running...
start /W "" "%exe%" -mode client -time %time% -priorityType %pt% -SetLocalIP true -d1 %d1% -d2 %d2% -d3 %d3%
echo copying...
robocopy "%logs%" ".\Test-%t%-SP\%d1%_%d2%_%d3%\%x%" /E /is /it > nul
echo done
%SendKeys% "^{%k%}"
echo.

set x=20
set k="F3"
echo ======== %x% Mbps ==========
%SendKeys% "^{%k%}"
echo running...
start /W "" "%exe%" -mode client -time %time% -priorityType %pt% -SetLocalIP true -d1 %d1% -d2 %d2% -d3 %d3%
echo copying...
robocopy "%logs%" ".\Test-%t%-SP\%d1%_%d2%_%d3%\%x%" /E /is /it > nul
echo done
%SendKeys% "^{%k%}"
echo.

set x=10
set k="F2"
echo ======== %x% Mbps ==========
%SendKeys% "^{%k%}"
echo running...
start /W "" "%exe%" -mode client -time %time% -priorityType %pt% -SetLocalIP true -d1 %d1% -d2 %d2% -d3 %d3%
echo copying...
robocopy "%logs%" ".\Test-%t%-SP\%d1%_%d2%_%d3%\%x%" /E /is /it > nul
echo done
%SendKeys% "^{%k%}"
echo.

set x=4
set k="F1"
echo ======== %x% Mbps ==========
%SendKeys% "^{%k%}"
echo running...
start /W "" "%exe%" -mode client -time %time% -priorityType %pt% -SetLocalIP true -d1 %d1% -d2 %d2% -d3 %d3%
echo copying...
robocopy "%logs%" ".\Test-%t%-SP\%d1%_%d2%_%d3%\%x%" /E /is /it > nul
echo done
%SendKeys% "^{%k%}"
echo.
echo.
echo.

REM Close the server with screen presence priority type
REM TASKKILL /IM %exe%
TASKKILL /FI "Windowtitle eq PrioritySystemProject" /F /T



REM SEVENTH ====================================================
echo === TEST NUMBER %t% SCREEN PRESENCE START:
set pt=sp
set d1=0
set d2=0.5
set d2=0.5
echo === WEIGHTS d1: %d1%, d2: %d2%, d3: %d3%
echo.
echo.

REM Start the server with screen presence priority type
start "ScreenPresenceTestServer" "%exe%" -mode server -priorityType %pt% -SetLocalIP true
powershell -Command "& {sleep 15}"

set x=40
set k="F4"
echo ======== %x% Mbps ==========
%SendKeys% "^{%k%}"
echo running...
start /W "" "%exe%" -mode client -time %time% -priorityType %pt% -SetLocalIP true -d1 %d1% -d2 %d2% -d3 %d3%
echo copying...
robocopy "%logs%" ".\Test-%t%-SP\%d1%_%d2%_%d3%\%x%" /E /is /it > nul
echo done
%SendKeys% "^{%k%}"
echo.

set x=20
set k="F3"
echo ======== %x% Mbps ==========
%SendKeys% "^{%k%}"
echo running...
start /W "" "%exe%" -mode client -time %time% -priorityType %pt% -SetLocalIP true -d1 %d1% -d2 %d2% -d3 %d3%
echo copying...
robocopy "%logs%" ".\Test-%t%-SP\%d1%_%d2%_%d3%\%x%" /E /is /it > nul
echo done
%SendKeys% "^{%k%}"
echo.

set x=10
set k="F2"
echo ======== %x% Mbps ==========
%SendKeys% "^{%k%}"
echo running...
start /W "" "%exe%" -mode client -time %time% -priorityType %pt% -SetLocalIP true -d1 %d1% -d2 %d2% -d3 %d3%
echo copying...
robocopy "%logs%" ".\Test-%t%-SP\%d1%_%d2%_%d3%\%x%" /E /is /it > nul
echo done
%SendKeys% "^{%k%}"
echo.

set x=4
set k="F1"
echo ======== %x% Mbps ==========
%SendKeys% "^{%k%}"
echo running...
start /W "" "%exe%" -mode client -time %time% -priorityType %pt% -SetLocalIP true -d1 %d1% -d2 %d2% -d3 %d3%
echo copying...
robocopy "%logs%" ".\Test-%t%-SP\%d1%_%d2%_%d3%\%x%" /E /is /it > nul
echo done
%SendKeys% "^{%k%}"
echo.
echo.
echo.

REM Close the server with screen presence priority type
REM TASKKILL /IM %exe%
TASKKILL /FI "Windowtitle eq PrioritySystemProject" /F /T



REM AREAS OF INTEREST
echo === TEST NUMBER %t% AREAS OF INTEREST START:
set pt="aoi"
echo.
echo.

REM Start the server with areas of interest priority type
start "AreasOfInterestTestServer" "%exe%" -mode server -priorityType %pt% -SetLocalIP true
powershell -Command "& {sleep 15}"

set x=40
set k="F4"
echo ======== %x% Mbps ==========
%SendKeys% "^{%k%}"
echo running...
start /W "" "%exe%" -mode client -time %time% -priorityType %pt% -SetLocalIP true
echo copying...
robocopy "%logs%" ".\Test-%t%-AOI\%x%" /E /is /it > nul
echo done
%SendKeys% "^{%k%}"
echo.

set x=20
set k="F3"
echo ======== %x% Mbps ==========
%SendKeys% "^{%k%}"
echo running...
start /W "" "%exe%" -mode client -time %time% -priorityType %pt% -SetLocalIP true
echo copying...
robocopy "%logs%" ".\Test-%t%-AOI\%x%" /E /is /it > nul
echo done
%SendKeys% "^{%k%}"
echo.

set x=10
set k="F2"
echo ======== %x% Mbps ==========
%SendKeys% "^{%k%}"
echo running...
start /W "" "%exe%" -mode client -time %time% -priorityType %pt% -SetLocalIP true
echo copying...
robocopy "%logs%" ".\Test-%t%-AOI\%x%" /E /is /it > nul
echo done
%SendKeys% "^{%k%}"
echo.

set x=4
set k="F1"
echo ======== %x% Mbps ==========
%SendKeys% "^{%k%}"
echo running...
start /W "" "%exe%" -mode client -time %time% -priorityType %pt% -SetLocalIP true
echo copying...
robocopy "%logs%" ".\Test-%t%-AOI\%x%" /E /is /it > nul
echo done
%SendKeys% "^{%k%}"
echo.

REM Close the server with screen presence priority type
REM TASKKILL /IM %exe%
TASKKILL /FI "Windowtitle eq PrioritySystemProject" /F /T



echo === EVALUATION OF TEST %t% VIEW RATIO:
echo.

python.exe .\view_ratio_priorityType_Move.py .\Test-%t%-SP\ .\Test-%t%-AOI\

pause

@end

var WshShell = WScript.CreateObject("WScript.Shell");
WshShell.SendKeys(WScript.Arguments(0));