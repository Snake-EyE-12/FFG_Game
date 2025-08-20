using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class GamePlayerBrain : PlayerBrain<PlayerSpawner, GamePlayer, GamePlayerBrain>
{
    [SerializeField] private float speed = 5;
    protected override void OnInitialized()
    {
        
    }


    private void Update()
    {
        if (!IsOwner) return;

        if (Keyboard.current.wKey.isPressed)
        {
            binding.transform.Translate(Vector3.forward * (Time.deltaTime * speed));
        }
        if (Keyboard.current.sKey.isPressed)
        {
            binding.transform.Translate(Vector3.back * (Time.deltaTime * speed));
        }
        if (Keyboard.current.aKey.isPressed)
        {
            binding.transform.Translate(Vector3.left * (Time.deltaTime * speed));
        }
        if (Keyboard.current.dKey.isPressed)
        {
            binding.transform.Translate(Vector3.right * (Time.deltaTime * speed));
        }
    }
}