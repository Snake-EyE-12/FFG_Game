using System;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class TestingSpawning : MonoBehaviour
{
    [ServerRpc]
    public void ActivatePlayersServerRpc()
    {
        foreach (var client in NetworkManager.Singleton.ConnectedClients.Values)
        {
            client.PlayerObject.GetComponent<GameCharacterController>().ActivateDummy();
        }
    }


    private void Update()
    {

        if (Keyboard.current.oKey.wasPressedThisFrame)
        {
            ActivatePlayersServerRpc();
        }
    }
}
