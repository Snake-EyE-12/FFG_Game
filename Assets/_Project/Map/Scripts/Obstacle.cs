using System.Collections;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    [SerializeField] private float respawnTime;
    [SerializeField] private MeshRenderer mr;
    [SerializeField] private Collider coll;

    private bool destroyed = false;

    [ContextMenu("Destroy")]
    public void OnDestroyed()
    {
        destroyed = true;
        mr.enabled = false;
        coll.enabled = false;
        StartCoroutine(RespawnTimer());
    }

    public void Respawn()
    {
        destroyed = false;
        mr.enabled = true;
        coll.enabled = true;
    }

    private IEnumerator RespawnTimer()
    {
        yield return new WaitForSeconds(respawnTime);
        Respawn();
    }
}
