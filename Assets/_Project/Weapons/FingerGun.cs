using UnityEngine;

public class FingerGun : MonoBehaviour
{
    [SerializeField] private Camera cam;

    private bool aiming;
    private bool gunLoaded;

    private float aimAngle;
    private Vector3 aimDir;

    private Vector3 leftAimPos;
    private Vector3 rightAimPos;

    [SerializeField] private LineRenderer aimLine;

    [SerializeField] private LayerMask shootLayerMask;
    [SerializeField] private LayerMask wallMask;

    [SerializeField] private float aimAnglePinchSpeed;
    private float aimTimer;
    [SerializeField] private float aimDist;
    [SerializeField] private float aimStartAngle;
    private float currentAimAngle;

    private void Awake()
    {
        gunLoaded = true;
    }

    private void Update()
    {

        if (aiming)
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            Plane groundPlane = new Plane(Vector3.up, Vector3.one);

            if (groundPlane.Raycast(ray, out float mouse))
            {
                Vector3 hitPoint = ray.GetPoint(mouse);

                Vector3 direction = hitPoint - transform.position;
                direction.y = 0; // remove vertical tilt if for purely horizontal aim

                aimDir = direction;

                if (direction != Vector3.zero)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(direction);
                    transform.rotation = targetRotation;
                    //transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, aimRotationSpeed * Time.deltaTime);
                }
            }

            Vector3 leftDir = Quaternion.AngleAxis(-currentAimAngle / 2, Vector3.up) * transform.forward;
            Vector3 rightDir = Quaternion.AngleAxis(currentAimAngle / 2, Vector3.up) * transform.forward;

            Ray leftRay = new Ray(new Vector3(transform.position.x, 1, transform.position.z), leftDir);
            Ray rightRay = new Ray(new Vector3(transform.position.x, 1, transform.position.z), rightDir);

            if (Physics.Raycast(leftRay, out RaycastHit leftHit, aimDist, wallMask))
            {
                leftAimPos = leftHit.point;
            }
            else
            {
                leftAimPos = leftDir * aimDist;
            }
            if (Physics.Raycast(rightRay, out RaycastHit rightHit, aimDist, wallMask))
            {
                rightAimPos = rightHit.point;
            }
            else
            {
                rightAimPos = rightDir * aimDist;
            }

            aimLine.SetPosition(0, leftAimPos);
            aimLine.SetPosition(2, rightAimPos);

            if (currentAimAngle > .5f)
                currentAimAngle = Mathf.Lerp(currentAimAngle, .1f, aimAnglePinchSpeed * Time.deltaTime);
        }
        else
        {
            aimLine.SetPosition(0, new Vector3(transform.position.x, 1, transform.position.z));
            aimLine.SetPosition(2, new Vector3(transform.position.x, 1, transform.position.z));
        }
        aimLine.SetPosition(1, transform.position);
    }

    public bool Aim()
    {
        currentAimAngle = aimStartAngle;
        aimTimer = aimAnglePinchSpeed;
        aiming = gunLoaded;

        return aiming;
    }

    public void StopAim()
    {
        aimLine.SetPosition(0, new Vector3(transform.position.x, 1, transform.position.z));
        aimLine.SetPosition(2, new Vector3(transform.position.x, 1, transform.position.z));
        aiming = false;
    }

    public void StartReload()
    {
        Reload();
    }

    private void Reload()
    {
        gunLoaded = true;
    }

    public void Shoot()
    {
        if (aiming && gunLoaded)
        {
            Vector3 ShootDir = Quaternion.AngleAxis(Random.Range(-currentAimAngle / 2, currentAimAngle / 2), Vector3.up) * transform.forward;
            Ray shootRay = new Ray(transform.position, ShootDir);

            if (Physics.Raycast(shootRay, out RaycastHit hit, aimDist, shootLayerMask))
            {
                if (hit.collider.gameObject.TryGetComponent(out Health health))
                {
                    health.HitPlayer();
                }
            }

            gunLoaded = false;
            aimTimer = aimAnglePinchSpeed;
            currentAimAngle = aimStartAngle;
            StartReload();

        }
    }
}
