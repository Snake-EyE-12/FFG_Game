using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponActionManager : MonoBehaviour
{
    public float aimSpeed;

    private bool aiming;
    private bool holdingFrag;

    [SerializeField] private FingerGun fingerGun;
    [SerializeField] private FragManager fragManager;

    public void Aim(InputAction.CallbackContext context)
    {
        if (context.performed && !holdingFrag)
        {
            aiming = fingerGun.Aim();
            Debug.Log("Aiming");
        }
        if (context.canceled && aiming)
        {
            aiming = false;
            fingerGun.StopAim();
        }
    }

    public void Shoot(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (holdingFrag)
            {
                fragManager.ThrowFrag();
            }
            else if (aiming)
            {
                fingerGun.Shoot();
                Debug.Log("Shot");
            }
        }
    }

    public void HoldFrag(InputAction.CallbackContext context)
    {
        if (context.performed && !aiming)
        {
            holdingFrag = fragManager.TryHoldFrag();
        }
        if (context.canceled && holdingFrag)
        {
            holdingFrag = false;
            fragManager.StopHoldFrag();
        }
    }

    public void Reload(InputAction.CallbackContext context)
    {
        throw new NotImplementedException();
    }
}