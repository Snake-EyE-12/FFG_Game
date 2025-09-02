using Unity.Netcode;
using UnityEngine;

public class NetworkBootstrap : MonoBehaviour
{
    [SerializeField] private GameObject networkManagerPrefab;

    private void Awake()
    {
        if (NetworkManager.Singleton == null)
        {
            Instantiate(networkManagerPrefab);
        }
    }
}