using UnityEngine;

public class LobbyUI : MonoBehaviour
{
    [SerializeField] private LobbyReadyStateDisplay display;
    private LobbyUIController lobbyUIController;
    public void ToggleReadyState()
    {
        if(lobbyUIController.IsOwner) lobbyUIController.ToggleReadyStateServerRpc();
    }
    public void Initialize(LobbyUIController controller)
    {
        lobbyUIController = controller;
    }

    public void SetReady()
    {
        display.DisplayReady();
    }

    public void SetNotReady()
    {
        display.DisplayNotReady();
    }

    public bool IsClient(ulong clientId)
    {
        return lobbyUIController.OwnerClientId == clientId;
    }
}