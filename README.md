# UFrameIT

The FrameIT project builds a Framework for developing Serious Games by combining Virtual Worlds with Mathematical Knowledge Management. 
The UFrameIT framework uses the Unity game engine with the [MMT](https://uniformal.github.io/) system.
This repository contains the Unity project, which currently includes the framework itself and a demo game.
MMT itself is a large system with many different use cases beyond FrameIT.
It operates with [archives](https://github.com/UFrameIT/archives) of formalized knowledge.
For FrameIT, we added a new archive that, in turn, makes use of archives that existed before and contain knowledge about mathematics and logics. 
Unity and MMT communicate via the [FrameIT-Server](https://github.com/UniFormal/MMT/tree/devel/src/frameit-mmt)

For more information about the project, please visit <https://uframeit.org>

## Installation (for end users)

Just download our latest release: <https://github.com/UFrameIT/UFrameIT/releases/latest>

## Installation (for developers)

First, you have to set up a development environment:

1. Install [Unity](https://unity3d.com/de/get-unity/download) 2019.4.x (LTS) via the Unity Hub.

   We periodically update to the latest LTS version. Currently, any 2019.4 version should work; you can safely ignore any version warnings popping up.
   
   Make sure to [activate your license](https://support.unity.com/hc/en-us/articles/211438683-How-do-I-activate-my-license).
   
2. [Install Git LFS](https://docs.github.com/en/free-pro-team@latest/github/managing-large-files/installing-git-large-file-storage)
3. Clone this repository: `git clone --recurse-submodules https://github.com/UFrameIT/UFrameIT.git`
4. Follow the [UFrameIT server's guide on setting up a dev environment](https://github.com/UniFormal/MMT/blob/devel/src/frameit-mmt/DEVENV.md).

   Thereby, you will also install the necessary [archives of formalization UFrameIT/archives](https://github.com/UFrameIT/archives).

### Running

1. Open the Unity Hub and add the folder where you cloned this repository to. Then open the project in the hub.
2. Follow the [UFrameIT server installation guide](https://github.com/UniFormal/MMT/blob/devel/src/frameit-mmt/installation.md) to run the server
3. In Unity, select the scene you want to start (the current demo scene is called TreeWorld_02, you can find it at Assets/Scenes) and run the game (Play Button).

## Useful Links for new Developers
* [Publications](https://kwarc.github.io/bibs/frameit/)

### UFrameIT Unity Plugin
* [Installation Guide](https://github.com/UFrameIT/UFrameIT/blob/master/README.md)

### FrameIT Server
* [Installation Guide](https://github.com/UniFormal/MMT/blob/master/src/frameit-mmt/installation.md)
* [Documentation](https://github.com/UniFormal/MMT/blob/master/src/frameit-mmt/README.md)
* [FrameIT Server Request Examples](https://kwarc-frameit.postman.co/workspace/FrameIT-Postman-Workspace~d9c28eb0-b28c-47b9-8403-864de6799418/documentation/9866886-8347f2b8-c8a6-4c8b-800c-c4d3b01c9580)
* [MMT API Documentation](https://uniformal.github.io//doc/api/)

### Formalized Knowledge
* [Installation guide](https://github.com/UFrameIT/archives/blob/master/README.md)
* [How to write a Scroll](https://gl.mathhub.info/FrameIT/frameworld/-/tree/devel/source/Scrolls)
* [MMT Language Documentation](https://uniformal.github.io//doc/language/)
* [MMT Tutorials](https://uniformal.github.io//doc/tutorials/)

