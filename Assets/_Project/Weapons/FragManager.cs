using UnityEngine;

public class FragManager : MonoBehaviour
{
    private bool holdingFrag;

    [SerializeField] private int maxFrags;
    private int currentFragCount;

    private void Awake()
    {
        RefillGrenades();
    }

    public void RefillGrenades()
    {
        currentFragCount = maxFrags;
    }

    public void ThrowFrag()
    {

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
