@echo off

set /p x=Enter the tests to evaluate: 
echo Evaluation of %x% start:
echo.

python.exe .\times.py .\Test-%x%\ .\Test-%x%-np\

pause