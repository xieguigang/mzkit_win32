# mzkit_win32

Mzkit Windows Desktop Application

## Source development of MZKit

1. Git clone of source projects and all of the submodules inside the repository:
   + [MZKit](https://github.com/xieguigang/mzkit.git)
   + [GCModeller](https://github.com/SMRUCC/GCModeller.git)

2. Development of MZKit in VisualStudio:
   + open the visual studio solution file in mzkit source folder: "src/mzkit/mzkit_win32.sln"

3. Build R language environment in VisualStudio:
   + open the visual studio solution file in GCModeller source folder: "src/R-sharp/R_system.NET5.sln"
   
4. Build R environment:
   + run the batch file in mzkit source folder: "src/mzkit/setup/build_pkgs.cmd" 
   
5. Finally build the installer package project:
   + open the project file in ``Advanced Installer`` in mzkit source folder: "src/mzkit/setup/mzkit_setups.aip"
