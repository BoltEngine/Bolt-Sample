using UnityEngine;

namespace Bolt.Samples.NetworkPaintStreamSample.Core
{
	public class ModelPaintManager : Bolt.EntityBehaviour<ICharacterPaintState>
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
		private readonly int _updateRate = 5; // every 3 secs
		private float _timeCounter = 0;

		void Awake()
		{
			// Subscribe to color changes
			BrokerSystem.OnColorChanged += OnColorChanged;
		}

		void Start()
		{
			_cam = Camera.main;

			// Brush
			_brushTextureManager = new TextureManager(BrushTexture);
			// Debug.LogFormat("Loading Brush with size: {0}:{1}", _brushTextureManager.Width, _brushTextureManager.Height);

			SetupTextureReferences();

			// Set starting color
			Invoke("SetupCurrentColor", 1f);
		}

		public override void Attached()
		{
			if (entity.IsOwner == false)
			{
				BrokerSystem.OnTextureChanged += OnTextureChangedFromRemote;
			}
		}

		private void OnDestroy()
		{
			BrokerSystem.OnColorChanged -= OnColorChanged;
			BrokerSystem.OnTextureChanged -= OnTextureChangedFromRemote;
		}

		void Update()
		{
			if (entity.IsOwner == false || _cam == null) { return; }

			// Change Brush Size
			// ChangeBrushSize();

			RaycastHit hit;
			if (Input.GetMouseButton(0) && // If mouse is pressed
			    Physics.Raycast(_cam.ScreenPointToRay(Input.mousePosition), out hit) && // If we hit something
			    hit.transform.Equals(transform)) // if something is this transform
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
								_textureChanged = true;
							}
						}
					}
				}

				if (_textureChanged)
				{
					_texture.Apply();
				}
			}
		}

		private void ReplaceTexture(Texture2D newTexture)
		{
			lock (_textureLock)
			{
				_texture.LoadRawTextureData(newTexture.GetRawTextureData());
				_texture.Apply();
			}
		}

		private void PublishTexture()
		{
			lock (_textureLock)
			{
				Graphics.CopyTexture(_texture, _transitTexture);

				// Publish current texture
				BrokerSystem.PublishTexture(entity.NetworkId, _transitTexture);

				_textureChanged = false;
			}
		}

		private void OnTextureChangedFromRemote(NetworkId entityId, Texture2D texture, BoltConnection origin)
		{
			if (entity.NetworkId.Equals((entityId)))
			{
				ReplaceTexture(texture);
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

		private void ChangeBrushSize()
		{
			if (_brushTextureManager != null)
			{
				if (Input.mouseScrollDelta.y > 0)
				{
					_brushTextureManager.Scale += 0.1f;
					// Debug.LogFormat("Set Scale to {0}", _brushTextureManager.Scale);
				}
				else if (Input.mouseScrollDelta.y < 0)
				{
					_brushTextureManager.Scale -= 0.1f;
					// Debug.LogFormat("Set Scale to {0}", _brushTextureManager.Scale);
				}
			}
		}
	}
}
