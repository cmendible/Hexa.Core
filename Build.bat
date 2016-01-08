@echo off
if not exist "%programfiles(x86)%\Microsoft Visual Studio 14.0\Common7\Tools\VsMSBuildCmd.bat" Goto FileNotExists

echo Setting the MSBuild-VS2015 Environment...
call "%programfiles(x86)%\Microsoft Visual Studio 14.0\Common7\Tools\VsMSBuildCmd.bat"

echo.
echo Restoring the solution's packages before invoking MSBuild...
echo.
call ".nuget/NuGet.exe" restore Hexa.Core.sln

echo.
echo.
set /p answer="Want to Build(1) or Want to Build & Metrics(2) or Build & Test(3) or Build & Test & Metrics(4)?"
if "%answer%" EQU "1" call msbuild.exe Build.msbuild /fl
if "%answer%" EQU "2" call msbuild.exe Build.msbuild /fl /p:"EnableMetrics=true" /p:"EnableFxCop=true" /p:"EnableSimian=true"
if "%answer%" EQU "3" call msbuild.exe Build.msbuild /fl /p:"EnableNUnit=true"
if "%answer%" EQU "4" call msbuild.exe Build.msbuild /fl /p:"EnableMetrics=true" /p:"EnableFxCop=true" /p:"EnableSimian=true" /p:"EnableNUnit=true"
goto :TheEnd

:FileNotExists
echo The file "%programfiles(x86)%\Microsoft Visual Studio 14.0\Common7\Tools\VsMSBuildCmd.bat" not exists.
echo ERROR: Cannot setting the MSBuild-VS2015 Environment!
echo.

:TheEnd
pause