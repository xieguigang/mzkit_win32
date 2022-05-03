@echo off

title BioNovoGene MZKit developer tool

set PATH="%RSTUDIO_HOME%;%PATH%"

echo Welcome to the BioNovoGene MZKit developer tool
@echo:
echo **********************************************************************
echo ** BioNovoGene MZKit developer tool v1.0
echo ** Copyright (c) 2022 BioNovoGene Corporation
echo **********************************************************************
@echo:
echo Automation environment initialized done~
@echo:
echo RStudio pkg repository: "%pkg_attach%"
echo RStudio application home: "%RSTUDIO_HOME%"
echo R# engine config file: "%R_LIBS_USER%"
@echo:
echo Run R# script or python.NET script at here,
echo you can try:
@echo:
echo type "R# some_script.R" or
echo      "R# some_script.py" 
@echo:
echo in the command console to start automation work
@echo: