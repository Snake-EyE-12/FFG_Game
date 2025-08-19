using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LobbyPlayButtonController : MonoBehaviour
{
    [SerializeField] private GameObject playButton;

    private void OnEnable()
    {
        LobbyDataContainer.OnPlayerReady += CheckCanStart;
        LobbyDataContainer.OnPlayerNotReady += CheckCanStart;
    }
    private void OnDisable()
    {
        LobbyDataContainer.OnPlayerReady -= CheckCanStart;
        LobbyDataContainer.OnPlayerNotReady -= CheckCanStart;
    }

    public void CheckCanStart()
    {
        int players = LobbyDataContainer.PlayerCount;
        playButton.SetActive(players > 1 && LobbyDataContainer.ReadyCount >= players);
    }
}

public static class LobbyDataContainer
{
    private static List<LobbyPlayerUIBinder> lobbyPlayers = new();
    
    public static void Join(LobbyPlayerUIBinder lobbyPlayer)
    {
        lobbyPlayers.Add(lobbyPlayer);
        OnJoin?.Invoke(lobbyPlayer);
    }

    public static void Leave(LobbyPlayerUIBinder lobbyPlayerUI)
    {
        lobbyPlayers.Remove(lobbyPlayerUI);
    }
    
    public static Action OnPlayerReady;
    public static Action OnPlayerNotReady;
    public static Action<LobbyPlayerUIBinder> OnJoin;

    public static void BecomeReady()
    {
        OnPlayerReady?.Invoke();
    }

    public static void BecomeNotReady()
    {
        OnPlayerNotReady?.Invoke();
    }

    public static void Reset()
    {
        lobbyPlayers.Clear();
    }
    
    public static int PlayerCount => lobbyPlayers.Count;
    public static int ReadyCount => lobbyPlayers.Count(x => x.Ready.Value);
}