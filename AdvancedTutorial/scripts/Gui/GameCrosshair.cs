using UnityEngine;
using System.Collections;

namespace Bolt.AdvancedTutorial {

	public class GameCrosshair : Bolt.GlobalEventListener {
	  BoltEntity me;
	  IPlayerState meState;

	  float previousSpread = float.MaxValue;

	  public int PixelWidth = 4;
	  public int PixelHeight = 11;

	  public int MinSpred = 5;
	  public int MaxSpred = 75;
	  public float Spread = 0f;

	  public Transform Left;
	  public Transform Right;
	  public Transform Top;
	  public Transform Bottom;

	  public override void ControlOfEntityGained(BoltEntity arg) {
	    me = arg;
	    meState = arg.GetState<IPlayerState>();
	  }

	  public override void ControlOfEntityLost(BoltEntity arg) {
	    me = null;
	    meState = null;
	  }

	  void Update() {
	    if (me && meState != null && meState.Aiming) {
	      Left.gameObject.SetActive(true);
	      Right.gameObject.SetActive(true);
	      Top.gameObject.SetActive(true);
	      Bottom.gameObject.SetActive(true);

	      Spread = Mathf.Clamp01(Spread);
	      Spread -= 0.01f;

	      if (Spread != previousSpread) {
	        int pixelSpread = Mathf.Clamp(Mathf.RoundToInt(Spread * MaxSpred), MinSpred, MaxSpred);

	        Left.position = ToScreenPosition(new Vector3(-PixelHeight - pixelSpread, (PixelWidth / 2), 1));
	        Right.position = ToScreenPosition(new Vector3(pixelSpread, (PixelWidth / 2), 1));
	        Top.position = ToScreenPosition(new Vector3(-(PixelWidth / 2), PixelHeight + pixelSpread, 1));
	        Bottom.position = ToScreenPosition(new Vector3(-(PixelWidth / 2), -pixelSpread, 1));

	        previousSpread = Spread;
	      }
	    }
	    else {
	      Left.gameObject.SetActive(false);
	      Right.gameObject.SetActive(false);
	      Top.gameObject.SetActive(false);
	      Bottom.gameObject.SetActive(false);
	    }
	  }

	  static Vector3 ToScreenPosition(Vector3 pos) {
	    pos.x = Mathf.RoundToInt(pos.x);
	    pos.y = Mathf.RoundToInt(pos.y);

	    switch (Application.platform) {
	      case RuntimePlatform.WindowsEditor:
	      case RuntimePlatform.WindowsPlayer:
//	      case RuntimePlatform.WindowsWebPlayer:
//	      case RuntimePlatform.XBOX360:
	        pos.x += 0.5f;
	        pos.y += 0.5f;
	        break;
	    }

	    return pos;
	  }
	}

}