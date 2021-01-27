# Deployment of new UFrameIT Releases

## A Note on Versioning

We use semantic versioning. A version number looks like `x.y.z`, no `v` prefix.

Always release all of

- [UFrameIT/UFrameIT](https://github.com/UFrameIT/UFrameIT) (this repo)
- [FrameIT/frameworld](https://gl.mathhub.info/FrameIT/frameworld) (the MMT archive)
- [UFrameIT/archives](https://github.com/UFrameIT/archives) (Git aggregation repo of MMT archives)

at the same time with the *same* version number.

## Making a new UFrameIT Release

1. Deploy prereleases on the [UFrameIT/UFrameIT GitHub repo](https://github.com/UFrameIT/UFrameIT/releases) for all OS. See below how to do that.
2. Let people test.
3. Merge [FrameIT/frameworld](https://gl.mathhub.info/FrameIT/frameworld)'s devel branch into master and git-tag with `x.y.z`.
4. Update archive submodules in [UFrameIT/archives](https://github.com/UFrameIT/archives) and git-tag with `x.y.z`.
5. Git-tag [UFrameIT/UFrameIT](https://github.com/UFrameIT/UFrameIT) (this repo) with `x.y.z.`.
6. Publish prerelease from step 1.
7. Write a news post on our website: <https://github.com/UFrameIT/UFrameIT.github.io>

## Deploying Binaries for UFrameIT

### Archive type

- For Linux, deploy `.tar.gz` files (-> smallest file size as compared to `.zip` and `.tar`)
- For macOS, deploy `.dmg` files
- For Windows, deploy `.zip` files (-> guaranteed compatibility among end users since Windows Explorer can open them)

### Building Binaries (MultiBuild.bat can be used to do this automatically. MultiBuild.sh is maybe outdated and needs adjustments)
- Make sure Support for all build target platforms is installed:
    - Open Unity Hub -> Installs -> Choose Version -> Add Modules -> Check the following: Windows Build Support, Linux Build Support, Mac Build Support
						
- Make sure to copy a suitable frameit-server.jar and a corresponding archives-folder into "Assets/StreamingAssets":
    - For creating a frameit-server.jar with corresponding archives, follow https://github.com/UniFormal/MMT/tree/devel/src/frameit-mmt
			
- Make sure to build with the unity-version that's set as the default-version for the project
						
- Build Binaries (HINT: THE BUILD FOR macOS USUALLY ONLY WORKS IF IT'S BUILT FROM A MAC)
	- Via Command Line: see MultiBuild.bat
    - Via Unity-UI: File -> Build Settings... -> Choose "Target Platform" and "Architecture" -> Build
				
### Building Archives
- For Windows: zip -r \<TARGET-DIR\>/UFrameIT-\<version\>-Windows.zip \<SOURCE-DIR\>
- For Linux: tar -zcvf UFrameIT-\<version\>-Linux.tar.gz \<SOURCE-DIR\>
- For macOS: Create a dmg-file from the .app-file (Only possible on macOS)
	- mkdir UFrameIT-Image-Folder
	- cp \<UFrameIT-.app-file\> UFrameIT-Image-Folder/\<UFrameIT-.app-file\>
	- hdiutil create UFrameIT-\<version\>-Mac.dmg -volname "UFrameIT-\<version\>" -srcfolder UFrameIT-Image-Folder
	- For adjusting the appearance of the dmg, the image from "Assets/Images/dmg_background.png" can be used
		
### Hints for deployment
- The build for macOS usually only works if it's built from a Mac
- There was a Unity-Bug, where two different versions of Unity were pointing to the same Unity.exe. When you verify the version of the Unity.exe, with opening its details-panel, this version should match the version used for the build and the version that's set as the default-version for the Unity-Project. If that's not the case, this could lead to the following error on Linux: "Invalid serialized file version: .../FrameWorld_Data/globalgamemanagers. Expected version: 2019.4.0f1. Actual version: 2019.4.3f1"
- If the resulting binary does not work properly, please go through the player.log file (https://docs.unity3d.com/Manual/LogFiles.html)
    - Linux:	~/.config/unity3d/CompanyName/ProductName/Player.log
	- macOS:	~/Library/Logs/Company Name/Product Name/Player.log
	- Windows:		C:\Users\username\AppData\LocalLow\CompanyName\ProductName\Player.log
- For problems concerning the appearance of the game, please open the Unity-Project and adjust the Project-/Player-Settings: Edit -> Project Settings... -> Player -> Resolution and Presentation
- For changing the scenes that will be build please change the Build-Settings: File -> Build Settings... -> Scenes in Build
