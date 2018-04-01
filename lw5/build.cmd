@echo off
set argCount=0
for %%x in (%*) do (set /A argCount+=1)

if %argCount% NEQ 1 (
	echo "./build.cmd <version>"
	exit 1
)

set buildDirectory=%1
set scriptPath="%cd%"

mkdir %scriptPath%\%buildDirectory%

call :build "Frontend"
call :build "Backend"
call :build "TextListener"
call :build "TextRankCalc"
call :build "VowelConsonantCounter"
call :build "VowelConsonantRater"

cd %scriptPath%

call :copy "run.sh" "F"
call :copy "run.cmd" "F"
call :copy "stop.sh" "F"
call :copy "stop.cmd" "F"
call :copy "config" "D"

exit /B %ERRORLEVEL%

:build
	cd %scriptPath%\src\%~1
	dotnet publish --configuration Release --framework netcoreapp2.0 -o %scriptPath%\%buildDirectory%\%~1 /property:PublishWithAspNetCoreTargetManifest=false
exit /B 0

:copy
	echo %~2 | xcopy /s %scriptPath%\src\%~1 %scriptPath%\%buildDirectory%\%~1 > NUL
exit /B 0
