# Disclaimer
> This folder contains a set of samples built on top of Bolt, that can be downloaded directly from the repository at https://github.com/BoltEngine/Bolt-Sample, where you can find the latest version of them or it can come inside the Bolt Free SDK from Asset Store (https://assetstore.unity.com/packages/tools/network/photon-bolt-free-127156).

# Bolt Samples

> This repository will contain a series of samples made using [Photon Bolt](https://www.photonengine.com/en-US/BOLT). They are intended to help understand how Bolt works and use it properly. This repository is **not** restricted to ExitGame developers and should be used by the community to learn, improve and share their own results using Bolt.

## Samples Documentation

This repository contains samples that highlight the most important Bolt features. The following samples are included:

| **Sample** 	| **Description** 	| **Documentation** 	| **Source** 	|
|--------------------------	|-------------------------------------------------------------------------------------------------	|:---------------------------------------------------------------------------------------------:	|:----------------------------:	|
| **Getting Started** 	| Overview on Bolt and its capabilities 	| [link](https://doc.photonengine.com/en-us/bolt/current/getting-started/bolt-101-wizard-setup) 	| [link](GettingStarted) 	|
| **Advanced Tutorial** 	| More complex example using Bolt 	| [link](https://doc.photonengine.com/en-us/bolt/current/advanced-tutorial/overview) 	| [link](AdvancedTutorial) 	|
| **PhotonCloud** 	| How to init Bolt to work on top of Photon Cloud or custom hosted Photon servers 	| --- 	| [link](PhotonCloud) 	|
| **ClickToMove** 	| A simple, server-authoritative example for moving a character by clicking on terrain 	| --- 	| [link](ClickToMove) 	|
| **ThirdPersonCharacter** 	| Multiplayer versions of the Unity TPC demo, both with and without server authoritative movement 	| --- 	| [link](ThirdPersonCharacter) 	|
| **HeadlessServer** 	| A simple example of how to run Bolt in headless mode for server purpose 	| [link](https://doc.photonengine.com/en-us/bolt/current/in-depth/headless-server) 	| [link](HeadlessServer) 	|
| **ServerMonitor** 	| Monitor a Bolt server using a standalone client. 	| --- 	| [link](NEW-ServerMonitor) 	|
| **Zeuz** 	| A simple example of dedicated Bolt server hosted by zeuz. Requires AdvancedTutorial. 	| WIP 	| [link](Zeuz) 	|

## Running the Samples

1. Create a new Unity project;
2. Download [Photon Bolt Free](https://assetstore.unity.com/packages/tools/network/photon-bolt-free-127156) from Asset Store;
3. Follow the instructions on the *Wizard* Window. If it did not show up, run it from `Window/Bolt/Wizard`;
4. Download this repository and place it inside your Unity Project. [Download link](https://github.com/BoltEngine/Bolt-Sample/archive/master.zip);
5. Replace the Bolt Assets files:
    1. `data/bolt.user.dll.backup` -> `Assets/Photon/PhotonBolt/assemblies/bolt.user.dll`;
    2. `data/project.bytes.backup` -> `Assets/Photon/PhotonBolt/project.bytes`
6. Compile Bolt: `Assets/Bolt/Compile Assembly`.

Any of the gameplay scenes from the included samples can be directly tested from Bolt's Scenes window (found on `Window/Bolt/Scenes` by following the instructions below:

1. Make sure the scene is added to Unity's `Build Settings`.
    - If you skip this, Bolt's Scene window won't show any scene.
2. Edit the `Player Settings` to make sure `Run in Background` is enabled!
    - The demos must be able to run in background. If you can't control the clients, or don't see coordinated movement, `Run in Background` is probably not set.
3. Open `Window/Bolt/Scenes` and click `Debug Start` to start any of the samples.
    - This will build and run the demo in Editor and (at least) 1 client.
