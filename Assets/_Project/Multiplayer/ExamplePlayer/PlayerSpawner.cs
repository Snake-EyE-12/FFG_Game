using Unity.Netcode;
using UnityEngine;

public class PlayerSpawner : BindingController<PlayerSpawner, GamePlayer, GamePlayerBrain>
{
    // public override GamePlayer CreateBinding()
    // {
    //     //prefab.GetComponent<NetworkObject>().Spawn();
    //     return null;
    // }
}