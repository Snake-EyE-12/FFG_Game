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
	}
	private AnimationState currentState;

	private float animationTimer;
	private int secondaryIndex; // second index of current two-frame animation


	public void ChangeState(AnimationState newState)
	{
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
			case AnimationState.RUNNING_BACK:
			case AnimationState.RUNNING_FRONT:
			case AnimationState.RUNNING_SIDE:
			case AnimationState.CROUCH_WALKING_FRONT:
			case AnimationState.CROUCH_WALKING_SIDE:
			case AnimationState.CROUCH_WALKING_BACK:
				if(animationTimer > 0)
				{
					animationTimer -= Time.deltaTime;
				}
				else
				{
					animationTimer = animationTime;

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
}