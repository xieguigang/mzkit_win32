@echo off

SET drive=%~d0
SET R_HOME=%drive%\GCModeller\src\R-sharp\App\net6.0
SET Rscript="%R_HOME%/Rscript.exe"
SET REnv="%R_HOME%/R#.exe"
SET pkg_repo=../../../dist\bin\Rstudio\packages

SET pkg=%pkg_repo%/mzkit.zip

%Rscript% --build /src ../../../Rscript\Library\mzkit_app /save %pkg% --skip-src-build
%REnv% --install.packages %pkg%

SET pkg=%pkg_repo%/MSImaging.zip

%Rscript% --build /src ../../../Rscript\Library\MSI_app /save %pkg% --skip-src-build
%REnv% --install.packages %pkg%

SET pkg=%pkg_repo%/REnv.zip

%Rscript% --build /src %drive%\GCModeller\src\R-sharp\REnv /save %pkg% --skip-src-build
%REnv% --install.packages %pkg%

SET pkg=%pkg_repo%/GCModeller.zip

%Rscript% --build /src %drive%\GCModeller\src\workbench\pkg /save %pkg% --skip-src-build
%REnv% --install.packages %pkg%

SET pkg=%pkg_repo%/markdown2pdf.zip

%Rscript% --build /src %drive%\GCModeller\src\workbench\markdown2pdf /save %pkg% --skip-src-build
%REnv% --install.packages %pkg%

SET pkg=%pkg_repo%/ggplot.zip

%Rscript% --build /src %drive%\GCModeller\src\runtime\ggplot /save %pkg% --skip-src-build
%REnv% --install.packages %pkg%

pause