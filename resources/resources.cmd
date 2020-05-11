@ECHO OFF

PATH=%PATH%;C:\Program Files (x86)\Microsoft SDKs\Windows\v7.0A\Bin;C:\Program Files\Microsoft SDKs\Windows\v7.0A\Bin

SETLOCAL ENABLEDELAYEDEXPANSION

REM Build Satellite assemblies

cd ..\resources\

for /F "tokens=2 delims=." %%i IN ('dir /B SharpWebMail.*.resources') DO call :resource %%i .%%i
goto :end

:resource
SET TARGET=bin\%1
SET FINALTARGET=..\%TARGET%
IF NOT EXIST %FINALTARGET% (
	mkdir %FINALTARGET%
)
al.exe /nologo /target:library /embed:SharpWebMail%2.resources,SharpWebMail.%1.resources /culture:%1 /out:%FINALTARGET%\SharpWebMail.resources.dll /template:%FINALTARGET%\..\SharpWebMail.dll

:end
ENDLOCAL
EXIT /B 0
