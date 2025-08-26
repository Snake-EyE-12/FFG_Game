using System;
using Unity.Netcode;
using UnityEngine;

public class GameStarter : MonoBehaviour
{
    public void StartGame()
    {
        if (NetworkManager.Singleton.IsServer)
        {
            OnGameStart?.Invoke();
        }
    }

    public static Action OnGameStart;
}