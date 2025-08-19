using UnityEngine;

public class LobbyPlayerUI : MonoBehaviour
{
    [SerializeField] private LobbyReadyStateDisplay readyDisplayer;
    private LobbyPlayerUIBinder binding;

    public void Initialize(LobbyPlayerUIBinder player)
    {
        binding = player;
        DisplayNotReady();
    }

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
        if (binding != null && binding.IsOwner)
            binding.ToggleReadyServerRpc();
    }
}