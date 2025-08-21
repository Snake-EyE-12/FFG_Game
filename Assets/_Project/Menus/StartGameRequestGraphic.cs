using Unity.Netcode;
using UnityEngine;

public class StartGameRequestGraphic : NetworkBehaviour
{
    [SerializeField] private GameObject controlledObject;
    [SerializeField] private float duration = 2f;
    private float timeOfTurnOff;

    // Optional: only enable this behaviour on server/host
    public override void OnNetworkSpawn()
    {
        if (!IsServer) enabled = false; // server/host only logic
    }

    [ServerRpc(RequireOwnership = false)]
    public void DisplayServerRpc(ServerRpcParams rpcParams = default)
    {
        if (!IsServer) return; // defensive in case of mis-setup

        // This runs ONLY on the server/host world
        controlledObject.SetActive(true);
        timeOfTurnOff = Time.time + duration;
    }
    private void Update()
    {
        if (Time.time > timeOfTurnOff)
        {
            controlledObject.SetActive(false);
        }
    }
}