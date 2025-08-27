using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    public Transform referencePlane;
    [SerializeField] private float moveSpeed;
    
    
    private Rigidbody rb;
    private Vector2 lastMoveDir;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        //referencePlane = Spawning.spawnPlane;
    }

    public void Move(InputAction.CallbackContext context)
    {
        lastMoveDir = context.ReadValue<Vector2>();
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = (referencePlane.right * (lastMoveDir.x * moveSpeed)) + (referencePlane.forward * (lastMoveDir.y * moveSpeed));
    }

}
