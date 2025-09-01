using UnityEngine;
using UnityEngine.UI;

public class LobbyUI : MonoBehaviour
{
    [SerializeField] private LobbyReadyStateDisplay display;
    private LobbyUIController lobbyUIController;
    [SerializeField] private GameObject arrow;
    [SerializeField] private Image playerImage;
    public void ToggleReadyState()
    {
        if(lobbyUIController.IsOwner) lobbyUIController.ToggleReadyStateServerRpc();
    }
    public void Initialize(LobbyUIController controller)
    {
        lobbyUIController = controller;
        if (lobbyUIController.IsOwner)
        {
            arrow.SetActive(true);
            playerImage.sprite = GetRandomImage();
        }
        else
        {
            arrow.SetActive(false);
            playerImage.sprite = GetOtherPlayerImage();
        }
    }

    private Sprite GetRandomImage()
    {
        var faces = Resources.LoadAll<Sprite>("PlayerImages");
        if(faces.Length <= 0) return GetOtherPlayerImage();
        return faces[Random.Range(0, faces.Length)];
    }

    private Sprite GetOtherPlayerImage()
    {
        return Resources.Load<Sprite>("DefaultImage");
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