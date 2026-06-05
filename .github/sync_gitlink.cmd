@echo off

REM git remote add gitee http://192.168.3.48:8418/xieguigang/mzkit_win32.git

git pull gitlink HEAD
git push gitee HEAD

echo synchronization of this code repository job done!