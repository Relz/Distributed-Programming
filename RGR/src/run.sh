#! /bin/bash

processNames=()
processArguments=()
processCounts=()

readProcesses()
{
	local flag=0
	for value in `jq '.process[] | (.name, .arguments, .count)' config/config.json`
	do
		if [ $flag = 0 ]; then
			flag=1
			processNames+=($value)
		else
			if [ $flag = 1 ]; then
				flag=2
				processArguments+=($value)
			else
				flag=0
				processCounts+=($value)
			fi
		fi
	done
}

runProcess()
{
	for (( i=0; i < $3; ++i ))
	do
		processID=-1
		xterm -hold -e "dotnet $1/$1.dll $2" & processID=$!
		echo $processID >> "pid"
	done
}

runProcesses()
{
	local index=0
	for processName in ${processNames[@]}
	do
		runProcess ${processNames[$index]} ${processArguments[$index]} ${processCounts[$index]}
		let index=$index+1
	done
}

truncate -s 0 "pid"

readProcesses
runProcesses
