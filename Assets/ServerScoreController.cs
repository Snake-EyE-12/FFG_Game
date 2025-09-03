using Cobra.DesignPattern;
using NaughtyAttributes;
using Unity.Netcode;
using UnityEngine;

public class ServerScoreController : NetworkBehaviour
{
    public static ServerScoreController Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(this.gameObject);
    }
    [Button]
    private void GameEnd()
    {
        GameStarter.OnGameEnd?.Invoke();
    }
    
    
    [ServerRpc(RequireOwnership = false)]
    public void ThisGuyKilledThisGuyServerRpc(
        NetworkObjectReference killerRef,
        NetworkObjectReference killedRef)
    {
        Debug.Log("Called");
        if (!killerRef.TryGet(out var killerObj) || !killedRef.TryGet(out var killedObj))
        {
            Debug.LogWarning("Could not resolve NetworkObjectReference(s).");
            return;
        }

        var killer = killerObj.GetComponentInChildren<Health>();
        var killed = killedObj.GetComponentInChildren<Health>();
        if (killer == null || killed == null)
        {
            Debug.Log("FAILING");
            return;
        }

        if (killer.kills.Value == 2)
        {
            GameEnd();
            return;
        }

        killer.kills.Value++;
        killed.kills.Value = 0;
    }
    
}
