using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
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



    public Transform referencePlane;
    [SerializeField,Tooltip("How fast player moves normally")] private float runSpeed;
    [SerializeField,Tooltip("How fast player moves while crouched")] private float crouchWalkSpeed;
    [SerializeField,Tooltip("How fast player moves while sliding")] private float slideSpeed;
    [SerializeField,Tooltip("How long the player slides for")] private float slideDuration;
    
    private Vector2 moveDir;
    private Rigidbody rb;

    [SerializeField] TMP_Text text;

    private void Start()
    {
        GameStarter.OnGameStart += Reset; //Problem
    }

    private void Reset()
    {
        text = FindFirstObjectByType<TMP_Text>();
        text.text = "Reset1";
        transform.position = Spawning.GetSpawnPoint().position;
        text.text = "Reset2";
        referencePlane = Spawning.spawnPlane;
        text.text = "Reset3";
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if (SceneManager.GetActiveScene().name.Equals("Game")) // Not good
        {
			FixedUpdateState();
        }
    }

	private void Update()
	{
		if (SceneManager.GetActiveScene().name.Equals("Game")) // Not good
		{

			UpdateState();
		}
	}

	public void Move(InputAction.CallbackContext context)
    {
        moveDir = context.ReadValue<Vector2>();
		OnNewMoveInput();
	}

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

	public void MoveRb(float speed)
	{
		rb.linearVelocity = (referencePlane.right * moveDir.x * speed) + (referencePlane.forward * moveDir.y * speed);
	}

	public void Run()
	{
		MoveRb(runSpeed);
	}

	public void CrouchWalk()
	{
		MoveRb(crouchWalkSpeed);
	}

	public void StandingToCrouch()
	{
		LowerHeight();
	}

	public void RunningToSlide()
	{
		LowerHeight();
	}

	public void SlideToCrouch()
	{

	}

	public void CrouchWalkToRun()
	{
		RaiseHeight();
	}

	public void CrouchToStand()
	{
		RaiseHeight();
	}

	private void LowerHeight()
	{

	}

	private void RaiseHeight()
	{

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
				break;
		}
	}

}