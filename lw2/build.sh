#! /bin/bash

if [ $# != 1 ]; then
	echo "./build.sh <version>"
	exit 1
fi

scriptPath="$( cd "$(dirname "$0")" ; pwd -P)"

mkdir -p $scriptPath/$1/config

cd $scriptPath/src/Frontend
dotnet publish --configuration Release --framework netcoreapp2.0 -o $scriptPath/$1/Frontend /property:PublishWithAspNetCoreTargetManifest=false
cd $scriptPath/src/Backend
dotnet publish --configuration Release --framework netcoreapp2.0 -o $scriptPath/$1/Backend /property:PublishWithAspNetCoreTargetManifest=false

cp $scriptPath/src/run.sh $scriptPath/$1
cp $scriptPath/src/stop.sh $scriptPath/$1
