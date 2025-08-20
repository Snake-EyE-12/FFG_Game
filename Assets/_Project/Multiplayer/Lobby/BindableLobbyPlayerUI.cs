using UnityEngine;


public class BindableLobbyPlayerUI : BindableObject<LobbyBindingController, BindableLobbyPlayerUI, PlayerLobbyUIBrain>
{
    [SerializeField] private LobbyReadyStateDisplay readyDisplayer;

    public void DisplayReady()
    {
        readyDisplayer.DisplayReady();
    }

    public void DisplayNotReady()
    {
        readyDisplayer.DisplayNotReady();
    }

    public void OnToggleReadyPressed()
    {
        if (bindedBrain.IsOwner) bindedBrain.ToggleReadyServerRpc();
    }
}
