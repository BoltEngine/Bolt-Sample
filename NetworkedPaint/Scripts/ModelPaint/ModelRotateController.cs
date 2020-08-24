using UnityEngine;

public class ModelRotateController : MonoBehaviour
{
	public float RotateSpeed = 10;
	public bool Control = false;

	private Quaternion _targetRotation = Quaternion.identity;
	private float AjustSpeed = 0.8f;

	/// <summary>
	/// Start is called on the frame when a script is enabled just before
	/// any of the Update methods is called the first time.
	/// </summary>
	void Start()
	{
		if (Control == false)
		{
			BrokerSystem.OnCharacterRorationChanged += OnCharacterRorationChanged;
		}
	}

	/// <summary>
	/// This function is called when the behaviour becomes disabled or inactive.
	/// </summary>
	void OnDisable()
	{
		if (Control == false)
		{
			BrokerSystem.OnCharacterRorationChanged -= OnCharacterRorationChanged;
		}
	}

	void Update()
	{
		if (Control && Input.GetMouseButton(1))
		{
			var x = (Input.GetAxis("Mouse Y") * RotateSpeed * Time.deltaTime);
			var y = -(Input.GetAxis("Mouse X") * RotateSpeed * Time.deltaTime);

			transform.Rotate(x, y, 0, Space.World);

			BrokerSystem.PublishCharacterRoration(transform.rotation);
		}

		if (Control == false && _targetRotation != Quaternion.identity)
		{
			transform.rotation = Quaternion.Lerp(transform.rotation, _targetRotation, Time.deltaTime * AjustSpeed);
		}
	}

	private void OnCharacterRorationChanged(Quaternion newRotation)
	{
		_targetRotation = newRotation;
	}
}
