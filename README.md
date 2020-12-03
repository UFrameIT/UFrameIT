# UFrameIT

The FrameIT project builds a Framework for developing Serious Games by combining Virtual Worlds with Mathematical Knowledge Management. 
The UFrameIT framework uses the Unity game engine with the [MMT](https://uniformal.github.io/) system.
This repository contains the Unity project, which currently includes the framework itself and a demo game.

For more information about the project, please visit <https://uframeit.github.io/>

## Installation (for end users)

Just download our latest release: <https://github.com/UFrameIT/UFrameIT/releases/latest>

## Installation (for developers)

First, you have to set up a development environment:

1. Install [Unity](https://unity3d.com/de/get-unity/download) 2019.4.x (LTS) via the Unity Hub. We periodically update to the latest LTS version. Currently, any 2019.4 version should work; you can safely ignore any version warnings popping up.
2. Clone this repository: `git clone https://github.com/UFrameIT/UFrameIT.git`
3. Get the [FrameIT archives](https://github.com/UFrameIT/archives), make sure to always update the submodules.
4. Install [IntelliJ IDEA](https://www.jetbrains.com/de-de/idea/)
5. Clone [MMT](https://github.com/UniFormal/MMT/tree/devel), which also contains the [FrameIT MMT server](https://github.com/UniFormal/MMT/tree/devel/src/frameit-mmt), on the *devel* branch: `git clone --branch devel https://github.com/UniFormal/MMT.git`
and [install](https://github.com/UniFormal/MMT/blob/devel/src/frameit-mmt/installation.md) the Server Component

### Running

1. Open the Unity Hub and add the folder where you have cloned this repository. Then, you can directly open the project in the hub.
2. Follow the [FrameIT MMT server's usage guide](https://github.com/UniFormal/MMT/tree/devel/src/frameit-mmt) for how to start the FrameIT MMT server
3. Run the game in unity
