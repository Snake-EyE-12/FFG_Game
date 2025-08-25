using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameCharacterDummy : MonoBehaviour
{
    private GameCharacterController characterController;
    public void Initialize(GameCharacterController controller)
    {
        // Save controller when needing to send back data ( Collisions )
        characterController = controller;
    }
    
    // ALL INPUT EVENT METHODS CALLED BY CONTROLLER

    [SerializeField] private PlayerMovement movement;
    private Vector2 moveDir;
    public void Move(InputAction.CallbackContext context)
    {
        if(characterController.IsOwner) characterController.ReceiveInput(context.ReadValue<Vector2>());
    }
    public PlayerMovement GetMovement() => movement;

    //
}