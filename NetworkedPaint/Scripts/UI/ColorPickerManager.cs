using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Bolt.Samples.NetworkPaintStreamSample.UI
{
	public class ColorPickerManager : MonoBehaviour, IPointerClickHandler
	{
		public void OnPointerClick(PointerEventData eventData)
		{
			Vector2 localCursor;
			var image = GetComponent<RawImage>();

			if (RectTransformUtility.ScreenPointToLocalPointInRectangle(image.rectTransform, eventData.pressPosition,
				eventData.pressEventCamera, out localCursor))
			{
				Texture2D tex = image.texture as Texture2D;
				Rect r = image.rectTransform.rect;

				//Using the size of the texture and the local cursor, clamp the X,Y coords between 0 and width - height of texture
				float coordX = Mathf.Clamp(0, (((localCursor.x - r.x) * tex.width) / r.width), tex.width);
				float coordY = Mathf.Clamp(0, (((localCursor.y - r.y) * tex.height) / r.height), tex.height);

				var color = tex.GetPixel((int)coordX, (int)coordY);

				if (color.a > 0.5)
				{
					BrokerSystem.PublishColorPicker(color);
				}
			}
		}
	}
}
