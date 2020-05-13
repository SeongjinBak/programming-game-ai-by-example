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

    public void ResetBallState()
    {
        oldPos = Vector2.zero;
        velocity = Vector2.zero;
        owner = null;
        transform.position = Vector2.zero;
    }

    // 공의 속력
    public float Speed()
    {
        return velocity.magnitude;
    }

    // 공의 속도
    public new Vector2 Velocity()
    {
        return velocity;
    }

    private void Awake()
    {
        ballRb = GetComponent<Rigidbody2D>();
        transform.position = new Vector2(0, 0);
    }
    
    // 공의 이동
    public IEnumerator Updating()
    {
        while (true)
        {
            oldPos = transform.position;
            
            velocity += (velocity).normalized * -0.03f;
            
            transform.position = (Vector2)transform.position + velocity;

            v_heading = velocity.normalized;

            // 경기장 밖으로 나간경우 속도 0으로 지정
            if(transform.position.x < -24 || transform.position.x > 24 || transform.position.y > 10 || transform.position.y < -10)
            {
                velocity = new Vector2(0, 0);
            }
            yield return new WaitForSeconds(0.05f);
        }
        
    }

    public bool HandleMessage(Telegram msg)
    {
        return false;
    }

    // 공을 찬 경우, 공에 힘을 가함
    public void Kick(Vector2 direction, float force)
    {
        velocity = new Vector2(0, 0);
        direction.Normalize();

        Vector2 acc = (direction * force) / ballRb.mass;

        velocity = acc;
    }

    // 공의 이동 예측 시간
    public float TimeToCoverDistance(Vector2 from, Vector2 to, float force)
    {
        float speed = force / ballRb.mass;

        float distanceToCover = Vector2.Distance(from, to);

        float term = speed * speed + 2f * distanceToCover * ballRb.drag;

        if (term <= 0) return 0f;

        float v = Mathf.Sqrt(term);

        return (v - speed) / ballRb.drag;
    }

    // 공의 위치 예측
    public Vector2 FuturePosition(float time)
    {
        // Use Expression : x = ut  + 1/2at^2;

        Vector2 ut = v_velocity * time;

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
