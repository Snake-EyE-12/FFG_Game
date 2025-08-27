using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    public Transform referencePlane;
    [SerializeField] private float moveSpeed;
    
    private Vector2 moveDir;
    private Rigidbody rb;

    private void Start()
    {
        Debug.Log("oiuytrtyuio");
        GameStarter.OnGameStart += Reset; //Problem
    }

    private void Reset()
    {
        transform.position = Spawning.GetSpawnPoint().position;
        referencePlane = Spawning.spawnPlane;
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        Debug.Log("MOVING: " + moveDir);
        if(SceneManager.GetActiveScene().name.Equals("Game")) rb.linearVelocity = (referencePlane.right * moveDir.x * moveSpeed) + (referencePlane.forward * moveDir.y * moveSpeed);
    }

    public void Move(InputAction.CallbackContext context)
    {
        moveDir = context.ReadValue<Vector2>();
    }
}