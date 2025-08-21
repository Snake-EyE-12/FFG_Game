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

        StartCoroutine(ScaleOverTime(0.5f, Vector3.zero, Vector3.one));
        mr.enabled = true;
        coll.enabled = true;
    }

    private IEnumerator RespawnTimer()
    {
        yield return new WaitForSeconds(respawnTime);
        Respawn();
    }

    private IEnumerator ScaleOverTime(float time, Vector3 startScale, Vector3 targetScale)
    {
        float t = 0;
        transform.localScale = startScale;
        while (t < time)
        {
            t += Time.deltaTime;

            transform.localScale = Vector3.Lerp(startScale, targetScale, t/time);

            yield return null;
        }
        transform.localScale = targetScale;
    }
}
