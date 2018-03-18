#! /bin/bash

frontendPID=-1
backendPID=-1
textListenerPID=-1
textRankCalcPID=-1
vowelConsonantCounterPID=-1
vowelConsonantRaterPID=-1
xterm -hold -e 'dotnet Frontend/Frontend.dll --configuration Release --launch-profile Production' & frontendPID=$!
xterm -hold -e 'dotnet Backend/Backend.dll --configuration Release --launch-profile Production' & backendPID=$!
xterm -hold -e 'dotnet TextListener/TextListener.dll --configuration Release --launch-profile Production' & textListenerPID=$!
xterm -hold -e 'dotnet TextRankCalc/TextRankCalc.dll --configuration Release --launch-profile Production' & textRankCalcPID=$!
xterm -hold -e 'dotnet VowelConsonantCounter/VowelConsonantCounter.dll --configuration Release --launch-profile Production' & vowelConsonantCounterPID=$!
xterm -hold -e 'dotnet VowelConsonantRater/VowelConsonantRater.dll --configuration Release --launch-profile Production' & vowelConsonantRaterPID=$!

truncate -s 0 "pid"
echo $frontendPID >> "pid"
echo $backendPID >> "pid"
echo $textListenerPID >> "pid"
echo $textRankCalcPID >> "pid"
echo $vowelConsonantCounterPID >> "pid"
echo $vowelConsonantRaterPID >> "pid"
