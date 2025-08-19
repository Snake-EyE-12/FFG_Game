using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class HostOnlyButton : MonoBehaviour
{
    void Start()
    {
        // disable button if not host
        if (!NetworkManager.Singleton.IsHost)
            GetComponent<Button>().interactable = false;
    }
}