@set PF=%ProgramFiles%
@if not "%ProgramFiles(x86)%" == "" set PF=%ProgramFiles(x86)%
@echo ProgramFiles set to %PF%

@if not exist "%PF%\MSBuild\HSBuild" mkdir "%PF%\MSBuild\HSBuild"
@if not exist "%PF%\HSBuild\bin" mkdir "%PF%\HSBuild\bin"

@copy "%~dp0\msbuild\tasks\bin\Release\*.dll" "%PF%\MSBuild\HSBuild"
@copy "%~dp0\msbuild\*.targets" "%PF%\MSBuild\HSBuild"
@copy "%~dp0\msbuild\*.props" "%PF%\MSBuild\HSBuild"
@xcopy /I /Y /E "%~dp0\msbuild\Platform" "%PF%\MSBuild\HSBuild\Platform"
@xcopy /I /Y /E "%~dp0\msbuild\templates" "%PF%\MSBuild\HSBuild\templates"

@copy "%~dp0\hsbuild\bin\x86\Release\hsbuild.console.exe" "%PF%\HSBuild\bin\hsbuild.exe"
@copy "%~dp0\hsbuild\bin\x86\Release\HSbuild.Core.dll" "%PF%\HSBuild\bin"
@copy "%~dp0\hsbuild\bin\x86\Release\ICSharpCode.SharpZipLib.dll" "%PF%\HSBuild\bin"

@rmdir /S /Q "%PF%\HSBuild\tools"
@xcopy /I /Y /E "%~dp0\tools" "%PF%\HSBuild\tools"
