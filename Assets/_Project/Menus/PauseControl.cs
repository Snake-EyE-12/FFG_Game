using System;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseControl : MonoBehaviour
{
    [SerializeField] private RectTransform pauseMenuTarget;
    private Vector3 pauseMenuPosition;
    private bool paused = false;
    [SerializeField] private NetworkSceneChanger menuSceneChanger;
    private void Awake()
    {
        paused = false;
        pauseMenuPosition = pauseMenuTarget.anchoredPosition;
    }

    public void Resume()
    {
        paused = false;
        pauseMenuTarget.anchoredPosition = pauseMenuPosition;
    }
    
    
    
    
    
    private void Start()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnect;
        }
    }

    public void Menu()
    {
        CleanupAndReturnToMenu();
    }

    private void CleanupAndReturnToMenu()
    {
        if (NetworkManager.Singleton != null)
        {
            // --- Host kicks clients ---
            if (NetworkManager.Singleton.IsServer)
            {
                var clientsToKick = new List<ulong>(NetworkManager.Singleton.ConnectedClientsIds);
                foreach (var clientId in clientsToKick)
                {
                    if (clientId != NetworkManager.Singleton.LocalClientId)
                        NetworkManager.Singleton.DisconnectClient(clientId);
                }
            }

            // --- Shutdown NGO ---
            if (NetworkManager.Singleton.IsListening)
            {
                NetworkManager.Singleton.Shutdown();
            }

            // --- Reset transport ---
            var transport = NetworkManager.Singleton.NetworkConfig.NetworkTransport as UnityTransport;
            if (transport != null)
            {
                transport.Shutdown();
            }

            // --- Destroy NetworkManager singleton ---
            Destroy(NetworkManager.Singleton.gameObject);
        }

        // --- Load menu ---
        SceneManager.LoadScene("MainMenu");
    }

    private void OnClientDisconnect(ulong clientId)
    {
        // If the host disconnected while we are a client, leave too
        if (!NetworkManager.Singleton.IsServer && clientId == NetworkManager.ServerClientId)
        {
            Debug.Log("Host disconnected. Returning to Main Menu...");
            CleanupAndReturnToMenu();
        }
    }
    
    
    
    
    
    
    
    
    


    public void Pause()
    {
        // Called Via Esc
        paused = true;
        pauseMenuTarget.anchoredPosition = Vector3.zero;
    }

    private void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (paused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }
}