# Setup Encryption System Sample

This example code shows how you can initialize the Bolt Encryption System.
The initialization occurs by calling "EncryptionManager.Instance.InitializeEncryption" with the desired keys.
It's included a Prefab capable of generating new keys and properly initialize and clear the Encryption System when destroyed.
You should use this Prefab just for testing purposes and never in the production environment as the keys are stored 
on the prefab itself as plain text.

In order to properly use the system, you must build/use a backend service capable of communicating the peers in a secure manner.
This service should be used to exchange the keys among the game clients, ensuring that all connection is encrypted. 