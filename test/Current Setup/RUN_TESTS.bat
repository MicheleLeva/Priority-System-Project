@if (@CodeSection == @Batch) @then
@echo off

set SendKeys=CScript //nologo //E:JScript "%~F0"
set time=30

@REM set count="-"
@REM :loop

set exe=%userprofile%\Documents\UNIMI\TESI\Progetto\Builds\Test\PrioritySystemProject.exe
set logs=C:\Users\Wolly\AppData\LocalLow\DefaultCompany\PrioritySystemProject\LOGS

set /p t=Enter the test number 
echo Test number %t% start:
echo.

set x=40
set k="F4"
echo ======== %x% Mbps ==========
%SendKeys% "^{%k%}"
echo running...
start /W "" "%exe%" -mode client -time %time% @REM -show %count%
echo copying...
robocopy "%logs%" ".\Test-%t%\%x%" /E /is /it > nul
echo done
%SendKeys% "^{%k%}"
echo.


set x=20
set k="F3"
echo ======== %x% Mbps ==========
%SendKeys% "^{%k%}"
echo running...
start /W "" "%exe%" -mode client -time %time% @REM -show %count%
echo copying...
robocopy "%logs%" ".\Test-%t%\%x%" /E /is /it > nul
echo done
%SendKeys% "^{%k%}"
echo.

set x=10
set k="F2"
echo ======== %x% Mbps ==========
%SendKeys% "^{%k%}"
echo running...
start /W "" "%exe%" -mode client -time %time% @REM -show %count%
echo copying...
robocopy "%logs%" ".\Test-%t%\%x%" /E /is /it > nul
echo done
%SendKeys% "^{%k%}"
echo.

set x=4
set k="F1"
echo ======== %x% Mbps ==========
%SendKeys% "^{%k%}"
echo running...
start /W "" "%exe%" -mode client -time %time% @REM -show %count%
echo copying...
robocopy "%logs%" ".\Test-%t%\%x%" /E /is /it > nul
echo done
%SendKeys% "^{%k%}"
echo.


@REM set /a count=count-1
@REM if %count%==1 goto exitloop
@REM goto loop
@REM :exitloop

echo.
echo.
echo.
pause
@end

var WshShell = WScript.CreateObject("WScript.Shell");
WshShell.SendKeys(WScript.Arguments(0));