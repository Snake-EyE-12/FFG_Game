using UnityEngine;

public class SimpleMovement : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;
    [SerializeField] private float moveSpeed = 10;
    private Vector2 lastMovement;
    public void Move(Vector2 dir)
    {
        lastMovement = dir;
    }

    private void FixedUpdate()
    {
        rb.MovePosition(transform.position + new Vector3(lastMovement.x * moveSpeed, 0, lastMovement.y * moveSpeed));
    }
    public void Zero()
    {
        rb.linearVelocity = Vector3.zero;
    }
}