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
    private void Start()
    {
        GameStarter.OnGameStart += Reset; //Problem
    }
	private void Reset()
	{
		Spawning.Instance.RequestSpawnPointServerRpc();
	}
	public void OnReceivedSpawnPoint(Vector3 spawnPos)
	{
		Debug.Log("Recieved");
		transform.position = spawnPos;
	}

	private void Awake()
    {
		rb = GetComponent<Rigidbody>();

    }

    private void FixedUpdate()
    {
		if (!GameManager.Instance.inGame) return;
		
		FixedUpdateState();
        
    }

	private void Update()
	{
		if (!GameManager.Instance.inGame) return;

		UpdateState();
		
	}

	public override void OnNetworkSpawn()
	{
		if (!IsOwner)
		{
			// Prevent remote players from reading input on this instance
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
		if (!IsOwner) return;
		
		moveDir = context.ReadValue<Vector2>();
		OnNewMoveInput();
		
	}

	public void Crouch(InputAction.CallbackContext context)
	{
		if (!IsOwner) return;
		
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
		}
		else
		{ // Letting go of movement keys
			switch (currentMoveState)
			{
				case MoveState.RUNNING:
					// start running
					ChangeState(MoveState.IDLE);
					break;

				case MoveState.CROUCH_WALKING:
					// start crouch walking
					ChangeState(MoveState.CROUCHING);
					break;
				default:
					// all other cases are ignored with current functionality, add more cases if need be
					break;
			}
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
		colliderT.localScale = new Vector3(colliderT.localScale.x, 
											colliderT.localScale.y * 0.5f, 
											colliderT.localScale.z);
		colliderT.localPosition = new Vector3(colliderT.localPosition.x, 
											colliderT.localPosition.y - 0.5f, 
											colliderT.localPosition.z);
	}

	private void RaiseHeight()
	{
		colliderT.localScale = new Vector3(colliderT.localScale.x,
									colliderT.localScale.y * 2f,
									colliderT.localScale.z);
		colliderT.localPosition = new Vector3(colliderT.localPosition.x,
											colliderT.localPosition.y + 0.5f,
											colliderT.localPosition.z);
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

        OnEnterstate();
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

	private void OnEnterstate()
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
}