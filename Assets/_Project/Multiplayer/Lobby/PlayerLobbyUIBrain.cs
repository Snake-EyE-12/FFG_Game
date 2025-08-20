using Unity.Netcode;
using UnityEngine;

public class PlayerLobbyUIBrain : PlayerBrain<LobbyBindingController, BindableLobbyPlayerUI, PlayerLobbyUIBrain>
{
    [HideInInspector] public NetworkVariable<bool> Ready = new NetworkVariable<bool>(false);

    protected override void OnInitialized()
    {
        Ready.OnValueChanged += OnReadyChanged;
        if (IsServer) LobbyDataContainer.Join(this);
        OnReadyChanged(false, false);
    }
    public override void OnNetworkDespawn()
    {
        if (IsServer) LobbyDataContainer.Leave(this);
    }

    private void OnReadyChanged(bool oldValue, bool newValue)
    {
        if (binding != null)
        {
            if (newValue) binding.DisplayReady();
            else binding.DisplayNotReady();
        }
    }

    [ServerRpc]
    public void ToggleReadyServerRpc()
    {
        Ready.Value = !Ready.Value;

        if (Ready.Value) LobbyDataContainer.BecomeReady();
        else             LobbyDataContainer.BecomeNotReady();
    }
}