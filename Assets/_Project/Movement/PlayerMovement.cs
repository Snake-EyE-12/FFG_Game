using NUnit.Framework.Api;
using System;
using System.Collections;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : NetworkBehaviour
{
	public static PlayerMovement LocalInstance;

	public enum MoveState
    {
        IDLE,
        AIMING,
        RUNNING,
        CROUCHING,
		CROUCH_WALKING,
        SLIDING
    }
    public MoveState currentMoveState;
    public MoveState lastMoveState;

	public Transform colliderT;
    [SerializeField,Tooltip("How fast player moves normally")] private float runSpeed;
    [SerializeField,Tooltip("How fast player moves while crouched")] private float crouchWalkSpeed;
    [SerializeField,Tooltip("How fast player moves while sliding")] private float slideSpeed;
    [SerializeField,Tooltip("How long the player slides for")] private float slideDuration;
    
    public Vector2 moveDir;
    private Rigidbody rb;

	private float slideTimer;
	private Vector2 slideDir;

	public Health health;
	public PlayerAnimationController animationController;

	private bool fullHeight = true;

	private void Awake()
    {
		rb = GetComponent<Rigidbody>();
		GameStarter.OnGameStart += OnGameStart;
    }

	private void OnGameStart()
	{
		ChangeState(MoveState.IDLE);
		RequestSpawnFromServer();
	}

    private void FixedUpdate()
    {
		if (!GameManager.Instance.inGame || health.dead) return;
		
		FixedUpdateState();
        
    }

	private void Update()
	{
		if (!GameManager.Instance.inGame || health.dead) return;

		UpdateState();
		
	}

	[HideInInspector] public System.Action<Vector3> RequestedSpawnCallback;

	public void OnReceivedSpawnPoint(Vector3 spawnPos)
	{
		transform.GetChild(0).GetComponent<Health>().OnReceivedSpawn(spawnPos);
		RequestedSpawnCallback?.Invoke(spawnPos);
	}

	public void RequestSpawnFromServer()
	{
		if (!IsOwner) return;
		Spawning.Instance.RequestSpawnPointServerRpc();
	}

	public override void OnNetworkSpawn()
	{
		if (!IsOwner)
		{
			enabled = false;
		}
		else
		{
			LocalInstance = this;
		}
	}

	#region INPUT ACTIONS
	public void Move(InputAction.CallbackContext context)
    {
		if (!IsOwner || health.dead) return;
		
		moveDir = context.ReadValue<Vector2>();
		OnNewMoveInput();
		
	}

	public void Crouch(InputAction.CallbackContext context)
	{
		if (!IsOwner || health.dead) return;
		
		if(context.performed)
		{
			switch(currentMoveState)
			{
				case MoveState.IDLE:
					StandingToCrouch();
					break;
				case MoveState.RUNNING:
					RunningToSlide(); 
					break;
				case MoveState.CROUCHING:
					CrouchToStand();
					break;
				case MoveState.CROUCH_WALKING:
					CrouchWalkToRun();
					break;
				default:
					// other cases can be ignored
					break;
			}
		}
		
	}

	#endregion

	public void OnNewMoveInput()
	{
		if (moveDir != Vector2.zero)
		{ // Trying to move
			switch (currentMoveState)
			{
				case MoveState.IDLE:
					// start running
					ChangeState(MoveState.RUNNING);
					break;

				case MoveState.CROUCHING:
					// start crouch walking
					ChangeState(MoveState.CROUCH_WALKING);
					break;
				case MoveState.SLIDING:
					if(moveDir != slideDir)
					{
						SlideToCrouchWalk();
					}
					break;
				default:
					// all other cases are ignored with current functionality, add more cases if need be
					break;
			}

			if (currentMoveState is not (MoveState.AIMING or MoveState.SLIDING))
			{
				UpdateFlip();
			}
		}
		else
		{
			switch (currentMoveState)
			{
				case MoveState.RUNNING:
					// stop running
					ChangeState(MoveState.IDLE);
					break;

				case MoveState.CROUCH_WALKING:
					// stop crouch walking
					ChangeState(MoveState.CROUCHING);
					break;
				default:
					// all other cases are ignored with current functionality, add more cases if need be
					break;
			}
		}
	}

	private void UpdateFlip()
	{
		if (moveDir.x > 0)
		{
			animationController.FlipX(false);
		}
		else if (moveDir.x < 0)
		{
			animationController.FlipX(true);
		}
	}

	public void MoveRb(float speed, Vector2 dir)
	{
		rb.linearVelocity = (Vector3.right * dir.x * speed) + (Vector3.forward * dir.y * speed);
	}


	public void Run()
	{
		MoveRb(runSpeed, moveDir);
	}

	public void CrouchWalk()
	{
		MoveRb(crouchWalkSpeed, moveDir);
	}

	public void Slide()
	{
		MoveRb(slideSpeed, slideDir);
	}

	public void StandingToCrouch()
	{
		ChangeState(MoveState.CROUCHING);
		LowerHeight();
	}

	public void RunningToSlide()
	{
		ChangeState(MoveState.SLIDING);
		LowerHeight();
	}

	public void SlideToCrouch()
	{
		ChangeState(MoveState.CROUCHING);
	}

	public void SlideToCrouchWalk()
	{
		ChangeState(MoveState.CROUCH_WALKING);
	}

	public void CrouchWalkToRun()
	{
		ChangeState(MoveState.RUNNING);
		RaiseHeight();
	}

	public void CrouchToStand()
	{
		ChangeState(MoveState.IDLE);
		RaiseHeight();
	}

	public void CrouchToAim()
	{
		RaiseHeight();
		ChangeState(MoveState.AIMING);
	}

	private void LowerHeight()
	{
		if (!fullHeight) return;
		colliderT.localScale = new Vector3(colliderT.localScale.x, 
											colliderT.localScale.y * 0.5f, 
											colliderT.localScale.z);
		colliderT.localPosition = new Vector3(colliderT.localPosition.x, 
											colliderT.localPosition.y - 0.5f, 
											colliderT.localPosition.z);
		fullHeight = false;
	}

	private void RaiseHeight()
	{
		if (fullHeight) return;
		colliderT.localScale = new Vector3(colliderT.localScale.x,
									colliderT.localScale.y * 2f,
									colliderT.localScale.z);
		colliderT.localPosition = new Vector3(colliderT.localPosition.x,
											colliderT.localPosition.y + 0.5f,
											colliderT.localPosition.z);
		fullHeight = true;
	}

	private void EndSlide()
	{
		if(moveDir != Vector2.zero)
		{
			ChangeState(MoveState.CROUCH_WALKING);
		}
		else
		{
			ChangeState(MoveState.CROUCHING);
		}
	}

	public bool TryAim()
	{
		switch(currentMoveState)
		{
			case MoveState.AIMING:
				return false;
			case MoveState.CROUCHING:
				CrouchToAim();
				return true;
			case MoveState.CROUCH_WALKING:
				CrouchToAim();
				return true;
			case MoveState.SLIDING:
				return false;
			default:
				ChangeState(MoveState.AIMING);
				return true;

		}
	}

    public void ChangeState(MoveState newState)
    {
        if(currentMoveState == newState) return;

        OnExitState();

        lastMoveState = currentMoveState;
        currentMoveState = newState;

        OnEnterState();
    }

    private void OnExitState()
    {
		switch (currentMoveState)
		{
			case MoveState.IDLE:
				break;
			case MoveState.AIMING:
				break;
			case MoveState.RUNNING:
				break;
			case MoveState.CROUCHING:
				break;
			case MoveState.CROUCH_WALKING:
				break;
			case MoveState.SLIDING:
				break;
		}
	}

	private void OnEnterState()
    {
		switch (currentMoveState)
		{
			case MoveState.IDLE:
				animationController.ChangeState(PlayerAnimationController.AnimationState.STANDING_FRONT);
				break;
			case MoveState.AIMING:
				animationController.ChangeState(PlayerAnimationController.AnimationState.STANDING_AIMING);
				break;
			case MoveState.RUNNING:
				UpdateFlip();
				animationController.ChangeState(PlayerAnimationController.AnimationState.RUNNING_FRONT);
				break;
			case MoveState.CROUCHING:
				break;
			case MoveState.CROUCH_WALKING:
				UpdateFlip();
				break;
			case MoveState.SLIDING:
				slideTimer = Time.time;
				slideDir = moveDir;
				break;
		}
	}

    private void UpdateState()
    {
		switch (currentMoveState)
		{
			case MoveState.IDLE:
				break;
			case MoveState.AIMING:
				break;
			case MoveState.RUNNING:
				break;
			case MoveState.CROUCHING:
				break;
			case MoveState.CROUCH_WALKING:
				break;
			case MoveState.SLIDING:
				float timeSliding = Time.time - slideTimer;
				bool stoppedMidSlide = timeSliding > 0.5f && rb.linearVelocity.magnitude < 1;
				if(timeSliding > slideDuration || stoppedMidSlide)
				{
					EndSlide();
				}
				break;
		}
	}

	private void FixedUpdateState()
	{
		switch (currentMoveState)
		{
			case MoveState.IDLE:
				break;
			case MoveState.AIMING:
				break;
			case MoveState.RUNNING:
				{
					Run();
				}
				break;
			case MoveState.CROUCHING:
				break;
			case MoveState.CROUCH_WALKING:
				{
					CrouchWalk();
				}
				break;
			case MoveState.SLIDING:
				{
					Slide();
				}
				break;
		}
	}

	public void OnDeath()
	{
		ChangeState(MoveState.IDLE);
		RaiseHeight();
	}
}