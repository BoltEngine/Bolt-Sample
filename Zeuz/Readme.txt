This is a simple example to demonstrate/help you/build/run dedicated server hosted by zeuz.
Please read documentation before you start.

Requirements
========================================================================================================================
1) Bolt Pro
2) Zeuz account
3) Advanced Tutorial package

How to build dedicated server
========================================================================================================================
1) Make sure these scenes are in build settings - Server, Level1
2) Assets => Bolt => Compile Assembly
3) Enable Run in background
4) Build
5) Upload to your repository
6) Wait for synchronization and service initialization

How to build client
========================================================================================================================
1) Set Server Group ID and Game Profiles on ZeuzGame prefab according to your configuration in zeuz dashboard
2) Make sure these scenes are in build settings - Client, Level1
3) Assets => Bolt => Compile Assembly
4) Enable Run in background
5) Build
6) Run

By default, you need 2 clients to trigger server reservation, you can change this on the ZeuzGame game object (property Max Players In Room).

You should be able to run the client from Unity Editor.
