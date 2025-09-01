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
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            var child = transform.GetChild(i).gameObject;
            var entry = child.GetComponent<LobbyUI>();
            if (entry != null && entry.IsClient(clientId))
            {
                Destroy(child);
                break;
            }
        }
    }

}