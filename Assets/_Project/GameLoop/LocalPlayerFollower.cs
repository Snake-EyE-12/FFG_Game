using Unity.VisualScripting;
using UnityEngine;

public class LocalPlayerFollower : MonoBehaviour
{
	private void Start()
	{
		if(PlayerMovement.LocalInstance != null)
		{
			transform.parent = PlayerMovement.LocalInstance.transform;
		}
	}
}
