using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PauseControl : MonoBehaviour
{
    [SerializeField] private RectTransform _pausePanel;
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

    public void Menu()
    {
        menuSceneChanger.ChangeScene();
    }

    public void Pause()
    {
        // Called Via Esc
        paused = true;
        pauseMenuTarget.anchoredPosition = _pausePanel.anchoredPosition;
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
