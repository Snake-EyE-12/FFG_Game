using UnityEngine;

public class FragManager : MonoBehaviour
{
    [SerializeField] private Camera cam;
    private bool holdingFrag;

    [SerializeField] private int maxFrags;
    [SerializeField] private float maxThrowDist;
    private int currentFragCount;

    private void Awake()
    {
        RefillGrenades();
    }

    public void RefillGrenades()
    {
        currentFragCount = maxFrags;
    }

    private void Update()
    {
        if (holdingFrag)
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            Plane groundPlane = new Plane(Vector3.up, Vector3.one);
        }
    }

    public void ThrowFrag()
    {
        if (holdingFrag)
        {
            currentFragCount--;

        }
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
