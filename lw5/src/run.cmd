@echo off
setlocal enableDelayedExpansion

set processNames=
set processCounts=
set /a programCount=1

call :readProcesses programCount
call :runProcesses programCount

exit /B %ERRORLEVEL%

:readProcesses
	set /a result=1
	set /a flag=1
	for /f %%G in ('jq ".process[] | (.name, .count)" config/config.json') do (
		if !flag! EQU 1 (
			set /a flag=0
			set processNames[!result!]=%%G
		) else (
			set /a flag=1
			set processCounts[!result!]=%%G
			set /a result+=1
		)
	)
	set /a result-=1
	set %~1=!result!
exit /B 0

:runProcess
	for /l %%a in (1, 1, %~2) do (
		start dotnet %~1/%~1.dll --configuration Release --launch-profile Production
	)
exit /B 0

:runProcesses
	for /l %%a in (1, 1, !%~1!) do (
		call :runProcess !processNames[%%a]! !processCounts[%%a]!
	)
exit /B 0