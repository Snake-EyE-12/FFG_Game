using System;
using System.Collections.Generic;
using NaughtyAttributes;
using Unity.Multiplayer.Center.NetcodeForGameObjectsExample.DistributedAuthority;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

public class ItemSpawner : MonoBehaviour
{
	
    private static List<Vector3> itemPickUpLocations = new List<Vector3>();
    
    public static void AddSpawnLocation(Vector3 pos)
    {
        itemPickUpLocations.Add(pos);
    }


    [SerializeField] private PickUp pickUpPrefab;

    
    public void ResetLocations()
    {
        itemPickUpLocations.Clear();
    }
    private List<Vector3> availableLocations;
    private List<ActiveSpawnLocationInUse> inUse = new();
    private bool prepared;
    public void PrepareSpawnableLocations()
    {
        if (prepared) return;
        availableLocations = new List<Vector3>(itemPickUpLocations);
        prepared = true;
    }

    [Button]
    public void SpawnItem()
    {
        PrepareSpawnableLocations();
        if (availableLocations.Count == 0) return;
        int index = Random.Range(0, availableLocations.Count);
        Vector3 newSpawnItemLocation = availableLocations[index];
        inUse.Add(CreateObject(newSpawnItemLocation));
        availableLocations.RemoveAt(index);
    }

    public void ReleaseItem(NetworkObject no)
    {
        for (int i = 0; i < inUse.Count; i++)
        {
            if (inUse[i].Equals(no))
            {
                availableLocations.Add(inUse[i].position);
                inUse.RemoveAt(i);
            }
        }
    }

    private ActiveSpawnLocationInUse CreateObject(Vector3 pos)
    {
        PickUp pickUp = Instantiate(pickUpPrefab, pos, Quaternion.identity);
        pickUp.OnSpawnItem(this);
        NetworkObject no = pickUp.GetComponent<NetworkObject>();
        no.Spawn();
        return new ActiveSpawnLocationInUse(pos, no);
    }
}

public class ActiveSpawnLocationInUse
{
    public Vector3 position;
    public NetworkObject pickUp;

    public ActiveSpawnLocationInUse(Vector3 pos, NetworkObject item)
    {
        this.position = pos;
        pickUp = item;
    }
    public bool Equals(NetworkObject other)
    {
        return other.Equals(pickUp);
    }
}

public abstract class PickUp : NetworkBehaviour
{
    private ItemSpawner itemSpawner;
    [SerializeField] protected NetworkObject networkObject;
    public void OnSpawnItem(ItemSpawner spawner)
    {
        itemSpawner = spawner;
        OnSpawn();
    }

    
    public void PickUpItem()
    {
        itemSpawner.ReleaseItem(networkObject);
        OnPickUp();
        networkObject.Despawn();
    }
    protected abstract void OnPickUp();
    protected abstract void OnSpawn();

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Health>(out Health h))
        {
            h.RanIntoPickup(this);
        }
    }
}