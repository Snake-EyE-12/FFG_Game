using System.Collections.Generic;
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
	[HideInInspector] public GameObject parent;
	[HideInInspector] public NetworkObject parentNetObj;

	private HashSet<NetworkObject> hitObjects = new HashSet<NetworkObject>();

	private Vector3 startPos;
	private float elapsedTime;

	private bool localExploded;
	private bool serverExploded;

	private Vector3 targetPos;
	private float interpSpeed = 10f;

	private float serverUpdateTimer;
	private float serverUpdateRate = 0.05f;

	private void Start()
	{
		startPos = transform.position;
		targetPos = startPos;

		if (parent != null)
			parentNetObj = parent.GetComponent<NetworkObject>();
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

		ulong parentId = parentNetObj != null ? parentNetObj.NetworkObjectId : 0;
		if (IsOwner)
			ExplodeServerRpc(transform.position, parentId);

		Destroy(gameObject);
	}

	[ServerRpc(RequireOwnership = false)]
	private void ExplodeServerRpc(Vector3 pos, ulong parentId)
	{
		if (serverExploded) return;
		serverExploded = true;

		Collider[] hits = Physics.OverlapSphere(pos, explosionRadius, hitMask);
		foreach (var hit in hits)
		{
			if (hit.TryGetComponent(out NetworkObject netObj))
			{
				// skip parent
				if (netObj.NetworkObjectId == parentId) continue;

				if (!hitObjects.Contains(netObj))
				{
					hitObjects.Add(netObj);

					if (hit.TryGetComponent(out Health health))
					{
						health.HitPlayer();
					}
					else if (hit.TryGetComponent(out Obstacle obs))
					{
						obs.DestroyServerRpc();
					}
				}
			}
		}

		SpawnExplosionClientRpc(pos);

		if (TryGetComponent<NetworkObject>(out var fragNetObj) && fragNetObj.IsSpawned)
			fragNetObj.Despawn();
	}

	[ClientRpc]
	private void SpawnExplosionClientRpc(Vector3 pos)
	{
		if (explosionPrefab != null)
			Instantiate(explosionPrefab, pos, Quaternion.identity);
	}

	[ClientRpc]
	private void SetServerPositionClientRpc(Vector3 pos)
	{
		if (!IsOwner)
			targetPos = pos;
	}
}
