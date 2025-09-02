using NaughtyAttributes;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class FingerGun : NetworkBehaviour
{
	[Header("Gun Settings")]
	[SerializeField] private LineRenderer aimLine;
	[SerializeField] private LayerMask shootLayerMask;
	[SerializeField] private LayerMask wallMask;
	[SerializeField] private float aimDist = 10f;
	[SerializeField] private float aimAnglePinchSpeed = 5f;
	[SerializeField] private float aimStartAngle = 30f;

	[Header("Reloading")]
	[SerializeField] private float reloadLength;
	[SerializeField, MinMaxSlider(0, 1)] private Vector2 perfectReloadPosRange;
	[SerializeField] private float perfectReloadLength;
	private float perfectReloadStartTime;
	private float reloadTimer;
	private bool isReloading;
	private bool attemptedPerfectReload;
	[SerializeField] private Slider reloadSlider;
	[SerializeField] private Slider perfectReloadSlider;
	[SerializeField] private Image reloadSliderBackground;

	private Transform topOfHead;

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

	private void Start()
	{
		if(topOfHead == null)
		{
			topOfHead = transform.GetChild(0).GetChild(0);
		}
	}

	private void Update()
	{
		bool drawAiming = IsOwner ? aiming : netAiming.Value;
		float angle = IsOwner ? currentAimAngle : netAimAngle.Value;
		Vector3 dir = IsOwner ? aimDir : netAimDir.Value;

		if (drawAiming && !isReloading)
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

		if (isReloading)
		{
			reloadTimer += Time.deltaTime;
			reloadSlider.value = reloadTimer / reloadLength;
			if (reloadTimer >= reloadLength)
			{
				Reload();
			}
		}
	}

	private void UpdateAimAngleAndDir()
	{
		Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
		Plane groundPlane = new Plane(Vector3.up, transform.position + Vector3.up);

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

		float angleRad = Mathf.Atan2(aimDir.z, aimDir.x);
		float angleDeg = angleRad * Mathf.Rad2Deg;
		if (angleDeg < 0) angleDeg += 360;

		PlayerMovement.LocalInstance.UpdateAimAngle(angleDeg);
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
		if (isReloading)
		{
			StopAim();
			return false;
		}
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

	public void StartReload()
	{
		reloadSlider.gameObject.SetActive(true);
		reloadTimer = 0;
		isReloading = true;
		reloadSlider.value = 0;
		reloadSliderBackground.color = Color.white;

		float PRVal = Random.Range(perfectReloadPosRange.x, perfectReloadPosRange.y);

		perfectReloadStartTime = Random.Range(reloadLength * PRVal, reloadLength * PRVal);

		perfectReloadSlider.value = PRVal;

	}

	private void Reload()
	{
		reloadSlider.gameObject.SetActive(false);
		gunLoaded = true;
		isReloading = false;
		attemptedPerfectReload = false;
    }

	private void TryPerfectReload()
	{
		if (attemptedPerfectReload) return;

		if (reloadTimer >= perfectReloadStartTime && reloadTimer <= perfectReloadStartTime + perfectReloadLength)
		{
			Reload();
			return;
		}

        reloadSliderBackground.color = Color.red;

        attemptedPerfectReload = true;
	}

	public void Shoot()
	{
		if (isReloading)
		{
			TryPerfectReload();
			return;
		}

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
