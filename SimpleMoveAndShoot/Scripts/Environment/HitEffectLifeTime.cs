using UnityEngine;

public class HitEffectLifeTime : MonoBehaviour
{
	[SerializeField] private float Delay;

	void Start()
	{
		Destroy(gameObject, Delay);
	}
}
