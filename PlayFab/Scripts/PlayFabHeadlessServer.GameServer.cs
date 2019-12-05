#if ENABLE_PLAYFABSERVER_API

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using PlayFab;
using PlayFab.MultiplayerAgent.Model;
using UnityEngine;

namespace Bolt.Samples.PlayFab
{
	/// <summary>
	/// PlayFab Related Calls
	/// </summary>
	public partial class PlayFabHeadlessServer
	{
		private List<ConnectedPlayer> _connectedPlayers;

		private void PlayFabStart()
		{
			_connectedPlayers = new List<ConnectedPlayer>();

			// Playfab Agent Initialization
			PlayFabMultiplayerAgentAPI.Start();
			PlayFabMultiplayerAgentAPI.IsDebugging = Debugging;
			PlayFabMultiplayerAgentAPI.OnMaintenanceCallback += OnMaintenance;
			PlayFabMultiplayerAgentAPI.OnShutDownCallback += OnShutdown;
			PlayFabMultiplayerAgentAPI.OnServerActiveCallback += OnServerActive;
			PlayFabMultiplayerAgentAPI.OnAgentErrorCallback += OnAgentError;

			ConfigurePlayFabLogs();

			// Notify Playfab Server is ready to accept connections
			StartCoroutine(ReadyForPlayers());
		}

		private IEnumerator ReadyForPlayers()
		{
			yield return new WaitForSeconds(.5f);
			PlayFabMultiplayerAgentAPI.ReadyForPlayers();
		}

		// Events

		private void OnAgentError(string error)
		{
			BoltLog.Warn(error);
		}

		private void OnMaintenance(DateTime? NextScheduledMaintenanceUtc)
		{
			BoltLog.Warn(MessageMaintenance);
			OnShutdown();
		}

		private void OnPlayerRemoved(string playfabId)
		{
			ConnectedPlayer player = _connectedPlayers.Find(x => x.PlayerId.Equals(playfabId, StringComparison.OrdinalIgnoreCase));
			_connectedPlayers.Remove(player);
			PlayFabMultiplayerAgentAPI.UpdateConnectedPlayers(_connectedPlayers);
		}

		private void OnPlayerAdded(string playfabId)
		{
			_connectedPlayers.Add(new ConnectedPlayer(playfabId));
			PlayFabMultiplayerAgentAPI.UpdateConnectedPlayers(_connectedPlayers);
		}

		// Utils

		// Configure Logger to output file to Playfab Logs folder
		private void ConfigurePlayFabLogs()
		{
			var logFolderPath = PlayFabMultiplayerAgentAPI.GetConfigSettings() [PlayFabMultiplayerAgentAPI.LogFolderKey];
			BoltLog.Add(new PlayfabLogger(logFolderPath));
		}

		private bool BuildBindingInfo(out BindingInfo bindingInfo)
		{
			var connectionInfo = PlayFabMultiplayerAgentAPI.GetGameServerConnectionInfo();

			IPAddress address;
			if (IPAddress.TryParse(connectionInfo.PublicIpV4Adress, out address))
			{
				try
				{
					var portInfo = connectionInfo.GamePortsConfiguration.First(port => port.Name.Equals(BindingConfigKey));
					var externalIP = new IPEndPoint(address, portInfo.ClientConnectionPort);

					bindingInfo = new BindingInfo
					{
						externalInfo = externalIP,
							internalServerPort = portInfo.ServerListeningPort
					};

					return true;
				}
				catch (InvalidOperationException ex)
				{
					BoltLog.Exception(ex);
				}
			}

			bindingInfo = null;
			return false;
		}
	}
}

#endif