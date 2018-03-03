#! /bin/bash

frontendPID=-1
backendPID=-1
textRankCalcPID=-1
exec dotnet Frontend/Frontend.dll --configuration Release --launch-profile Production & frontendPID=$!
exec dotnet Backend/Backend.dll --configuration Release --launch-profile Production & backendPID=$!
exec dotnet TextRankCalc/TextRankCalc.dll --configuration Release --launch-profile Production & textRankCalcPID=$!

truncate -s 0 "pid"
echo $frontendPID >> "pid"
echo $backendPID >> "pid"
echo $textRankCalcPID >> "pid"
