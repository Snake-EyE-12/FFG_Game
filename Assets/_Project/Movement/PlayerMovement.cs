using System;
using TMPro;
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
        Debug.Log("MOVING: " + moveDir);
        if (SceneManager.GetActiveScene().name.Equals("Game"))
        {
            rb.linearVelocity = (referencePlane.right * moveDir.x * moveSpeed) + (referencePlane.forward * moveDir.y * moveSpeed);
            text.text = transform.position.ToString();
        }
    }

    public void Move(InputAction.CallbackContext context)
    {
        moveDir = context.ReadValue<Vector2>();
    }
}