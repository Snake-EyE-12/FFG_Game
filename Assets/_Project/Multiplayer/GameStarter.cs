using System;
using Unity.Netcode;
using UnityEngine;

public class GameStarter : MonoBehaviour
{
    private void Start()
    {
        OnGameStart?.Invoke();
    }

    public static Action OnGameStart;
    public static Action OnGameEnd; // needs to be invoked somewhere
}