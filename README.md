# UFrameIT

The FrameIT project builds a Framework for developing Serious Games by combining Virtual Worlds with Mathematical Knowledge Management. 
The UFrameIT framework uses the Unity game engine with the [MMT](https://uniformal.github.io/) system.
This repository contains the Unity project, which currently includes the framework itself and a demo game.

For more information about the project, please visit <https://uframeit.org>

## Installation (for end users)

Just download our latest release: <https://github.com/UFrameIT/UFrameIT/releases/latest>

## Installation (for developers)

First, you have to set up a development environment:

1. Install [Unity](https://unity3d.com/de/get-unity/download) 2019.4.x (LTS) via the Unity Hub.

   We periodically update to the latest LTS version. Currently, any 2019.4 version should work; you can safely ignore any version warnings popping up.
   
   Make sure to [activate your license](https://support.unity.com/hc/en-us/articles/211438683-How-do-I-activate-my-license-#:~:text=Log%20into%20your%20Unity%20ID,input%20your%20new%20License%20key).
   
2. Clone this repository: `git clone --recurse-submodules https://github.com/UFrameIT/UFrameIT.git`
3. Follow the [UFrameIT server installation guide](https://github.com/UniFormal/MMT/blob/devel/src/frameit-mmt/installation.md).

   Thereby, you will also install the necessary [archives of formalization UFrameIT/archives](https://github.com/UFrameIT/archives).

### Running

1. Open the Unity Hub and add the folder where you cloned this repository to. Then open the project in the hub.
2. Follow the [UFrameIT server installation guide](https://github.com/UniFormal/MMT/blob/devel/src/frameit-mmt/installation.md) to run the server
3. In Unity, select the scene you want to start (the current demo scene is called TreeWorld_02, you can find it at Assets/Scenes) and run the game (Play Button).
