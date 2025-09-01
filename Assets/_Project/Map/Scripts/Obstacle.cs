using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class Obstacle : NetworkBehaviour
{
	[SerializeField] private float respawnTime;
	[SerializeField] private MeshRenderer mr;
	[SerializeField] private Collider coll;

	private NetworkVariable<bool> isDestroyed = new NetworkVariable<bool>(
		false,
		NetworkVariableReadPermission.Everyone,
		NetworkVariableWritePermission.Server);

	private void Awake()
	{
		isDestroyed.OnValueChanged += OnStateChanged;
	}

	private void OnDestroy()
	{
		isDestroyed.OnValueChanged -= OnStateChanged;
	}

	[ContextMenu("Destroy")]
	[ServerRpc(RequireOwnership = false)]
	public void DestroyServerRpc()
	{
		Debug.Log("DestroyServerRpc");
		if (isDestroyed.Value) return;

		isDestroyed.Value = true; // updates clients
		Debug.Log("is destroyed="+isDestroyed.Value);
		StartCoroutine(RespawnTimer());
	}

	private void Respawn()
	{
		isDestroyed.Value = false; // updates clients
	}

	private IEnumerator RespawnTimer()
	{
		yield return new WaitForSeconds(respawnTime);
		Respawn();
	}

	private void OnStateChanged(bool oldValue, bool newValue)
	{
		if (newValue) // destroyed
		{
			mr.enabled = false;
			coll.enabled = false;
			transform.localScale = Vector3.zero;
		}
		else // respawned
		{
			mr.enabled = true;
			coll.enabled = true;
			StartCoroutine(ScaleOverTime(0.5f, Vector3.zero, Vector3.one));
		}
	}

	private IEnumerator ScaleOverTime(float time, Vector3 startScale, Vector3 targetScale)
	{
		float t = 0;
		transform.localScale = startScale;
		while (t < time)
		{
			t += Time.deltaTime;
			transform.localScale = Vector3.Lerp(startScale, targetScale, t / time);
			yield return null;
		}
		transform.localScale = targetScale;
	}
}
