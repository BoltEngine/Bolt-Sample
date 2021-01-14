using System.Collections;
using System.Collections.Generic;
using Photon.Bolt;
using UnityEngine;
using UnityEngine.UI;

public class BoltFrameController : MonoBehaviour
{
	[SerializeField] private Text Label;
	[SerializeField] private Text ValuePing;
	[SerializeField] private Text ValueBitsInt;
	[SerializeField] private Text ValueBitsOut;

	public void SetLabel(BoltConnection connection)
	{
		this.Label.text = string.Format("[{0}] {1}", connection.ConnectionType, connection.RemoteEndPoint.ToString());
	}

	public void SetValue(float ping, float bitsIn, float bitsOut)
	{
		this.ValuePing.text = ping.ToString();
		this.ValueBitsInt.text = bitsIn.ToString();
		this.ValueBitsOut.text = bitsOut.ToString();
	}
}
