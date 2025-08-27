using UnityEngine;

public class Frag : MonoBehaviour
{
    [SerializeField] private float duration;
    [SerializeField] private float explosionRadius;
    [SerializeField] private LayerMask hitMask;

    [HideInInspector] public Vector3 endPos;
    private Vector3 startPos;

    private float elapsedTime;

    private void Awake()
    {
        startPos = transform.position;
    }


    private void Update()
    {
        elapsedTime += Time.deltaTime;
        transform.position = Vector3.Slerp(startPos, endPos, elapsedTime / duration);

        if (elapsedTime >= duration)
        {
            Explode();
        }
    }

    private void Explode()
    {
        var hits = Physics.OverlapSphere(transform.position, explosionRadius, hitMask);
        foreach (var hit in hits)
        {
            if(hit.gameObject.TryGetComponent(out Health health))
            {
                health.HitPlayer();
            }
        }
        Destroy(gameObject);
    }
}
