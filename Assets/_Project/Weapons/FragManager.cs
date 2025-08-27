using UnityEngine;

public class FragManager : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private Frag fragPrefab;
    private bool holdingFrag;

    [SerializeField] private float maxThrowDist;
    [SerializeField] private GameObject markerPrefab;
    private GameObject markerInstance;

    [SerializeField] private float fragCooldownLength;
    private float fragCooldownTime;
    [SerializeField] private int maxFrags;

    private int currentFragCount;

    private Vector3 hitPoint;

    private void Awake()
    {
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
        fragCooldownTime -= Time.deltaTime;
        if (holdingFrag)
        {

            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
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
        else
        {
            markerInstance.SetActive(false);
        }
    }

    public bool ThrowFrag()
    {
        if (fragCooldownTime > 0)
            return TryHoldFrag();


        currentFragCount--;

        Frag spawned = Instantiate(fragPrefab, transform.position, transform.rotation);
        spawned.endPos = hitPoint;
        spawned.parent = gameObject;
        fragCooldownTime = fragCooldownLength;
        return TryHoldFrag();
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
}
