@echo off

SET msbuild_logger=%CD%
SET dir=%CD%
SET drive=%~d0
SET R_HOME=%drive%\GCModeller\src\R-sharp\App\net8.0
SET Rscript="%R_HOME%/Rscript.exe"
SET REnv="%R_HOME%/R#.exe"
SET pkg_repo=../../../dist\bin\Rstudio\packages
SET Rstudio=../../../dist\bin\Rstudio
SET GCModeller_src=%drive%\GCModeller\src
SET mzkit_src=%drive%\mzkit\Rscript\Library
SET erica_src=%drive%\Erica
SET js_url_mzkit="https://mzkit.org/assets/js/R_syntax.js"
SET js_url_gcmodeller="https://gcmodeller.org/lib/R_syntax.js"
SET js_url_rsharp="https://rdocumentation.rsharp.net/assets/R_syntax.js"

if "%1"=="--Rpackage" (
	goto :jump_to_build_Rpackages
)

goto :start_msbuild

REM ----===== msbuild function =====----
:exec_msbuild
SETLOCAL
SET _src=%1
SET _sln=%2
SET logfile="%msbuild_logger%/%_sln%.txt"

:echo
echo "build %_sln% package"

REM clean works and rebuild libraries
REM cd %base%
cd %_src%

echo "VisualStudio workspace:"
echo " --> %CD%"

dotnet msbuild ./%_sln% -target:Clean
dotnet msbuild ./%_sln% -t:restore 
dotnet msbuild ./%_sln% -t:Rebuild /p:Configuration="Rsharp_app_release" /p:Platform="x64" -detailedSummary:True -verbosity:minimal > %logfile% & type %logfile%

REM cd %base%

:echo
:echo
echo "build package %_sln% job done!"
echo "---------------------------------------------------------"
:echo
:echo

ENDLOCAL & SET _result=0
goto :%jump%

REM ----===== end of function =====----

:start_msbuild

cd %GCModeller_src%

SET jump=renv
CALL :exec_msbuild "%GCModeller_src%/R-sharp" "./R_system.NET5.sln"
:renv

SET jump=erica
CALL :exec_msbuild "%erica_src%/src" "./Erica.sln"
:erica

SET jump=reportKit
CALL :exec_msbuild "%GCModeller_src%/runtime/markdown2pdf/src" "./reportKit.NET5.sln"
:reportKit

SET jump=gcmodeller
CALL :exec_msbuild "%GCModeller_src%/workbench/R#" "./packages.NET5.sln"
:gcmodeller

SET jump=ggplot
CALL :exec_msbuild "%GCModeller_src%/runtime/ggplot" "./ggplot.NET5.sln"
:ggplot

SET jump=msimaging
CALL :exec_msbuild "%mzkit_src%/MSI_app" "./MSImaging.sln"
:msimaging

SET jump=mzkit_pkg
CALL :exec_msbuild "%mzkit_src%" "./mzkit.NET5.sln"
:mzkit_pkg

REM -------- end of run msbuild -----------

:jump_to_build_Rpackages

cd %msbuild_logger%

SET pkg=%pkg_repo%/Erica.zip

%Rscript% --build /src %erica_src% /save %pkg% --skip-src-build --github-page %js_url_gcmodeller%
%REnv% --install.packages %pkg%

SET pkg=%pkg_repo%/mzkit.zip

%Rscript% --build /src ../../../Rscript\Library\mzkit_app /save %pkg% --skip-src-build  --github-page %js_url_mzkit%
%REnv% --install.packages %pkg%

SET pkg=%pkg_repo%/MSImaging.zip

%Rscript% --build /src ../../../Rscript\Library\MSI_app /save %pkg% --skip-src-build  --github-page %js_url_mzkit%
%REnv% --install.packages %pkg%

SET pkg=%pkg_repo%/REnv.zip

%Rscript% --build /src %drive%\GCModeller\src\R-sharp\REnv /save %pkg% --skip-src-build --github-page %js_url_rsharp%
%REnv% --install.packages %pkg%

SET pkg=%pkg_repo%/GCModeller.zip

%Rscript% --build /src %drive%\GCModeller\src\workbench\pkg /save %pkg% --skip-src-build  --github-page %js_url_gcmodeller%
%REnv% --install.packages %pkg%

SET pkg=%pkg_repo%/markdown2pdf.zip

%Rscript% --build /src %drive%\GCModeller\src\runtime\markdown2pdf /save %pkg% --skip-src-build --github-page %js_url_rsharp%
%REnv% --install.packages %pkg%

SET pkg=%pkg_repo%/ggplot.zip

%Rscript% --build /src %drive%\GCModeller\src\runtime\ggplot /save %pkg% --skip-src-build --github-page %js_url_rsharp%
%REnv% --install.packages %pkg%

SET pkg=%pkg_repo%/languageserver.zip

%Rscript% --build /src %drive%\mini-R\languageserver /save %pkg% --skip-src-build --github-page %js_url_rsharp%
%REnv% --install.packages %pkg%


REM copy the required assembly dll file
REM
REM copy command will not working if the source
REM file name has the quot character inside?
SET dll=wkhtmltopdf/Newtonsoft.Json.dll

cd %dir%

echo %CD%
echo %dir%
echo %dll%
echo %Rstudio%

copy /Y /V %dll% "%Rstudio%/bin/"
copy /Y /V %dll% "%Rstudio%/host/"
copy /Y /V %dll% "%Rstudio%/library/"

pause
exit 0