# UFrameIT



## What Quests are there 
1. Tree Stage
1.1. On map Tree Stage, measure the Height of the Tree near the villager.
2. River Stage
2.1. On map River Stage, measure the Height of the Tree near the villager.


## what Gameplay modes are there
1. First Person Gameplay
2. Third Person Gameplay
3. Escaperoom Gameplay
4. Sidescroller Gameplay


### 1.1 First person gameplay
- first person perspective 
- compatible with (old) Input Manager
- compatible with (new) Input System Package
- supports FrameITUI
- supports FrameITUI_mobile

 
### 1.2 First person gameplay (old mainplayer version)
- first person perspective  
- compatible with (old) Input Manager
- uses FrameITUI --> no Touchinput supported

 
### 2.1 Gameplay with Camera right behind shoulder
- third person perspective 
- player controls camera, which controls direct the playermodel

### 2.2 Third Person Gameplay with dampened Camera
- third person perspective 
- player controls Playermodel and the camera follows and rotates in a smoothed behavior.

### 2.3 Third Person Gameplay with manual Camera (For example Birdview)
- third person perspective 
- player controls Playermodel and the camera follows without rotating behavior. 

### 3.1 Escaperoom Gameplay
- Fixed Camera


### 4.1 Sidescroller Gameplay
- Only 2D Walking.


### How to set Options for Missions


## what options do:

### Control Options:

#### Set Control Mode
	- Keyboard & Mouse
	- Touch-Control
		- Touch-control Interfaces will be displayed.

#### Set Input Mode
	- Input_Manager
		- old input system
	- Input_System_Package
		- new input system

#### Keyboard Mouse Options
	- Look up or change Bindings

#### Touch-control Options
	- choose the Touch-Control mode you want to use


### System Options:
	
#### Operating System Recognition
	- saved in Application.persistentDataPath/Config/Network.JSON 
    - Defines what happends, when the Scene LaunchMenue is the first time called.
	- when recognition deactivated, the last safed configuration will be used
	- when recognition activated, Hardware will be detected and the configurations automatically adapted
	
#### Operating System Optimisation.
	- saved in Application.persistentDataPath/Config/Network.JSON 
    - After handling the "Operating System Recognition", it will displayed for which Operating System the App will be optimized.
	- Needed for handling the paths to folders for saving and loading files.
	- Deactivates the Mouse for Mobile systems like Android.

#### Select UI
	- saved in Application.persistentDataPath/Config/Network.JSON 
    - FrameITUI: 
		- Optimized for Keybord and Mouse
		- Supports Keybord and Mouse
	- FrameITUI_mobile:
		- Optimized for Touch-Controls 	
		- Supports Keybord and Mouse
		- Supports Touch-Controls

### Reset Options:
	- Following Reset Operations are handled by the Streaming AssetLoader.cs

#### Reset Configurations
	- Reloads files from Assets/StreamingsAssets to Application.persistentDataPath/Config/Network.JSON 
#### Reset DataPath
	- Reloads files from Assets/StreamingsAssets to Application.dataPath/Config/Network.JSON 
#### Reset Saved Games
	- Reloads files from Assets/StreamingsAssets to Application.dataPath/Config/Network.JSON 

#### Reset All Data
	- Reloads files from Assets/StreamingsAssets to Application.persistentDataPath/Config/Network.JSON 
	- Reloads files from Assets/StreamingsAssets to Application.dataPath/Config/Network.JSON 

### Gameplay Options:


#### Resize Cursor 
    - saved in Application.persistentDataPath/Config/Network.JSON 
	- (relevant when using a mouse)
	
#### Resize Hitbox for Mouseclicks 
 	- saved in Application.persistentDataPath/Config/Network.JSON 
 	- (Relevant for Touch Inputs)

### Network Options:
	You need to select a valid MMT Server which supports UFrameIT.
	UFrameIT needs a holding online connection to this server when playing.

## Known B

