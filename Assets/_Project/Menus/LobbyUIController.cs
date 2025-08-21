using System;
using Unity.Netcode;
using UnityEngine;
public class LobbyUIController : NetworkBehaviour
{
    [SerializeField] private LobbyUI lobbyUIPrefab;
    [SerializeField] private string lobbyParentTag = "LobbyParent";

    public static Action OnAnyReadyStateChanged;
    public bool isReady => ready.Value;

    private LobbyUI instance;
    private NetworkVariable<bool> ready = new NetworkVariable<bool>(false);

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        instance = new LobbyUIBuilder(lobbyUIPrefab)
            .WithParent(GameObject.FindGameObjectWithTag(lobbyParentTag).transform)
            .WithInitialization(this)
            .Build();

        // Subscribe to ready-state changes for THIS controller
        ready.OnValueChanged += (oldVal, newVal) =>
        {
            if (newVal) instance.SetReady();
            else instance.SetNotReady();

            OnAnyReadyStateChanged?.Invoke();
        };

        // Ensure UI starts correct
        if (ready.Value) instance.SetReady();
        else instance.SetNotReady();
    }

    [ServerRpc]
    public void ToggleReadyStateServerRpc()
    {
        ready.Value = !ready.Value;
    }
}

public class LobbyUIBuilder : ILobbyUIBuilder
{
    private LobbyUI _lobbyUIPrefab;
    private Transform _parent;
    private LobbyUIController _controller;

    public LobbyUIBuilder(LobbyUI prefab)
    {
        _lobbyUIPrefab = prefab;
    }

    public ILobbyUIBuilder WithParent(Transform parent)
    {
        _parent = parent;
        return this;
    }

    public ILobbyUIBuilder WithInitialization(LobbyUIController controller)
    {
        _controller = controller;
        return this;
    }

    public LobbyUI Build()
    {
        LobbyUI gme = GameObject.Instantiate(_lobbyUIPrefab, _parent);
        gme.Initialize(_controller);
        return gme;
    }
}

public interface ILobbyUIBuilder
{
    ILobbyUIBuilder WithParent(Transform parent);
    ILobbyUIBuilder WithInitialization(LobbyUIController controller);
    LobbyUI Build();

}
