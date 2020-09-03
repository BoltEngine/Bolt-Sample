using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Bolt.Samples.NetworkPaintStreamSample.Core
{
	public class ModelPaintManager : MonoBehaviour
	{
		public Renderer Renderer;
		public Texture2D BrushTexture;

		private Camera _cam;
		private Texture2D _texture;
		private Texture2D _originalTexture;
		private Texture2D _transitTexture;
		private TextureManager _brushTextureManager;

		private Color _currentColor;

		private readonly object _textureLock = new object();
		private bool _textureChanged = false;
		private int _updateRate = 60 * 3 * 1000; // every 3 secs
		private float _timeCounter = 0;

		void Awake()
		{
			// Subscribe to color changes
			BrokerSystem.OnColorChanged += OnColorChanged;
		}

		void Start()
		{
			Debug.Log("Starting Manager");
			_cam = Camera.main;

			// Brush
			_brushTextureManager = new TextureManager(BrushTexture);
			Debug.LogFormat("Loading Brush with size: {0}:{1}", _brushTextureManager.Width, _brushTextureManager.Height);

			SetupTextureReferences();

			// Set starting color
			Invoke("SetupCurrentColor", 1f);
		}

		void OnDisable()
		{
			BrokerSystem.OnColorChanged -= OnColorChanged;
		}

		void Update()
		{
			if (_brushTextureManager != null)
			{
				if (Input.mouseScrollDelta.y > 0)
				{
					_brushTextureManager.Scale += 0.1f;
					Debug.LogFormat("Set Scale to {0}", _brushTextureManager.Scale);
				}
				else if (Input.mouseScrollDelta.y < 0)
				{
					_brushTextureManager.Scale -= 0.1f;
					Debug.LogFormat("Set Scale to {0}", _brushTextureManager.Scale);
				}
			}

			if (_cam == null || !Input.GetMouseButton(0))
				return;

			RaycastHit hit;
			if (Physics.Raycast(_cam.ScreenPointToRay(Input.mousePosition), out hit) == false)
				return;

			if (hit.transform.Equals(transform))
			{
				Paint(hit);
			}

			_timeCounter += Time.deltaTime;
			if (_textureChanged && _timeCounter > _updateRate)
			{
				PublishTexture();
				_timeCounter = 0;
			}
		}

		private void SetupTextureReferences()
		{
			var baseTexture = Renderer.material.mainTexture as Texture2D;

			if (!ReferenceEquals(baseTexture, null))
			{
				_texture = new Texture2D(baseTexture.width, baseTexture.height, baseTexture.format, false);
				_originalTexture = new Texture2D(baseTexture.width, baseTexture.height, baseTexture.format, false);
				_transitTexture = new Texture2D(baseTexture.width, baseTexture.height, baseTexture.format, false);

				Graphics.CopyTexture(baseTexture, _texture);
				Graphics.CopyTexture(baseTexture, _originalTexture);
			}

			Renderer.material.mainTexture = _texture;
			Debug.LogFormat("Loading Base Texture with size: {0}:{1}", _texture.width, _texture.height);
		}

		private void Paint(RaycastHit hit)
		{
			lock (_textureLock)
			{
				Vector2 pixelUV = hit.textureCoord;
				pixelUV.x *= _texture.width;
				pixelUV.y *= _texture.height;

				for (int i = 0; i < _brushTextureManager.Width; i++)
				{
					for (int j = 0; j < _brushTextureManager.Height; j++)
					{
						if (_brushTextureManager.GetPixel(i, j).a > 0.5)
						{
							var x = (int)pixelUV.x - (_brushTextureManager.Width / 2) + i;
							var y = (int)pixelUV.y - (_brushTextureManager.Height / 2) + j;

							if (x >= 0 && y >= 0 && x < _texture.width && y < _texture.height)
							{
								_texture.SetPixel(x, y, _currentColor);
							}
						}
					}
				}

				_texture.Apply();
				_textureChanged = true;
			}
		}

		private void PublishTexture()
		{
			lock (_textureLock)
			{
				Graphics.CopyTexture(_texture, _transitTexture);
				BrokerSystem.PublishTexture(_transitTexture);

				_textureChanged = false;
			}
		}

		private void OnColorChanged(Color newColor)
		{
			_currentColor = newColor;
		}

		private void SetupCurrentColor()
		{
			BrokerSystem.PublishColorPicker(Color.white);
		}
	}
}
