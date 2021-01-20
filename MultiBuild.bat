@echo off
set /p=For more information about the proper deployment of UFrameIT, please have a look into Deploy.md. Hit ENTER to continue...
set /p=Please make sure to copy a working frameit-server.jar with a suitable archives-folder into /Assets/StreamingAssets of your Unity-Project. Hit ENTER to continue...
set /p=Please make sure unity-support for all build target platforms is installed. Hit ENTER to continue...
set /p=Please make sure to build with the unity-version that's set as the default for the project. Hit ENTER to continue...
set /p unityDir=Please enter your absolute unity installation path: 
set /p projectPath=Please enter your absolute project path: 

echo Windows-Build...
start "Windows-Build" /D "%projectPath%" /W "%unityDir%"/Unity.exe -batchmode -buildTarget Win64 -projectPath "%projectPath%" -buildWindows64Player Build/FrameWorld.exe -quit
if %errorlevel% neq 0 (
	set /p=An error occured. Hit ENTER to exit...
	exit %errorlevel%
)

echo Linux-Build...
start "Linux-Build" /D "%projectPath%" /W "%unityDir%"/Unity.exe -batchmode -buildTarget Linux64 -projectPath "%projectPath%" -buildLinux64Player Linux/FrameWorld.x86_64 -quit
if %errorlevel% neq 0 (
	set /p=An error occured. Hit ENTER to exit...
	exit %errorlevel%
)

echo Mac-Build... (The Mac-Build will usually work only if it was built from a Mac)
start "Mac-Build" /D "%projectPath%" /W "%unityDir%"/Unity.exe -batchmode -buildTarget OSXUniversal -projectPath "%projectPath%" -buildOSXUniversalPlayer FrameWorld.app -quit
if %errorlevel% neq 0 (
	set /p=An error occured. Hit ENTER to exit...
	exit %errorlevel%
)

echo Creating Windows zip-file...
start "Windows-zip" /D "%projectPath%" /W zip -r UFrameIT-Windows.zip Build

echo Creating Linux tgz-file...
start "Linux-tgz" /D "%projectPath%" /W tar -zcvf UFrameIT-Linux.tar.gz Linux

echo Creating Mac tgz-file... (Please use macOS to create a dmg-file from the .app-file, see DEPLOY.md)
start "Mac-tgz" /D "%projectPath%" /W tar -zcvf UFrameIT-Mac.tar.gz FrameWorld.app