using System.Collections;
using System.Collections.Generic;
using UdpKit;
using UnityEngine;

[BoltGlobalBehaviour("NetworkedPaint_Menu", "NetworkedPaint_Game")]
public class CharacterPaintChannelManager : Bolt.GlobalEventListener
{
	private const string TextureTransmitChannelName = "TextureTransmitChannel";
	private UdpKit.UdpChannelName _textureTransmitChannel;

	public override void BoltStartBegin()
	{
		// Define Reliable Channel
		BoltNetwork.CreateStreamChannel(TextureTransmitChannelName, UdpChannelMode.Reliable, 1);
	}
}
