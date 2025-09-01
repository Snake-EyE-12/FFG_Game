using Unity.Netcode;
using UnityEngine;

public class FragManager : NetworkBehaviour
{
	[Header("Frag Settings")]
	[SerializeField] private Frag fragPrefab;
	[SerializeField] private float maxThrowDist = 10f;
	[SerializeField] private GameObject markerPrefab;
	private GameObject markerInstance;

	[SerializeField] private float fragCooldownLength = 2f;
	private float fragCooldownTime;

	[SerializeField] private int maxFrags = 3;
	private int currentFragCount;

	private bool holdingFrag;
	private Vector3 hitPoint;

	public override void OnNetworkSpawn()
	{
		if (!IsOwner)
			enabled = false;
	}

	private void Start()
	{
		GameStarter.OnGameStart += Init;
	}

	public void Init()
	{
		if (!IsOwner) return;

		markerInstance = Instantiate(markerPrefab);
		markerInstance.SetActive(false);

		RefillGrenades();
	}

	public void RefillGrenades()
	{
		currentFragCount = maxFrags;
	}

	private void Update()
	{
		if (!IsOwner || !GameManager.Instance.inGame) return;

		fragCooldownTime -= Time.deltaTime;

		if (holdingFrag)
			UpdateMarker();
		else if (markerInstance != null)
			markerInstance.SetActive(false);
	}

	private void UpdateMarker()
	{
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		Plane groundPlane = new Plane(Vector3.up, Vector3.zero);

		if (groundPlane.Raycast(ray, out float enter))
		{
			hitPoint = ray.GetPoint(enter);
			Vector3 dir = hitPoint - transform.position;

			if (dir.magnitude > maxThrowDist)
				hitPoint = transform.position + dir.normalized * maxThrowDist;

			markerInstance.transform.position = hitPoint + Vector3.up * 0.01f;
			markerInstance.transform.rotation = Quaternion.Euler(90, 0, 0);
		}

		markerInstance.SetActive(true);
	}

	public bool ThrowFrag()
	{
		if (fragCooldownTime > 0f || currentFragCount <= 0)
			return false;

		currentFragCount--;
		fragCooldownTime = fragCooldownLength;

		// Spawn predicted frag locally for instant movement
		SpawnPredictedFrag(hitPoint);

		// Tell server to spawn authoritative frag for everyone
		ThrowFragServerRpc(hitPoint);

		return true;
	}

	private void SpawnPredictedFrag(Vector3 target)
	{
		Frag predicted = Instantiate(fragPrefab, transform.position, transform.rotation);
		predicted.endPos = target;
		predicted.parent = gameObject;
	}

	[ServerRpc]
	private void ThrowFragServerRpc(Vector3 target, ServerRpcParams rpcParams = default)
	{
		Frag spawned = Instantiate(fragPrefab, transform.position, transform.rotation);
		spawned.endPos = target;
		spawned.parent = gameObject;
		spawned.GetComponent<NetworkObject>().Spawn(true);
	}

	public bool TryHoldFrag()
	{
		holdingFrag = currentFragCount > 0;
		return holdingFrag;
	}

	public void StopHoldFrag()
	{
		holdingFrag = false;
	}

	public int GetCurrentFragCount() => currentFragCount;
}
