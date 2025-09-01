using Unity.Netcode;
using UnityEngine;

public class Frag : NetworkBehaviour
{
	[Header("Frag Settings")]
	[SerializeField] private float duration = 1.5f;
	[SerializeField] private float explosionRadius = 3f;
	[SerializeField] private LayerMask hitMask;
	[SerializeField] private ParticleSystem explosionPrefab;

	[HideInInspector] public Vector3 endPos;
	public GameObject parent;

	private Vector3 startPos;
	private float elapsedTime = 0f;

	// Explosion flags
	private bool localExploded = false;
	private bool serverExploded = false;

	// Interpolation for non-owners
	private Vector3 targetPos;
	private float interpSpeed = 10f;

	private float serverUpdateTimer = 0f;
	private float serverUpdateRate = 0.05f;

	private void Start()
	{
		startPos = transform.position;

		if (!IsOwner)
			targetPos = startPos;
	}

	private void Update()
	{
		if (localExploded && (!IsOwner || (IsOwner && !IsServer))) return;

		if (IsOwner)
		{
			elapsedTime += Time.deltaTime;
			transform.position = Vector3.Slerp(startPos, endPos, elapsedTime / duration);

			if (elapsedTime >= duration && !localExploded)
				ExplodeLocal();

			if (IsServer)
			{
				serverUpdateTimer += Time.deltaTime;
				if (serverUpdateTimer >= serverUpdateRate)
				{
					serverUpdateTimer = 0f;
					SetServerPositionClientRpc(transform.position);
				}
			}
		}
		else
		{
			transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * interpSpeed);
		}
	}

	private void ExplodeLocal()
	{
		if (localExploded) return;
		localExploded = true;

		SpawnExplosionClientRpc(transform.position);

		if (IsOwner)
			ExplodeServerRpc(transform.position, parent.GetComponent<NetworkObject>().NetworkObjectId);

		Destroy(gameObject);
	}

	[ServerRpc]
	private void ExplodeServerRpc(Vector3 pos, ulong parentId)
	{
		if (serverExploded) return;
		serverExploded = true;

		var hits = Physics.OverlapSphere(pos, explosionRadius, hitMask);
		foreach (var hit in hits)
		{
			if (hit.TryGetComponent(out Health health) &&
				hit.gameObject.GetComponent<NetworkObject>().NetworkObjectId != parentId)
			{
				health.HitPlayer();
			}
			else if (hit.TryGetComponent(out Obstacle obs))
			{
				obs.DestroyServerRpc();
			}
		}

		SpawnExplosionClientRpc(pos);

		if (TryGetComponent<NetworkObject>(out var netObj) && netObj.IsSpawned)
			netObj.Despawn();
	}

	[ClientRpc]
	private void SpawnExplosionClientRpc(Vector3 pos)
	{
		Instantiate(explosionPrefab, pos, Quaternion.identity);
	}

	[ClientRpc]
	public void SetServerPositionClientRpc(Vector3 pos)
	{
		if (!IsOwner)
			targetPos = pos;
	}
}
