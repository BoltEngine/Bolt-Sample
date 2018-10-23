﻿using UnityEngine;

namespace Bolt.Samples.Photon.Lobby
{
    public class BomberPlayerController : Bolt.EntityBehaviour<IBomberState>
    {
        public TextMesh playerNameText;

        public override void Attached()
        {
            BoltLog.Info("Attached BomberPlayer");

            state.SetTransforms(state.Transform, transform);

            if (entity.isOwner)
            {
                state.Color = Color.white;
            }

            state.AddCallback("Color", () =>
            {
                GetComponent<MeshRenderer>().material.color = state.Color;
            });

            state.AddCallback("Name", () =>
            {
                playerNameText.text = state.Name;
            });
        }

        public override void Initialized()
        {
            BoltLog.Info("Initialized BomberPlayer");
        }

        public override void ControlGained()
        {
            BoltLog.Info("ControlGained BomberPlayerController");
        }

        public override void SimulateOwner()
        {
            var speed = 4f;
            var movement = Vector3.zero;

            if (Input.GetKey(KeyCode.W)) { movement.z += 1; }
            if (Input.GetKey(KeyCode.S)) { movement.z -= 1; }
            if (Input.GetKey(KeyCode.A)) { movement.x -= 1; }
            if (Input.GetKey(KeyCode.D)) { movement.x += 1; }

            if (Input.GetKey(KeyCode.F)) { movement.x += 1; }

            if (movement != Vector3.zero)
            {
                transform.position = transform.position + (movement.normalized * speed * BoltNetwork.frameDeltaTime);
            }
        }

        private void Setup(string playerName, Color playerColor)
        {
            BoltLog.Info("Setup BomberPlayer");

            if (entity.isOwner)
            {
                state.Color = playerColor;
                state.Name = playerName;
            }
        }

        public static void Spawn()
        {
            var pos = new Vector3(Random.Range(-16, 16), 0.6f, Random.Range(-16, 16));
            BoltEntity playerEntity = BoltNetwork.Instantiate(BoltPrefabs.BomberPlayer, pos, Quaternion.identity);
            playerEntity.TakeControl();

            BomberPlayerController playerController = playerEntity.GetComponent<BomberPlayerController>();

            Photon.Lobby.LobbyPhotonPlayer lobbyPlayer = Photon.Lobby.LobbyPhotonPlayer.localPlayer;

            if (lobbyPlayer)
            {
                playerController.Setup(lobbyPlayer.playerName, lobbyPlayer.playerColor);
            }
            else
            {
                playerController.Setup("Player #" + Random.Range(1, 100), Random.ColorHSV());
            }
        }
    }
}