using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class FingerGun : NetworkBehaviour
{
	[Header("Gun Settings")]
	[SerializeField] private LineRenderer aimLine;
	[SerializeField] private LayerMask shootLayerMask;
	[SerializeField] private LayerMask wallMask;
	[SerializeField] private float aimDist = 10f;
	[SerializeField] private float aimAnglePinchSpeed = 5f;
	[SerializeField] private float aimStartAngle = 30f;

	private bool aiming = false;
	private bool gunLoaded = true;

	private float currentAimAngle;
	private Vector3 aimDir;

	private Vector3 leftAimPos;
	private Vector3 rightAimPos;

	private NetworkVariable<bool> netAiming = new NetworkVariable<bool>(
		false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
	private NetworkVariable<float> netAimAngle = new NetworkVariable<float>(
		0f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
	private NetworkVariable<Vector3> netAimDir = new NetworkVariable<Vector3>(
	Vector3.zero, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

	private void Update()
	{
		bool drawAiming = IsOwner ? aiming : netAiming.Value;
		float angle = IsOwner ? currentAimAngle : netAimAngle.Value;
		Vector3 dir = IsOwner ? aimDir : netAimDir.Value;

		if (drawAiming)
		{
			if (IsOwner)
				UpdateAimAngleAndDir();

			DrawVLine(dir, angle);
		}
		else
		{
			Vector3 origin = transform.position + Vector3.up * 1.5f;
			aimLine.SetPosition(0, origin);
			aimLine.SetPosition(1, origin);
			aimLine.SetPosition(2, origin);
		}
	}

	private void UpdateAimAngleAndDir()
	{
		Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
		Plane groundPlane = new Plane(Vector3.up, transform.position);

		if (groundPlane.Raycast(ray, out float distance))
		{
			Vector3 hitPoint = ray.GetPoint(distance);
			Vector3 direction = hitPoint - transform.position;
			direction.y = 0f;

			if (direction != Vector3.zero)
			{
				aimDir = direction;
				transform.rotation = Quaternion.LookRotation(direction);
			}
		}

		if (currentAimAngle > 0.5f)
			currentAimAngle = Mathf.Lerp(currentAimAngle, 0.01f, aimAnglePinchSpeed * Time.deltaTime);

		netAiming.Value = aiming;
		netAimAngle.Value = currentAimAngle;
		netAimDir.Value = aimDir;
	}

	private void DrawVLine(Vector3 baseDir, float angle)
	{
		Vector3 origin = transform.position + Vector3.up * 1.5f;

		Vector3 leftDir = Quaternion.AngleAxis(-angle / 2f, Vector3.up) * baseDir;
		Vector3 rightDir = Quaternion.AngleAxis(angle / 2f, Vector3.up) * baseDir;

		// Cast rays to walls, fallback to full distance
		leftAimPos = Physics.Raycast(origin, leftDir, out RaycastHit leftHit, aimDist, wallMask)
			? leftHit.point
			: origin + leftDir * aimDist;

		rightAimPos = Physics.Raycast(origin, rightDir, out RaycastHit rightHit, aimDist, wallMask)
			? rightHit.point
			: origin + rightDir * aimDist;

		aimLine.SetPosition(0, leftAimPos);
		aimLine.SetPosition(1, origin);
		aimLine.SetPosition(2, rightAimPos);
	}

	public bool Aim()
	{
		currentAimAngle = aimStartAngle;
		aiming = gunLoaded;

		if (IsOwner)
		{
			netAiming.Value = aiming;
			netAimAngle.Value = currentAimAngle;
		}

		return aiming;
	}

	public void StopAim()
	{
		aiming = false;
		if (IsOwner)
			netAiming.Value = false;

		Vector3 origin = transform.position + Vector3.up * 1.5f;
		aimLine.SetPosition(0, origin);
		aimLine.SetPosition(1, origin);
		aimLine.SetPosition(2, origin);
	}

	public void StartReload() => Reload();

	private void Reload() => gunLoaded = true;

	public void Shoot()
	{
		if (!aiming || !gunLoaded) return;

		Vector3 origin = transform.position + Vector3.up * 1.5f;

		Vector3 shootDir = Quaternion.AngleAxis(
			Random.Range(-currentAimAngle / 2f, currentAimAngle / 2f),
			Vector3.up) * transform.forward;

		Ray shootRay = new Ray(origin, shootDir);
		if (Physics.Raycast(shootRay, out RaycastHit hit, aimDist, shootLayerMask))
		{
			if (hit.collider.TryGetComponent(out Health health))
			{
				health.HitPlayer();
			}
		}

		gunLoaded = false;
		currentAimAngle = aimStartAngle;
		StartReload();

		if (IsOwner)
			netAimAngle.Value = currentAimAngle;
	}
}
