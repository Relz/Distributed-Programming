#! /bin/bash

frontendPID=-1
backendPID=-1
textListenerPID=-1
exec dotnet Frontend/Frontend.dll --configuration Release --launch-profile Production & frontendPID=$!
exec dotnet Backend/Backend.dll --configuration Release --launch-profile Production & backendPID=$!
exec dotnet TextListener/TextListener.dll --configuration Release --launch-profile Production & textListenerPID=$!

truncate -s 0 "pid"
echo $frontendPID >> "pid"
echo $backendPID >> "pid"
echo $textListenerPID >> "pid"
