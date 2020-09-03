using System.IO;
using System.Linq;
using Bolt.Utils;
using UdpKit;
using UnityEngine;

namespace Bolt.Samples.NetworkPaintStreamSample.Network
{
	class PlayerTextureMeta : IProtocolToken
	{
		public NetworkId TargetEntity { get { return _entityID; } }

		public Texture2D TargetTexture
		{
			get
			{
				if (_internalTexture == null && _textureData != null)
				{
					_internalTexture = new Texture2D(_width, _height, (TextureFormat)_format, false);
					_internalTexture.LoadRawTextureData(_textureData);
				}

				return _internalTexture;
			}
		}

		private NetworkId _entityID;
		private int _size;
		private int _width;
		private int _height;
		private int _format;

		private byte[] _textureData;
		private Texture2D _internalTexture = null;

		public PlayerTextureMeta() { }

		public PlayerTextureMeta(NetworkId entityID, Texture2D textureData)
		{
			_entityID = entityID;
			_width = textureData.width;
			_height = textureData.height;
			_format = (int)textureData.format;

			_textureData = textureData.GetRawTextureData();
			_size = _textureData.Length;
		}

		public void Read(UdpPacket packet)
		{
			_entityID = packet.ReadNetworkId();
			_width = packet.ReadInt();
			_height = packet.ReadInt();
			_format = packet.ReadInt();
		}

		public void Write(UdpPacket packet)
		{
			packet.WriteNetworkId(_entityID);
			packet.WriteInt(_width);
			packet.WriteInt(_height);
			packet.WriteInt(_format);
		}

		public byte[] Serialize()
		{
			using (MemoryStream m = new MemoryStream())
			{
				using (BinaryWriter writer = new BinaryWriter(m))
				{
					writer.Write(_size);
					writer.Write(_textureData);

					var metaSerialized = this.ToByteArray();
					var metaSize = metaSerialized.Length;

					writer.Write(metaSize);
					writer.Write(metaSerialized);
				}

				return m.ToArray();
			}
		}

		public static PlayerTextureMeta Deserialize(byte[] data)
		{
			PlayerTextureMeta result = null;
			using (MemoryStream m = new MemoryStream(data))
			{
				using (BinaryReader reader = new BinaryReader(m))
				{
					var size = reader.ReadInt32();
					var textureData = reader.ReadBytes(size);

					var metaSize = reader.ReadInt32();
					var metaSerialized = reader.ReadBytes(metaSize);

					result = metaSerialized.ToToken() as PlayerTextureMeta;

					if (result != null)
					{
						result._size = size;
						result._textureData = textureData;
					}
				}
			}

			return result;
		}
	}

	[BoltGlobalBehaviour("NetworkedPaint_Menu", "NetworkedPaint_Game")]
	public class CharacterPaintChannelManager : Bolt.GlobalEventListener
	{
		private const string TextureTransmitChannelName = "TextureTransmitChannel";
		private UdpKit.UdpChannelName _textureTransmitChannel;

		private void Awake()
		{
			BrokerSystem.OnTextureChanged += SendTexture;
		}

		private void OnDestroy()
		{
			BrokerSystem.OnTextureChanged -= SendTexture;
		}

		public override void BoltStartBegin()
		{
			// Define Reliable Channel
			_textureTransmitChannel = BoltNetwork.CreateStreamChannel(TextureTransmitChannelName, UdpChannelMode.Reliable, 1);

			BoltNetwork.RegisterTokenClass<PlayerTextureMeta>();
		}

		public override void Connected(BoltConnection connection)
		{
			connection.SetStreamBandwidth(1024 * 100);
		}

		public void SendTexture(NetworkId entityId, Texture2D texture, BoltConnection origin)
		{
			if (BoltNetwork.Connections.Any() == false) { return; }

			var textureInfo = new PlayerTextureMeta(entityId, texture);

			SendTexture(textureInfo.Serialize(), origin);
		}

		private void SendTexture(byte[] playerTexture, BoltConnection origin)
		{
			foreach (var remoteConn in BoltNetwork.Connections)
			{
				// skip original sender
				if (origin != null && remoteConn.Equals(origin))
				{
					continue;
				}

				remoteConn.StreamBytes(_textureTransmitChannel, playerTexture);
			}
		}

		public override void StreamDataReceived(BoltConnection connection, UdpStreamData data)
		{
			var playerTexture = PlayerTextureMeta.Deserialize(data.Data);

			// Retransmit
			if (playerTexture != null)
			{
				BrokerSystem.PublishTexture(playerTexture.TargetEntity, playerTexture.TargetTexture, connection);
			}
		}
	}
}
