@mkdir "%ProgramFiles%\MSBuild\HSBuild"
@copy "%~dp0\msbuild\tasks\bin\Release\*.dll" "%ProgramFiles%\MSBuild\HSBuild"
@copy "%~dp0\msbuild\*.targets" "%ProgramFiles%\MSBuild\HSBuild"
@copy "%~dp0\msbuild\*.props" "%ProgramFiles%\MSBuild\HSBuild"
@xcopy /I /Y /E "%~dp0\msbuild\Platform" "%ProgramFiles%\MSBuild\HSBuild\Platform"
@xcopy /I /Y /E "%~dp0\msbuild\templates" "%ProgramFiles%\MSBuild\HSBuild\templates"
@if exist "%ProgramFiles% (x86)" mklink /D "%ProgramFiles% (x86)\MSBuild\HSBuild" "%ProgramFiles%\MSBuild\HSBuild"

@mkdir "%ProgramFiles%\HSBuild"
@xcopy /I /Y /E "%~dp0\tools" "%ProgramFiles%\HSBuild\tools"

@mkdir "%ProgramFiles%\HSBuild\bin"
@copy "%~dp0\hsbuild\bin\Release\hsbuild.console.exe" "%ProgramFiles%\HSBuild\bin\hsbuild.exe"
@copy "%~dp0\hsbuild\bin\Release\HSbuild.Core.dll" "%ProgramFiles%\HSBuild\bin"
