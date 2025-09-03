using System;
using TMPro;
using UnityEngine;

public class KillCountDisplay : MonoBehaviour
{
    [SerializeField] private Health health;
    [SerializeField] private TMP_Text text;

    private void OnEnable()
    {
        health.kills.OnValueChanged += SetText;
    }

    private void SetText(int old, int newValue)
    {
        text.text = newValue.ToString();
    }
}
