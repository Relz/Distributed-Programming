#! /bin/bash

if [ $# != 1 ]; then
	echo "./build.sh <version>"
	exit 1
fi

while IFS='.' read -ra semverParts; do
	if [ ${#semverParts[@]} != 3 ]; then
		echo "<version> not seems to be a semver version"
		exit 2
	fi
done <<< "$1"

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
	"VowelConsonantRater"
	"TextStatistics" )

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
