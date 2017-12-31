@echo Off
SETLOCAL
set config=%1

if "%config%" == "" (
   set config=release
)

REM Some unit-tests may leave nuget.config files in %TEMP% which leads to hard-to-debug failures
FOR /F "tokens=*" %%I IN ('dir /s /b "%TEMP%\nuget.config" 2^>NUL') DO DEL "%%I"

REM Dev10 and Dev11 msbuild path
set nugetmsbuildpath="%WINDIR%\Microsoft.NET\Framework\v4.0.30319\msbuild.exe"

REM Dev12 msbuild path
set nugetmsbuildpathtmp="%ProgramFiles%\MSBuild\12.0\bin\msbuild.exe"
if exist %nugetmsbuildpathtmp% set nugetmsbuildpath=%nugetmsbuildpathtmp%
set nugetmsbuildpathtmp="%ProgramFiles(x86)%\MSBuild\12.0\bin\msbuild.exe"
if exist %nugetmsbuildpathtmp% set nugetmsbuildpath=%nugetmsbuildpathtmp%

REM Dev14 msbuild path
set nugetmsbuildpathtmp="%ProgramFiles%\MSBuild\14.0\bin\msbuild.exe"
if exist %nugetmsbuildpathtmp% set nugetmsbuildpath=%nugetmsbuildpathtmp%
set nugetmsbuildpathtmp="%ProgramFiles(x86)%\MSBuild\14.0\bin\msbuild.exe"
if exist %nugetmsbuildpathtmp% set nugetmsbuildpath=%nugetmsbuildpathtmp%
set EnableNuGetPackageRestore=true
%nugetmsbuildpath% Build\Build.CommandLine.proj /p:Configuration="%config%" /p:Platform="Any CPU" /p:TargetFrameworkVersion="v4.0" /m /v:M /fl /flp:LogFile=msbuild.log;Verbosity=Detailed /nr:false /target:GoMono

ENDLOCAL
