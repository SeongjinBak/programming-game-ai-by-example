using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoccerBall : MovingEntity_CH4
{
    [SerializeField]
    private Vector2 oldPos;
    [SerializeField]
    private Vector2 velocity;
    [SerializeField]
    // pointer of a player who got a ball
    private GameObject owner;

    [SerializeField]
    private Rigidbody2D ballRb;
    private void Start()
    {
        oldPos = transform.position;
        StartCoroutine(Updating());
    }
    public void TestCollisionWithWalls()
    {
        
    }

    public float Speed()
    {
        return velocity.magnitude;
    }

    public new Vector2 Velocity()
    {
        return velocity;
    }
    private void Awake()
    {
        ballRb = GetComponent<Rigidbody2D>();
        transform.position = new Vector2(0, 0);

    }
    
    public IEnumerator Updating()
    {
        while (true)
        {
            oldPos = transform.position;
            
            velocity += (velocity).normalized * -0.05f;
            
            transform.position = (Vector2)transform.position + velocity;

            v_heading = velocity.normalized;
            yield return new WaitForSeconds(0.05f);
        }
        
    }

    public bool HandleMessage(Telegram msg)
    {
        return false;
    }

    public void Kick(Vector2 direction, float force)
    {
       // velocity = new Vector2(0, 0);
        direction.Normalize();

        Vector2 acc = (direction * force) / ballRb.mass;

        velocity = acc;


    }

    public float TimeToCoverDistance(Vector2 from, Vector2 to, float force)
    {
        float speed = force / ballRb.mass;

        // Use Expression : v^2 = u^2 + ax;

        float distanceToCover = Vector2.Distance(from, to);

        float term = speed * speed + 2f * distanceToCover * ballRb.drag;

        if (term <= 0) return 0f;

        float v = Mathf.Sqrt(term);

        return (v - speed) / ballRb.drag;
    }

    public Vector2 FuturePosition(float time)
    {
        // Use Expression : x = ut  + 1/2at^2;

        Vector2 ut = v_velocity * time;
        // I use rigidbody.drag instead of Prm.Friction.
        float half_a_t_squared = 0.5f * ballRb.drag * time * time;

        Vector2 scalarToVector = half_a_t_squared * v_velocity.normalized;

        return (Vector2)transform.position + ut + scalarToVector;

    }

    // This method is used when keeper and player who trapping the ball trap(to stop the ball).
    public void Trap(GameObject owner)
    {
        v_velocity = new Vector2(0,0);
        this.owner = owner;
    }
    public void SetOwner(GameObject ow)
    {
        owner = ow;
    }
    public GameObject GetOwner()
    {
        return owner;
    }
    public Vector2 OldPos()
    {
        return oldPos;
    }

    public void PlaceAtPosition(Vector2 newPos)
    {
        transform.position = newPos;
    }
}
