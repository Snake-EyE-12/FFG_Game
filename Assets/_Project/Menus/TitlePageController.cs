using System;
using Cobra.Utilities;
using UnityEngine;
using UnityEngine.Events;

public class TitlePageController : MonoBehaviour
{
    public UnityEvent OnLoad;
    public void Quit()
    {
        Game.Quit();
    }

    private void Start()
    {
        OnLoad.Invoke();
    }
}
