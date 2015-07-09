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
	) ELSE (
		ECHO Optic.dll not found. File will not be copied.
		PAUSE
	)

	IF EXIST OpticUtil.dll (
		ECHO[
		ECHO About to copy OpticUtil.dll to "%_TARGETPATH%\bin\OpticUtil". Press Ctrl+C to abort, or 
		PAUSE
		xcopy /vyd OpticUtil.dll "%_TARGETPATH%\bin\OpticUtil\"
	) ELSE (
		ECHO OpticUtil.dll not found. File will not be copied.
		PAUSE
	)

	REM If VuGen exists, copy the VuGen Addin
	IF NOT "%VUGEN_PATH%"=="" (
		IF EXIST OpticVuGenAddin.dll (
			ECHO[
			ECHO About to copy OpticVuGenAddin.dll to "%VUGEN_PATH%\Addins\extra\OpticVuGenAddin\". Press Ctrl+C to abort, or 
			PAUSE
			xcopy /vyd OpticVuGenAddin.dll "%VUGEN_PATH%\Addins\extra\OpticVuGenAddin\"
		) ELSE (
			ECHO OpticVuGenAddin.dll not found. File will not be copied.
			PAUSE
		)

		IF EXIST OpticVuGenAddin.addin (
			ECHO[
			ECHO About to copy OpticVuGenAddin.addin to "%VUGEN_PATH%\Addins\extra\OpticVuGenAddin\". Press Ctrl+C to abort, or 
			PAUSE
			xcopy /vyd OpticVuGenAddin.addin "%VUGEN_PATH%\Addins\extra\OpticVuGenAddin\"
		) ELSE (
			ECHO OpticVuGenAddin.addin not found. File will not be copied.
			PAUSE
		)

		IF EXIST OpticUtil.dll (
			ECHO[
			ECHO About to copy OpticUtil.dll to "%VUGEN_PATH%\Addins\extra\OpticVuGenAddin\". Press Ctrl+C to abort, or 
			PAUSE
			xcopy /vyd OpticUtil.dll "%VUGEN_PATH%\Addins\extra\OpticVuGenAddin\"
		) ELSE (
			ECHO OpticUtil.dll not found. File will not be copied.
			PAUSE
		)
	) ELSE (
		ECHO[
		ECHO VuGen does not exist so not copying OpticVuGenAddin.
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