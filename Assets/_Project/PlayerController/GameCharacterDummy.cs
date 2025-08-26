using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameCharacterDummy : MonoBehaviour
{
    
    [SerializeField] private SimpleMovement movement;

    public void Move(Vector2 dir)
    {
        movement.Move(dir);
    }

    public void Reset()
    {
        movement.Zero();
    }
}