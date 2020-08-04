using UnityEngine;
using System.Collections;
using UdpKit.Platform;

namespace Bolt.Samples.GettingStarted
{
	public class CubeBehavior : Bolt.EntityEventListener<ICubeState>
	{
		public GameObject[] WeaponObjects;

		float resetColorTime;
		Renderer gameObjectrenderer;

		public override void Attached()
		{
			gameObjectrenderer = GetComponent<Renderer>();

			state.SetTransforms(state.CubeTransform, transform);

			if (entity.IsOwner)
			{
				state.CubeColor = new Color(Random.value, Random.value, Random.value);

				// NEW: On the owner, we want to setup the weapons, we randomize the selected weapon by the index on the Weapon list and the Ammo between 50 to 100.
				for (int i = 0; i < state.WeaponArray.Length; ++i)
				{
					state.WeaponArray[i].WeaponId = i;
					state.WeaponArray[i].WeaponAmmo = Random.Range(50, 100);
				}

				//NEW: by default we don't have any weapon up, so set index to -1
				state.WeaponActiveIndex = -1;
			}

			state.AddCallback("CubeColor", ColorChanged);

			// NEW: we also setup a callback for whenever the index changes
			state.AddCallback("WeaponActiveIndex", WeaponActiveIndexChanged);
		}

		void OnGUI()
		{
			if (entity.IsOwner)
			{
				GUI.color = state.CubeColor;
				GUILayout.Label("@@@");
				GUI.color = Color.white;
			}
		}

		void Update()
		{
			if (resetColorTime < Time.time)
			{
				gameObjectrenderer.material.color = state.CubeColor;
			}
		}

		public override void SimulateOwner()
		{
			var speed = 4f;
			var movement = Vector3.zero;

			if (Input.GetKey(KeyCode.W)) { movement.z += 1; }
			if (Input.GetKey(KeyCode.S)) { movement.z -= 1; }
			if (Input.GetKey(KeyCode.A)) { movement.x -= 1; }
			if (Input.GetKey(KeyCode.D)) { movement.x += 1; }

			// NEW: Input polling for weapon selection
			if (Input.GetKeyDown(KeyCode.Alpha1)) state.WeaponActiveIndex = 0;
			if (Input.GetKeyDown(KeyCode.Alpha2)) state.WeaponActiveIndex = 1;
			if (Input.GetKeyDown(KeyCode.Alpha3)) state.WeaponActiveIndex = 2;
			if (Input.GetKeyDown(KeyCode.Alpha0)) state.WeaponActiveIndex = -1;

			if (movement != Vector3.zero)
			{
				transform.position = transform.position + (movement.normalized * speed * BoltNetwork.FrameDeltaTime);
			}

			if (Input.GetKeyDown(KeyCode.F))
			{
				var flash = FlashColorEvent.Create(entity);
				flash.FlashColor = Color.red;
				flash.Send();
			}
		}

		void ColorChanged()
		{
			GetComponent<Renderer>().material.color = state.CubeColor;
		}

		void WeaponActiveIndexChanged()
		{
			for (int i = 0; i < WeaponObjects.Length; ++i)
			{
				WeaponObjects[i].SetActive(false);
			}

			if (state.WeaponActiveIndex >= 0)
			{
				int objectId = state.WeaponArray[state.WeaponActiveIndex].WeaponId;
				WeaponObjects[objectId].SetActive(true);
			}
		}

		public override void OnEvent(FlashColorEvent evnt)
		{
			resetColorTime = Time.time + 0.25f;
			gameObjectrenderer.material.color = evnt.FlashColor;
		}
	}
}