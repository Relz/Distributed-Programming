@echo off
start dotnet Frontend/Frontend.dll --configuration Release --launch-profile Production
start dotnet Backend/Backend.dll --configuration Release --launch-profile Production
start dotnet TextListener/TextListener.dll --configuration Release --launch-profile Production
start dotnet TextRankCalc/TextRankCalc.dll --configuration Release --launch-profile Production
start dotnet VowelConsonantCounter/VowelConsonantCounter.dll --configuration Release --launch-profile Production
start dotnet VowelConsonantRater/VowelConsonantRater.dll --configuration Release --launch-profile Production
