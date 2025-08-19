using Unity.Netcode;
using UnityEngine;

public class LobbyPlayerUIBinder : NetworkBehaviour
{
    public NetworkVariable<bool> Ready = new NetworkVariable<bool>(false);

    private LobbyPlayerUI ui;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        // Create UI instance under canvas
        ui = FindFirstObjectByType<LobbyPlayerStorage>().CreateBinding();
        ui.Initialize(this);

        // Update UI when state changes
        Ready.OnValueChanged += OnReadyChanged;

        // Join lobby when spawned
        if (IsServer)
            LobbyDataContainer.Join(this);
    }

    public override void OnNetworkDespawn()
    {
        if (IsServer)
            LobbyDataContainer.Leave(this);
    }

    private void OnReadyChanged(bool oldValue, bool newValue)
    {
        if (ui != null)
        {
            if (newValue) ui.DisplayReady();
            else ui.DisplayNotReady();
        }
    }

    [ServerRpc]
    public void ToggleReadyServerRpc()
    {
        Ready.Value = !Ready.Value;

        if (Ready.Value)
            LobbyDataContainer.BecomeReady();
        else
            LobbyDataContainer.BecomeNotReady();
    }
}