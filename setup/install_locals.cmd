@echo off

SET R_HOME=../bin/
SET R_ENV=%R_HOME%/R#.exe

%R_ENV% --install.packages "./GCModeller.zip"
%R_ENV% --install.packages "./ggplot.zip"
%R_ENV% --install.packages "./markdown2pdf.zip"
%R_ENV% --install.packages "./MSImaging.zip"
%R_ENV% --install.packages "./mzkit.zip"
%R_ENV% --install.packages "./REnv.zip"