using Unity.Netcode;
using UnityEngine;

public class ActiveLobbyPlayersContainer : NetworkBehaviour
{

    // Called when a client leaves the session
    public void OnLeftSession()
    {
        if (IsClient && !IsServer)
        {
            DestroyChildren();
            RequestRemoveLobbyEntryServerRpc(NetworkManager.Singleton.LocalClientId);
        }
        else if (IsServer)
        {
            RemoveLobbyEntryClientRpc(NetworkManager.Singleton.LocalClientId);
        }
    }

    private void DestroyChildren()
    {
        Debug.Log($"DestroyChildren() called with {transform.childCount} children", this);

        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            var child = transform.GetChild(i).gameObject;
            if (child != null)
            {
                Destroy(child);
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void RequestRemoveLobbyEntryServerRpc(ulong clientId)
    {
        RemoveLobbyEntryClientRpc(clientId);
    }

    [ClientRpc]
    private void RemoveLobbyEntryClientRpc(ulong clientId)
    {
        Debug.Log("Called 1");
        // Find the UI child representing this client
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Debug.Log("Called 2");
            var child = transform.GetChild(i).gameObject;
            var entry = child.GetComponent<LobbyUI>();
            if (entry != null && entry.IsClient(clientId))
            {
                Debug.Log("Called 3");
                Destroy(child);
                break;
            }
        }
    }

}