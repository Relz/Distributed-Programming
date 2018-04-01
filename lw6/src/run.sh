#! /bin/bash

processNames=()
processCounts=()

readProcesses()
{
	local flag=true
	for value in `jq '.process[] | (.name, .count)' config/config.json`
	do
		if [ $flag = true ]; then
			flag=false
			processNames+=($value)
		else
			flag=true
			processCounts+=($value)
		fi
	done
}

runProcess()
{
	for (( i=0; i < $2; ++i ))
	do
		processID=-1
		xterm -hold -e "dotnet $1/$1.dll --configuration Release --launch-profile Production" & processID=$!
		echo $processID >> "pid"
	done
}

runProcesses()
{
	local index=0
	for processName in ${processNames[@]}
	do
		runProcess ${processNames[$index]} ${processCounts[$index]}
		let index=$index+1
	done
}

truncate -s 0 "pid"

readProcesses
runProcesses
