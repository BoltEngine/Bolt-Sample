﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class CurrentColorController : MonoBehaviour
{
	private Image _image;

	void Awake()
	{
		BrokerSystem.OnColorPicker += OnColorChanged;
	}

	void Start()
	{
		_image = GetComponent<Image>();
	}

	void OnDisable()
	{
		BrokerSystem.OnColorPicker -= OnColorChanged;
	}

	private void OnColorChanged(Color newColor)
	{
		_image.color = newColor;
	}
}
