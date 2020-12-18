#!/bin/sh

DIR=/g/UnityProjects/UFrameIT

"/c/Program Files/Unity/Hub/Editor/2018.4.13f1/Editor/Unity.exe" -batchmode -buildTarget Win64 -projectPath "${DIR}" -buildWindows64Player "${DIR}"/Build/FrameWorld1.exe -quit 
"/c/Program Files/Unity/Hub/Editor/2018.4.13f1/Editor/Unity.exe" -batchmode -buildTarget Linux64 -projectPath "${DIR}" -buildLinux64Player "${DIR}"/Linux/frameworld.x86_64 -quit 
"/c/Program Files/Unity/Hub/Editor/2018.4.13f1/Editor/Unity.exe" -batchmode -buildTarget OSXUniversal -projectPath "${DIR}" -buildOSXUniversalPlayer "${DIR}"/FrameWorld.app -quit 

read -p "Press enter to continue"

rm "${DIR}"/Packed/*


zip -r "${DIR}"/Packed/FrameWorld_Windows.zip "${DIR}"/Build/ &

zip -r "${DIR}"/Packed/FrameWorld_Linux.zip "${DIR}"/Linux/ &
tar -cvf "${DIR}"/Packed/FrameWorld_Linux.tar "${DIR}"/Linux/ &

zip -r "${DIR}"/Packed/FrameWorld_Mac.zip "${DIR}"/FrameWorld.app/ &
tar -cvf "${DIR}"/Packed/FrameWorld_Mac.tar "${DIR}"/FrameWorld.app/ &

wait

hub release create -p -m "auto-build" v0.0

for file in "${DIR}"/Packed/* ; do hub release edit  -p -m "" -a "$file" v0.0; done

