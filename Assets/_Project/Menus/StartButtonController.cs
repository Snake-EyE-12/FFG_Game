using System;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class StartButtonController : MonoBehaviour
{
    [SerializeField] private NetworkSceneChanger sceneChanger;
    [SerializeField] private StartGameRequestGraphic graphics;
    [SerializeField] private GameObject controlledObj;
    [SerializeField] private int minimumPlayerCount = 2;

    private bool CorrectPlayerCount => NetworkManager.Singleton.ConnectedClients.Count >= minimumPlayerCount;
    private bool AllReady => NetworkManager.Singleton.SpawnManager.SpawnedObjects.Values
        .ToList()
        .Where(x => x.TryGetComponent<LobbyUIController>(out _))
        .Count(x => x.GetComponent<LobbyUIController>().isReady)
        >= NetworkManager.Singleton.ConnectedClients.Count
        ;

    public void OnReadyStateChanged()
    {
        controlledObj.SetActive(CorrectPlayerCount && AllReady);
    }

    private void OnEnable()
    {
        LobbyUIController.OnAnyReadyStateChanged += OnReadyStateChanged;
    }

    private void OnDisable()
    {
        LobbyUIController.OnAnyReadyStateChanged -= OnReadyStateChanged;
    }

    public void OnStartButtonClicked()
    {
        if (NetworkManager.Singleton.IsServer)
        {
            sceneChanger.ChangeScene();
        }
        else
        {
            graphics.DisplayServerRpc();
        }
    }
}
