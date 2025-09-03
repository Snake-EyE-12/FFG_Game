using Unity.VisualScripting;
using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
	[SerializeField] private NetworkSprite nSprite;
	[SerializeField, Tooltip("Time spent on each frame of an animation")] private float animationTime = 0.3f;

	/**
	 * Indexes for NetworkSprite availableSprites
	 * 
	 * 0 - Standing_Front
	 * 1 - Standing_Back
	 * 2 - Standing_Aiming (up)
	 * 3 - Standing_Aiming (up-right)
	 * 4 - Standing_Aiming (right)
	 * 5 - Standing_Aiming (down-right)
	 * 6 - Standing_Aiming (down)
	 * 7 - Standing_Aiming (down-left)
	 * 8 - Standing_Aiming (left)
	 * 9 - Standing_Aiming (up-left)
	 * 10 - Run_Back (1)
	 * 11 - Run_Back (2)
	 * 12 - Run_Front (1)
	 * 13 - Run_Front (2)
	 * 14 - Run_Side (1)
	 * 15 - Run_Side (2)
	 * 
	 * 16 - Crouching_Front
	 * 17 - Crouching_Back
	 * 18 - Crouching_Side
	 * 19 - Crouch_Walk_Front (1)
	 * 20 - Crouch_Walk_Front (2)
	 * 21 - Crouch_Walk_Side (1)
	 * 22 - Crouch_Walk_Side (2)
	 * 23 - Crouch_Walk_Back (1)
	 * 24 - Crouch_Walk_Back (2)
	 * 25 - Slide_Up
	 * 26 - Slide_Side
	 * 27 - Slide_Down
	 * 28 - AimNade_Up
	 * 29 - AimNade_Side
	 * 30 - AimNade_Down
	 * 31 - ThrowNade_Up
	 * 32 - ThrowNade_Side
	 * 33 - ThrowNade_Down
	 * 34 - Die_Front
	 * 35 - Die_Side
	 * 36 - Die_Back
	 */

	public enum AnimationState
	{
		STANDING_FRONT, 
		STANDING_BACK, 
		STANDING_AIMING, // 8
		RUNNING_BACK, // 2
		RUNNING_FRONT, // 2
		RUNNING_SIDE, // 2
		CROUCHING_FRONT, 
		CROUCHING_BACK, 
		CROUCHING_SIDE, 
		CROUCH_WALKING_FRONT, // 2
		CROUCH_WALKING_SIDE, // 2
		CROUCH_WALKING_BACK, // 2
		SLIDING_UP,
		SLIDING_SIDE,
		SLIDING_DOWN,
		AIMING_NADE_UP,
		AIMING_NADE_SIDE,
		AIMING_NADE_DOWN,
		THROWING_NADE_UP,
		THROWING_NADE_SIDE,
		THROWING_NADE_DOWN,
		DEAD_FRONT,
		DEAD_SIDE,
		DEAD_BACK
	}
	private AnimationState currentState;

	private float animationTimer;
	private int secondaryIndex; // second index of current two-frame animation


	public void ChangeState(AnimationState newState)
	{
		if(currentState == newState) return;
		currentState = newState;
		OnEnterState();
	}

	private void OnEnterState()
	{
		switch (currentState)
		{
			case AnimationState.STANDING_FRONT:
				nSprite.ChangeSpriteServerRpc(0);
				break;
			case AnimationState.STANDING_BACK:
				nSprite.ChangeSpriteServerRpc(1);
				break;
			case AnimationState.STANDING_AIMING:
				FlipX(false);
				// update sprite depending on aim direction. will need process state

				break;
			case AnimationState.RUNNING_BACK:
				nSprite.ChangeSpriteServerRpc(10); // cycle 10-11
				secondaryIndex = 11;
				animationTimer = animationTime;
				break;
			case AnimationState.RUNNING_FRONT:
				nSprite.ChangeSpriteServerRpc(12); // cycle 12-13
				secondaryIndex = 13;
				animationTimer = animationTime;
				break;
			case AnimationState.RUNNING_SIDE:
				nSprite.ChangeSpriteServerRpc(14); // cycle 14-15
				secondaryIndex = 15;
				animationTimer = animationTime;
				break;
			case AnimationState.CROUCHING_FRONT:
				nSprite.ChangeSpriteServerRpc(16);
				break;
			case AnimationState.CROUCHING_BACK:
				nSprite.ChangeSpriteServerRpc(17);
				break;
			case AnimationState.CROUCHING_SIDE:
				nSprite.ChangeSpriteServerRpc(18);
				break;
			case AnimationState.CROUCH_WALKING_FRONT:
				nSprite.ChangeSpriteServerRpc(19); // cycle 19-20
				secondaryIndex = 20;
				animationTimer = animationTime;
				break;
			case AnimationState.CROUCH_WALKING_SIDE:
				nSprite.ChangeSpriteServerRpc(21); // cycle 21-22
				secondaryIndex = 22;
				animationTimer = animationTime;
				break;
			case AnimationState.CROUCH_WALKING_BACK:
				nSprite.ChangeSpriteServerRpc(23); // cycle 23-24
				secondaryIndex = 24;
				animationTimer = animationTime;
				break;
			case AnimationState.SLIDING_UP:
				nSprite.ChangeSpriteServerRpc(25);
				break;
			case AnimationState.SLIDING_SIDE:
				nSprite.ChangeSpriteServerRpc(26);
				break;
			case AnimationState.SLIDING_DOWN:
				nSprite.ChangeSpriteServerRpc(27);
				break;
			case AnimationState.AIMING_NADE_UP:
				nSprite.ChangeSpriteServerRpc(28);
				break;
			case AnimationState.AIMING_NADE_SIDE:
				nSprite.ChangeSpriteServerRpc(29);
				break;
			case AnimationState.AIMING_NADE_DOWN:
				nSprite.ChangeSpriteServerRpc(30);
				break;
			case AnimationState.THROWING_NADE_UP:
				nSprite.ChangeSpriteServerRpc(31);
				break;
			case AnimationState.THROWING_NADE_SIDE:
				nSprite.ChangeSpriteServerRpc(32);
				break;
			case AnimationState.THROWING_NADE_DOWN:
				nSprite.ChangeSpriteServerRpc(33);
				break;
			case AnimationState.DEAD_FRONT:
				nSprite.ChangeSpriteServerRpc(34);
				break;
			case AnimationState.DEAD_SIDE:
				nSprite.ChangeSpriteServerRpc(35);
				break;
			case AnimationState.DEAD_BACK:
				nSprite.ChangeSpriteServerRpc(36);
				break;
		}
	}

	private void Update()
	{
		UpdateState();
	}

	private void UpdateState()
	{
		switch (currentState)
		{
			case AnimationState.CROUCH_WALKING_FRONT:
			case AnimationState.CROUCH_WALKING_SIDE:
			case AnimationState.CROUCH_WALKING_BACK:
				if (animationTimer > 0)
				{
					animationTimer -= Time.deltaTime;
				}
				else
				{
					animationTimer = animationTime;
					AudioManager.Instance.PlaySneakFootstep(transform.position, 1, new Vector2(0.9f, 1f));

					int index = secondaryIndex;
					if (nSprite.SpriteIndex == secondaryIndex) index -= 1;
					nSprite.ChangeSpriteServerRpc(index);
				}
				break;
			case AnimationState.RUNNING_BACK:
			case AnimationState.RUNNING_FRONT:
			case AnimationState.RUNNING_SIDE:
				if(animationTimer > 0)
				{
					animationTimer -= Time.deltaTime;
				}
				else
				{
					animationTimer = animationTime;
					AudioManager.Instance.PlayFootstep(transform.position, 1, new Vector2(0.9f,1f));

					int index = secondaryIndex;
					if(nSprite.SpriteIndex == secondaryIndex) index -= 1;
					nSprite.ChangeSpriteServerRpc(index);
				}
				break;
			default:
				break;
		}
	}

	public void FlipX(bool newFlip)
	{
		nSprite.SetFlipXServerRpc(newFlip);
	}

	private float currentAimAngle;
	public void UpdateAimAngle(float angle)
	{
		currentAimAngle = angle;

		int sectorCount = 8;
		float sectorSize = 360f / sectorCount;
		float offset = 30f;

		float normalized = (angle + 360f + offset) % 360f;

		int sectorIndex = Mathf.FloorToInt(normalized / sectorSize);

		switch (sectorIndex)
		{
			case 0: 
				//Debug.Log("Right");
				nSprite.ChangeSpriteServerRpc(4);
				break;
			case 1:
				//Debug.Log("Up-Right"); 
				nSprite.ChangeSpriteServerRpc(3);
				break;
			case 2:
				//Debug.Log("Up"); 
				nSprite.ChangeSpriteServerRpc(2);
				break;
			case 3: 
				//Debug.Log("Up-Left"); 
				nSprite.ChangeSpriteServerRpc(9);
				break;
			case 4:
				//Debug.Log("Left");
				nSprite.ChangeSpriteServerRpc(8);
				break;
			case 5: 
				//Debug.Log("Down-Left"); 
				nSprite.ChangeSpriteServerRpc(7);
				break;
			case 6:
				//Debug.Log("Down"); 
				nSprite.ChangeSpriteServerRpc(6);
				break;
			case 7:
				//Debug.Log("Down-Right"); 
				nSprite.ChangeSpriteServerRpc(5);
				break;
		}
	}


	public void UpdateNadeAimAngle(float angle)
	{
		currentAimAngle = angle;

		int sectorCount = 6;
		float sectorSize = 360f / sectorCount;
		float offset = 30f;

		float normalized = (angle + 360f + offset) % 360f;

		int sectorIndex = Mathf.FloorToInt(normalized / sectorSize);

		switch (sectorIndex)
		{
			case 0:
				Debug.Log("right");
				ChangeState(AnimationState.AIMING_NADE_SIDE);
				FlipX(true);
				break;
			case 1:
				Debug.Log("up-right");
				ChangeState(AnimationState.AIMING_NADE_UP);
				FlipX(true);
				break;
			case 2:
				Debug.Log("up-left");
				ChangeState(AnimationState.AIMING_NADE_UP);
				FlipX(false);
				break;
			case 3:
				Debug.Log("left");
				ChangeState(AnimationState.AIMING_NADE_SIDE);
				FlipX(false);
				break;
			case 4:
				Debug.Log("down-left");
				ChangeState(AnimationState.AIMING_NADE_DOWN);
				FlipX(true);
				break;
			case 5:
				Debug.Log("down-right");
				ChangeState(AnimationState.AIMING_NADE_DOWN);
				FlipX(false);
				break;
		}
	}

	public void DoDeathFrame(float angle)
	{
		int sectorCount = 6;
		float sectorSize = 360f / sectorCount;
		float offset = 30f;

		float normalized = (angle + 360f + offset) % 360f;

		int sectorIndex = Mathf.FloorToInt(normalized / sectorSize);

		switch (sectorIndex)
		{
			case 0:
				Debug.Log("right");
				ChangeState(AnimationState.DEAD_SIDE);
				FlipX(true);
				break;
			case 1:
				Debug.Log("up-right");
				ChangeState(AnimationState.DEAD_FRONT);
				FlipX(true);
				break;
			case 2:
				Debug.Log("up-left");
				ChangeState(AnimationState.DEAD_FRONT);
				FlipX(false);
				break;
			case 3:
				Debug.Log("left");
				ChangeState(AnimationState.DEAD_SIDE);
				FlipX(false);
				break;
			case 4:
				Debug.Log("down-left");
				ChangeState(AnimationState.DEAD_BACK);
				FlipX(true);
				break;
			case 5:
				Debug.Log("down-right");
				ChangeState(AnimationState.DEAD_BACK);
				FlipX(false);
				break;
		}
	}
}