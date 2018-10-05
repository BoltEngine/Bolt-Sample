# Bolt Samples

> This repository will contain a series of samples made using [Photon Bolt](https://www.photonengine.com/en-US/BOLT). They are intended to help understand how Bolt works and use it properly. This repository is **not** restricted to ExitGame developers and should be used by the community to learn, improve and share their own results using Bolt.

## Samples Content

This folder contains samples that highlight the most important Bolt features. The following samples are included:

- PhotonCloud: how to init Bolt to work on top of Photon Cloud or custom hosted Photon servers.
- ClickToMove: a simple, server-authoritative example for moving a character by clicking on terrain;
- ThirdPersonCharacter: multiplayer versions of the Unity TPC demo, both with and without server authoritative movement;
- AdvancedTutorial: assets and sources for Bolt's advanced tutorial.

Any of the gameplay scenes from the included samples can be directly tested from Bolt's Scenes window (found on `Window/Bolt/Scenes` by following the instructions bellow:

1. Make sure the scene is added to Unity's `Build Settings`.
> If you skip this, Bolt's Scene window won't show any scene.
2. Edit the `Player Settings` to make sure `Run in Background` is enabled!
> The demos must be able to run in background. If you can't control the clients, or don't see coordinated movement, `Run in Background` is probably not set.
3. Open `Window/Bolt/Scenes` and click `Debug Start` to start any of the samples.
> This will build and run the demo in Editor and (at least) 1 client.

## Main Documentation

You can find the main documentation of some samples from this repository here: 

| Sample Name          | Documentation                                                                                 | Source |
| -------------------- |:---------------------------------------------------------------------------------------------:| :-----:|
| Getting Started      | [link](https://doc.photonengine.com/en-us/bolt/current/getting-started/bolt-101-wizard-setup) | ---    |
| Advanced Tutorial    | [link](https://doc.photonengine.com/en-us/bolt/current/advanced-tutorial/overview)            | [link](AdvancedTutorial)    |