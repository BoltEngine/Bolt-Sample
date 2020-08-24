using UnityEngine;

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

	public int Height
	{
		get { return (int) (this.baseTexture.height * Scale); }
	}

	public int Width
	{
		get { return (int) (this.baseTexture.width * Scale); }
	}

	private readonly Texture2D baseTexture;
	private int OriginalHeight { get { return this.baseTexture.height; } }
	private int OriginalWidth { get { return this.baseTexture.width; } }
	private float _scale;

	public Color GetPixel(int x, int y)
	{
		x = (int) ScaleValues(0, Width, 0, OriginalWidth, x);
		y = (int) ScaleValues(0, Height, 0, OriginalHeight, y);

		return this.baseTexture.GetPixel(x, y);
	}

	public TextureManager(Texture2D baseTexture)
	{
		this.baseTexture = baseTexture;
		this.Scale = 1;
	}

	private float ScaleValues(float OldMin, float OldMax, float NewMin, float NewMax, float OldValue)
	{
		float OldRange = (OldMax - OldMin);
		float NewRange = (NewMax - NewMin);
		float NewValue = (((OldValue - OldMin) * NewRange) / OldRange) + NewMin;

		return (NewValue);
	}
}
