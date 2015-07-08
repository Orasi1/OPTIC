@ECHO OFF

IF NOT "%VUGEN_PATH%"=="" SET _TARGETPATH=%VUGEN_PATH%
IF NOT "%LR_PATH%"==""    SET _TARGETPATH=%LR_PATH%
IF NOT "%LR_ROOT%"==""    SET _TARGETPATH=%LR_ROOT%

IF NOT "%_TARGETPATH%"=="" (
	IF EXIST Optic.dll (
		ECHO[
		ECHO About to copy Optic.dll to "%_TARGETPATH%\bin". Press Ctrl+C to abort, or 
		PAUSE
		xcopy /vyd Optic.dll     "%_TARGETPATH%\bin"
		IF ERRORLEVEL 0 (
			ECHO Filed copy succeeded.
		) ELSE (
			ECHO Filed copy failed.
		)
	) ELSE (
		ECHO Optic.dll not found. File will not be copied.
		PAUSE
	)

	IF EXIST OpticUtil.dll (
		ECHO[
		ECHO About to copy OpticUtil.dll to "%_TARGETPATH%\bin\OpticUtil". Press Ctrl+C to abort, or 
		PAUSE
		xcopy /vyd OpticUtil.dll "%_TARGETPATH%\bin\OpticUtil\"
		IF ERRORLEVEL 0 (
			ECHO Filed copy succeeded.
		) ELSE (
			ECHO Filed copy failed.
		)
	) ELSE (
		ECHO OpticUtil.dll not found. File will not be copied.
		PAUSE
	)

) ELSE (
	ECHO[
	ECHO Environment variable LR_ROOT, LR_PATH, or VUGEN_PATH does not exist so no files will be copied. One of these variables
	ECHO needs to be set to the location of LoadRunner which is typically one of the following:
	ECHO -	"%ProgramFiles(x86)%\HP\Performance Center Host\"
	ECHO -	"%ProgramFiles(x86)%\HP\Virtual User Generator\"
)

ECHO[
ECHO About to install vcredist_64.exe. Press Ctrl+C to abort, or 
PAUSE
Start iexplore.exe "http://download.microsoft.com/download/2/E/6/2E61CFA4-993B-4DD4-91DA-3737CD5CD6E3/vcredist_x64.exe"


ECHO[
ECHO About to install vcredist_86.exe. Press Ctrl+C to abort, or 
PAUSE
Start iexplore.exe "http://download.microsoft.com/download/2/E/6/2E61CFA4-993B-4DD4-91DA-3737CD5CD6E3/vcredist_x86.exe"

ECHO[
ECHO %0 Complete.
ECHO[