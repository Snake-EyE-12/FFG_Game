using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkSceneChanger : MonoBehaviour
{
    [SerializeField] private string sceneToLoad;
    public void ChangeScene()
    {
        NetworkManager.Singleton.SceneManager.LoadScene(sceneToLoad, LoadSceneMode.Single);
    }
}