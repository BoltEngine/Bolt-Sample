using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Bolt.Samples.NetworkPaintStreamSample.UI
{
	[RequireComponent(typeof(Image))]
	public class CurrentColorController : MonoBehaviour
	{
		private Image _image;

		void Awake()
		{
			BrokerSystem.OnColorChanged += OnColorChanged;
		}

		void Start()
		{
			_image = GetComponent<Image>();
		}

		void OnDisable()
		{
			BrokerSystem.OnColorChanged -= OnColorChanged;
		}

		private void OnColorChanged(Color newColor)
		{
			_image.color = newColor;
		}
	}
}
