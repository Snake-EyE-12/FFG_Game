using UnityEngine;

public class Frag : MonoBehaviour
{
    [SerializeField] private float duration;
    [SerializeField] private float explosionRadius;
    [SerializeField] private LayerMask hitMask;
    [SerializeField] private ParticleSystem explosion;

    public GameObject parent;
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
        var hits = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (var hit in hits)
        {
            if(hit.gameObject.TryGetComponent(out Health health) && hit.transform.gameObject != parent)
            {
                health.HitPlayer();
            }
            else if (hit.gameObject.TryGetComponent(out Obstacle obs))
            {
                obs.OnDestroyed();
            }
        }
        Instantiate(explosion, transform.position, transform.rotation);
        Destroy(gameObject);
    }
}
