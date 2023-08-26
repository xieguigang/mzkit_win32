@echo off

set blender=./plugins/blender/BlenderHost.exe 
set workbench=mzkit_win32

REM /b %workbench% --debug --port=33361 --blender=8331
"%blender%" /start --port 8331 --master debug-blender --debug