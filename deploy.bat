@mkdir "%ProgramFiles%\MSBuild\HSBuild"
@copy "%~dp0\msbuild\tasks\bin\Release\HSBuildTasks.dll" "%ProgramFiles%\MSBuild\HSBuild"
@copy "%~dp0\msbuild\*.targets" "%ProgramFiles%\MSBuild\HSBuild"
@copy "%~dp0\msbuild\*.props" "%ProgramFiles%\MSBuild\HSBuild"
@xcopy /I /Y /E "%~dp0\msbuild\templates" "%ProgramFiles%\MSBuild\HSBuild\templates"

