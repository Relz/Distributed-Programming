#! /bin/bash

if [ $# != 1 ]; then
	echo "./build.sh <version>"
	exit 1
fi

buildDirectory=$1
scriptPath="$( cd "$(dirname "$0")" ; pwd -P)"

build()
{
	cd $scriptPath/src/$1
	dotnet publish --configuration Release --framework netcoreapp2.0 -o $scriptPath/$buildDirectory/$1 /property:PublishWithAspNetCoreTargetManifest=false
}

copy()
{
	if [ -d $scriptPath/src/$1 ]; then
		cp $scriptPath/src/$1 $scriptPath/$buildDirectory/$1 -r
	else
		cp $scriptPath/src/$1 $scriptPath/$buildDirectory/$1
	fi
}

programs=(
	"Frontend"
	"Backend"
	"TextListener"
	"TextRankCalc"
	"VowelConsonantCounter"
	"VowelConsonantRater" )

elementsToCopy=(
	"run.sh"
	"run.cmd"
	"stop.sh"
	"stop.cmd"
	"config" )

for program in ${programs[@]}
do
	build $program
done

for element in ${elementsToCopy[@]}
do
	copy $element
done
