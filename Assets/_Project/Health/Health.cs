using UnityEngine;

public class Health : MonoBehaviour
{
    private bool hasFingerGun;

    public void HitPlayer()
    {
        if (hasFingerGun)
        {
            //REMOVE FINGER GUN
        }
        else
        {
            Death();
        }
    }

    private void Death()
    {
        Destroy(gameObject);
    }
}
