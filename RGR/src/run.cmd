@echo off
setlocal enableDelayedExpansion

set processNames=
set processArguments=
set processCounts=
set /a programCount=1

call :readProcesses programCount
call :runProcesses programCount

exit /B %ERRORLEVEL%

:readProcesses
	set /a result=1
	set /a flag=0
	for /f %%G in ('jq ".process[] | (.name, .arguments, .count)" config/config.json') do (
		if !flag! EQU 0 (
			set /a flag=1
			set processNames[!result!]=%%G
		) else (
			if !flag! EQU 1 (
				set /a flag=2
				set processArguments[!result!]=%%G
			) else (
				set /a flag=0
				set processCounts[!result!]=%%G
				set /a result+=1
			)
		)
	)
	set /a result-=1
	set %~1=!result!
exit /B 0

:runProcess
	for /l %%a in (1, 1, %~3) do (
		start "%~1 %~2" cmd /c "cd %~1 && dotnet %~1.dll %~2"
	)
exit /B 0

:runProcesses
	for /l %%a in (1, 1, !%~1!) do (
		call :runProcess !processNames[%%a]! !processArguments[%%a]! !processCounts[%%a]!
	)
exit /B 0