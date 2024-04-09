@echo off

set /p x=Enter the tests to evaluate: 
echo Evaluation of %x% start:
echo.

python.exe .\view_ratio.py .\Test-%x%\ .\Test-%x%-np\

pause