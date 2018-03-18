@echo off
set argCount=0
for %%x in (%*) do (set /A argCount+=1)

if %argCount% NEQ 1 (
	echo "./build.cmd <version>"
	exit 1
)

set scriptPath="%cd%"

mkdir %scriptPath%\%1\config

cd %scriptPath%\src\Frontend
dotnet publish --configuration Release --framework netcoreapp2.0 -o %scriptPath%\%1\Frontend /property:PublishWithAspNetCoreTargetManifest=false
cd %scriptPath%\src\Backend
dotnet publish --configuration Release --framework netcoreapp2.0 -o %scriptPath%\%1\Backend /property:PublishWithAspNetCoreTargetManifest=false
cd %scriptPath%\src\TextListener
dotnet publish --configuration Release --framework netcoreapp2.0 -o %scriptPath%\%1\TextListener /property:PublishWithAspNetCoreTargetManifest=false
cd %scriptPath%\src\TextRankCalc
dotnet publish --configuration Release --framework netcoreapp2.0 -o %scriptPath%\%1\TextRankCalc /property:PublishWithAspNetCoreTargetManifest=false

cd %scriptPath%

copy %scriptPath%\src\run.cmd %scriptPath%\%1
copy %scriptPath%\src\stop.cmd %scriptPath%\%1
