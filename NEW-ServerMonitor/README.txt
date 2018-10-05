Instructions:

1 - Edit samples/NEW-Server Monitor/ServerCallbacks.cs and make sure the annotation corresponds to your host's gameplay scene (by default it comes marked to Advanced tutorial's Level1 scene) - this will load the Resources/ServerMonitor prefab when that scene is loaded on a Bolt host;
2 - Edit samples/NEW-Server Monitor/ClientMonitor.cs and put IP address of your Bolt host (leave "127.0.0.1" to test with Unity editor as bolt host);
3 - Build a standalone app with this single scene: samples/Server Monitor/MonitorScene (this will be the monitor application);

This server Monitor uses Google Protobuf to serialise data from the server to the monitor application. This gives a lot of extensibility and flexibility opportunities, such as:

- building monitor application in different languages (for example a C/C++ shell-friendly executable);
- extending the monitored data (edit ServerStats.cs, then look into ServerMonitor.cs' CollectBoltData() method to periodically gather and insert the new info);
- Take a look at ClientMonitor.cs to modify/augment the monitoring UI;
