@mkdir "%ProgramFiles%\MSBuild\HSBuild"
@copy "%~dp0\msbuild\tasks\bin\Release\HSBuildTasks.dll" "%ProgramFiles%\MSBuild\HSBuild"
@copy "%~dp0\msbuild\*.targets" "%ProgramFiles%\MSBuild"
@copy "%~dp0\msbuild\*.props" "%ProgramFiles%\MSBuild"

