using NaughtyAttributes;
using UnityEngine;

public class ServerScoreController : MonoBehaviour
{
    [Button]
    private void GameEnd()
    {
        GameStarter.OnGameEnd?.Invoke();
    }
}
