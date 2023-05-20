using System.Collections.Generic;
using UnityEngine;

public class FlockingAgent : MonoBehaviour
{
    public float neighborRadius = 1.5f;
    public float separationWeight = 1f;
    public float alignmentWeight = 1f;
    public float cohesionWeight = 1f;
    public float moveSpeed = 1f;
    public float randomness = 1f;

    private Rigidbody2D rb;
    private List<FlockingAgent> neighbors;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        neighbors = new List<FlockingAgent>();
        RandomizeStartingDirection();
    }

    private void FixedUpdate()
    {
        FindNeighbors();

        Vector2 separationVector = CalculateSeparation();
        Vector2 alignmentVector = CalculateAlignment();
        Vector2 cohesionVector = CalculateCohesion();

        Vector2 flockingVector = (separationVector * separationWeight) +
                                 (alignmentVector * alignmentWeight) +
                                 (cohesionVector * cohesionWeight);

        Vector2 moveDirection = (flockingVector + Random.insideUnitCircle * randomness).normalized;
        rb.velocity = moveDirection * moveSpeed;
    }

    private void FindNeighbors()
    {
        neighbors.Clear();

        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, neighborRadius);

        foreach (var collider in colliders)
        {
            if (collider.CompareTag("FlockingAgent") && collider.gameObject != gameObject)
            {
                neighbors.Add(collider.GetComponent<FlockingAgent>());
            }
        }
    }

    private Vector2 CalculateSeparation()
    {
        Vector2 separationVector = Vector3.zero;

        foreach (var neighbor in neighbors)
        {
            Vector2 awayVector = transform.position - neighbor.transform.position;
            separationVector += awayVector.normalized / awayVector.magnitude;
        }

        return separationVector;
    }

    private Vector2 CalculateAlignment()
    {
        Vector2 alignmentVector = Vector3.zero;

        foreach (var neighbor in neighbors)
        {
            alignmentVector += neighbor.rb.velocity;
        }

        alignmentVector /= neighbors.Count;

        return alignmentVector;
    }

    private Vector2 CalculateCohesion()
    {
        Vector3 cohesionVector = Vector2.zero;

        foreach (var neighbor in neighbors)
        {
            cohesionVector += neighbor.transform.position;
        }

        cohesionVector /= neighbors.Count;
        cohesionVector -= transform.position;

        return cohesionVector;
    }

    private void RandomizeStartingDirection()
    {
        float angle = Random.Range(0f, 360f);
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }
}
