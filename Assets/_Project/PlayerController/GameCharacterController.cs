using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameCharacterController : NetworkBehaviour
{
    [SerializeField] private GameCharacterDummy dummy;
    

    public void OnMove(InputAction.CallbackContext context)
    {
        if (IsOwner && dummy.enabled) dummy.Move(context.ReadValue<Vector2>());
    }

    public void ActivateDummy()
    {
        dummy.enabled = true;
        dummy.transform.position = Spawning.GetSpawnPoint().position;
        dummy.Reset();
    }
    

}

