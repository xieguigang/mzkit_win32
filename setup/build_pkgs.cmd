@echo off

SET drive=%~d0
SET R_HOME=%drive%\GCModeller\src\R-sharp\App\net6.0
SET Rscript="%R_HOME%/Rscript.exe"
SET REnv="%R_HOME%/R#.exe"

SET pkg=../../../dist\bin\Rstudio\packages\mzkit.zip

%Rscript% --build /src ../../../Rscript\Library\mzkit_app /save %pkg% --skip-src-build
%REnv% --install.packages %pkg%

pause