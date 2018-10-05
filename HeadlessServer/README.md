# Bolt Headless Server sample

In order to start a Bolt Server follow the next steps:

1. If you want to change the default configuration, open the `HeadlessServer/Scenes/BoltHeadlessServer` scene on your editor:
    1. Click on the `HeadlessServerHolder` game object;
    2. Update the values of `Map` (scene name that will be loaded by the server), `Game Type` (just an example of a custom property) and `Room ID` (name of the created room, if you let it empty, a random name will be created).
2. Build a version of your game using `BoltHeadlessServer` scene as the Scene with index `0`. **Don't forget** to run the Bolt Compiler (`Assets/Bolt/Compile Assembly`).
3. From the `Command Line`, locate your executable and run:
    * `<path/to/your executable>.exe -batchmode -nographics -logFile [-map <other scene>] [-gameType <other game type>] [-room <other room name>]`
4. Build your game with your main scene, start as a client peer and connect to the room as usual.