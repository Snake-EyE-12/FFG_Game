using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyReadyStateDisplay : MonoBehaviour
{
    [SerializeField] private TMP_Text readyUpText;
    [SerializeField] private Image readyButton;
    [SerializeField] private string readyText;
    [SerializeField] private Color readyColor;
    [SerializeField] private string notReadyText;
    [SerializeField] private Color notReadyColor;
    
    public void DisplayReady()
    {
        readyUpText.text = readyText;
        readyButton.color = readyColor;
    }

    public void DisplayNotReady()
    {
        readyUpText.text = notReadyText;
        readyButton.color = notReadyColor;
    }
}