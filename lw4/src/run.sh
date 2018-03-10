#! /bin/bash

frontendPID=-1
backendPID=-1
textRankCalcPID=-1
xterm -hold -e 'dotnet Frontend/Frontend.dll --configuration Release --launch-profile Production' & frontendPID=$!
xterm -hold -e 'dotnet Backend/Backend.dll --configuration Release --launch-profile Production' & backendPID=$!
xterm -hold -e 'dotnet TextRankCalc/TextRankCalc.dll --configuration Release --launch-profile Production' & textRankCalcPID=$!

truncate -s 0 "pid"
echo $frontendPID >> "pid"
echo $backendPID >> "pid"
echo $textRankCalcPID >> "pid"
