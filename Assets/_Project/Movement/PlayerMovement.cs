using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    public Transform referencePlane;
    [SerializeField] private float moveSpeed;
    
    private Vector2 moveDir;
    private Rigidbody rb;

    private void Start()
    {
        //referencePlane = Spawning.spawnPlane;
        Debug.Log("oiuytrtyuio");
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        //rb.linearVelocity = (referencePlane.right * moveDir.x * moveSpeed) + (referencePlane.forward * moveDir.y * moveSpeed);
        //Debug.Log(rb.linearVelocity);
        //Debug.Log(moveDir);
    }

    public void Move(InputAction.CallbackContext context)
    {
        moveDir = context.ReadValue<Vector2>();
    }
}