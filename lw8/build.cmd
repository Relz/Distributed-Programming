@echo off
set argCount=0
for %%x in (%*) do (set /A argCount+=1)

if %argCount% NEQ 1 (
	echo "build.cmd <version>"
	exit /B 1
)

set buildDirectory=%1

setlocal enableDelayedExpansion
for /f "tokens=1,2,3 delims=." %%a in ("%buildDirectory%") do (
	set /a result=1
	set first=%%a
	set second=%%b
	set third=%%c
	if not defined first (
		set result=0
	)
	if not defined second (
		set result=0
	)
	if not defined third (
		set result=0
	)
	if !result! EQU 0 (
		echo "<version> not seems to be a semver version"
		exit /B 1
	)
)

set scriptPath="%cd%"

mkdir %scriptPath%\%buildDirectory%

call :packLibrary "ConstantLibrary"
call :packLibrary "ModelLibrary"
call :packLibrary "RabbitMqLibrary"
call :packLibrary "RedisLibrary"

call :buildProgram "Frontend"
call :buildProgram "Backend"
call :buildProgram "TextListener"
call :buildProgram "TextRankCalc"
call :buildProgram "VowelConsonantCounter"
call :buildProgram "VowelConsonantRater"
call :buildProgram "TextStatistics"
call :buildProgram "TextProcessingLimiter"
call :buildProgram "TextSuccessMarker"

cd %scriptPath%

call :copy "run.sh" "F"
call :copy "run.cmd" "F"
call :copy "stop.sh" "F"
call :copy "stop.cmd" "F"
call :copy "config" "D"

exit /B %ERRORLEVEL%

:packLibrary
	cd %scriptPath%\src\%~1
	dotnet pack --configuration Release -o %scriptPath%\src\Libraries
exit /B 0

:buildProgram
	cd %scriptPath%\src\%~1
	dotnet publish --configuration Release --framework netcoreapp2.0 -o %scriptPath%\%buildDirectory%\%~1 /property:PublishWithAspNetCoreTargetManifest=false
exit /B 0

:copy
	echo %~2 | xcopy /s %scriptPath%\src\%~1 %scriptPath%\%buildDirectory%\%~1 > NUL
exit /B 0
