echo Setting VS 2010 Environment...

if exist "%programfiles(x86)%\Microsoft Visual Studio 10.0\VC\vcvarsall.bat" call "%programfiles(x86)%\Microsoft Visual Studio 10.0\VC\vcvarsall.bat" x86
if exist "%programfiles%\Microsoft Visual Studio 10.0\VC\vcvarsall.bat"	call "%programfiles%\Microsoft Visual Studio 10.0\VC\vcvarsall.bat" x86
if exist "%programfiles(x86)%\Microsoft Visual Studio 11.0\VC\vcvarsall.bat" call "%programfiles(x86)%\Microsoft Visual Studio 11.0\VC\vcvarsall.bat" x86
if exist "%programfiles%\Microsoft Visual Studio 11.0\VC\vcvarsall.bat"	call "%programfiles%\Microsoft Visual Studio 11	.0\VC\vcvarsall.bat" x86

set /p answer="Want to Build(1) or Build & Test(2) or Build & Test & Metrics(3)?"

if "%answer%" EQU "1" call msbuild.exe Build.msbuild /fl 
if "%answer%" EQU "2" call msbuild.exe Build.msbuild /fl /p:"EnableNUnit=true"
if "%answer%" EQU "3" call msbuild.exe Build.msbuild /fl /p:"EnableMetrics=true" /p:"EnableFxCop=true" /p:"EnableSimian=true" /p:"EnableNUnit=true"

pause