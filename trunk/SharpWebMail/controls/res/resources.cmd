@ECHO OFF

SETLOCAL ENABLEDELAYEDEXPANSION

REM Build Satellite assemblies

for /F "tokens=2 delims=." %%i IN ('dir /B SharpWebMail.*.resources') DO call :resource %%i .%%i
call :resource en
goto :end

:resource
SET TARGET=bin\%1
SET FINALTARGET=..\..\asp.net\%TARGET%
IF NOT EXIST %FINALTARGET% (
	mkdir %FINALTARGET%
)
al.exe /nologo /target:library /embed:SharpWebMail%2.resources,SharpWebMail.%1.resources /culture:%1 /out:%FINALTARGET%\SharpWebMail.resources.dll /template:%FINALTARGET%\..\SharpWebMail.dll

:end
ENDLOCAL
EXIT /B 0