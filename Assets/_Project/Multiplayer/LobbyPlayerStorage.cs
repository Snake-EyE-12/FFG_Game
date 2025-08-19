using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;

public class LobbyPlayerStorage : MonoBehaviour
{
    [SerializeField] private LobbyPlayerUI lobbyPlayerUIPrefab;
    public LobbyPlayerUI CreateBinding()
    {
        return Instantiate(lobbyPlayerUIPrefab, transform);
    }
}