using System;
using UnityEngine;

namespace Bolt.Samples.NetworkPaintStreamSample
{
	public static class BrokerSystem
	{
		public static event Action<Color> OnColorChanged;
		public static event Action<Quaternion> OnCharacterRotationChanged;
		public static event Action<GameObject> OnMainCharacterChanged;
		public static event Action<GameObject> OnAddOtherCharacter;
		public static event Action<GameObject> OnRemoveOtherCharacter;
		public static event Action<NetworkId, Texture2D, BoltConnection> OnTextureChanged;

		public static void PublishTexture(NetworkId entityId, Texture2D texture, BoltConnection origin = null)
		{
			if (OnTextureChanged != null)
			{
				OnTextureChanged.Invoke(entityId, texture, origin);
			}
		}

		public static void PublishColorPicker(Color newColor)
		{
			FireEvent(OnColorChanged, newColor);
		}

		public static void PublishCharacterRotation(Quaternion newRotation)
		{
			FireEvent(OnCharacterRotationChanged, newRotation);
		}

		public static void PublishNewMainCharacter(GameObject go)
		{
			FireEvent(OnMainCharacterChanged, go);
		}

		public static void PublishAddOtherCharacter(GameObject go)
		{
			FireEvent(OnAddOtherCharacter, go);
		}

		public static void PublishRemoveOtherCharacter(GameObject go)
		{
			FireEvent(OnRemoveOtherCharacter, go);
		}

		/// <summary>
		/// Trigger an Internal Event
		/// </summary>
		/// <typeparam name="T">Type used by the Action</typeparam>
		/// <param name="action">Event Action to be Invoked</param>
		/// <param name="target">Event Object to be sent by the Action</param>
		private static void FireEvent<T>(Action<T> action, T target)
		{
			if (action != null)
			{
				action.Invoke(target);
			}
		}
	}
}
