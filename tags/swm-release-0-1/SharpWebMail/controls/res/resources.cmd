@ECHO OFF

SETLOCAL ENABLEDELAYEDEXPANSION

REM Spanish resources

SET TARGET=bin\es
SET FINALTARGET=..\..\asp.net\%target%

IF NOT EXIST %FINALTARGET% (
	mkdir %FINALTARGET%
)

al /nologo /target:library /embed:SharpWebMail.es.resources /culture:es /out:%FINALTARGET%\SharpWebMail.resources.dll /template:%FINALTARGET%\..\SharpWebMail.dll

ENDLOCAL
EXIT /B 0