using UnityEngine;

namespace Bolt.Samples.NetworkPaintStreamSample.Core
{
	public class TextureManager
	{
		public float Scale
		{
			get { return _scale; }
			set
			{
				if (value > 0.1)
				{
					_scale = value;
				}
			}
		}

		public int Height { get { return (int)(this._baseTexture.height * Scale); } }
		public int Width { get { return (int)(this._baseTexture.width * Scale); } }
		private int OriginalHeight { get { return this._baseTexture.height; } }
		private int OriginalWidth { get { return this._baseTexture.width; } }
		private float _scale;
		private readonly Texture2D _baseTexture;

		public TextureManager(Texture2D baseTexture)
		{
			this._baseTexture = baseTexture;
			this.Scale = 0.5f;
		}

		public Color GetPixel(int x, int y)
		{
			x = (int)ScaleValues(0, Width, 0, OriginalWidth, x);
			y = (int)ScaleValues(0, Height, 0, OriginalHeight, y);

			return this._baseTexture.GetPixel(x, y);
		}

		private static float ScaleValues(float oldMin, float oldMax, float newMin, float newMax, float oldValue)
		{
			float oldRange = (oldMax - oldMin);
			float newRange = (newMax - newMin);
			float newValue = (((oldValue - oldMin) * newRange) / oldRange) + newMin;

			return (newValue);
		}
	}
}
