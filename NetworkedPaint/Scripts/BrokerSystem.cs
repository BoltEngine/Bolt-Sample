using System;
using UnityEngine;

public static class BrokerSystem
{
	public static event Action<Color> OnColorPicker;
	public static event Action<Quaternion> OnCharacterRorationChanged;

	public static void PublishColorPicker(Color newColor)
	{
		if (OnColorPicker != null)
		{
			OnColorPicker.Invoke(newColor);
		}
	}

	public static void PublishCharacterRoration(Quaternion newRotation)
	{
		if (OnCharacterRorationChanged != null)
		{
			OnCharacterRorationChanged.Invoke(newRotation);
		}
	}
}
