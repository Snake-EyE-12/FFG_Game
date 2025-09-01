using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponActionManager : NetworkBehaviour
{
	private bool aiming;
	private bool holdingFrag;

	[SerializeField] private FingerGun fingerGun;
	[SerializeField] private FragManager fragManager;
	[SerializeField] private PlayerMovement playerMovement;

	public override void OnNetworkSpawn()
	{
		if (!IsOwner)
		{
			enabled = false;
		}
	}

	public void Aim(InputAction.CallbackContext context)
	{
		if (!IsOwner || playerMovement.health.dead) return;
		if (context.performed && !holdingFrag)
		{
			if(playerMovement.TryAim())
			{
				aiming = fingerGun.Aim();
			}
		}
		if (context.canceled && aiming)
		{
			if(playerMovement.moveDir == Vector2.zero)
			{
				playerMovement.ChangeState(PlayerMovement.MoveState.IDLE);
			}
			else
			{
				playerMovement.ChangeState(PlayerMovement.MoveState.RUNNING);
			}
			aiming = false;
			fingerGun.StopAim();
		}
	}

	public void Shoot(InputAction.CallbackContext context)
	{
		if (!IsOwner || playerMovement.health.dead) return;
		if (context.performed)
		{
			if (holdingFrag)
			{
				bool threw = fragManager.ThrowFrag();

				holdingFrag = fragManager.TryHoldFrag();

				if (threw)
				{
					playerMovement.ChangeState(PlayerMovement.MoveState.IDLE);
				}
			}
			//else if (aiming)
			//{
				fingerGun.Shoot();
			//}
		}
	}

	public void HoldFrag(InputAction.CallbackContext context)
	{
		if (!IsOwner || playerMovement.health.dead) return;
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
