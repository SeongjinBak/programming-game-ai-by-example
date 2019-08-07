using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vehicle : MovingEntity
{
    private SteeringBehaviors pSteering;
    public GameObject target;
    // verify scale of the desired velocity.
    public Vector2 desiredVelocity;

    private void Start()
    {
        pSteering = new SteeringBehaviors(this.gameObject);
        v_velocity = new Vector2(0f, 0f);
        mass = 1f;
        maxSpeed = 10f;
        _pos = transform.position;
        v_heading = transform.localEulerAngles;
        StartCoroutine(Updating());

    }

    public float GetMaxSpeed()
    {
        return maxSpeed;
    }
    public Vector2 GetVelocity()
    {
        return v_velocity;
    }
   
    public IEnumerator Updating()
    {
        while (true)
        {
            Vector2 steeringForce = pSteering.Calculate();
            // just for inspecter view and gizmos
            desiredVelocity = steeringForce;
            Vector2 acceleration = steeringForce / mass;

            // Renewal the speed;
            v_velocity += acceleration * 0.5f;
            Truncate(maxSpeed);
            _pos += v_velocity * 0.5f;
            transform.position = _pos;
            if(LengthSq() > 0.0000001f)
            {
                v_heading = v_velocity.normalized;
                v_side = Vector2.Perpendicular(v_heading);
              
            }
            yield return new WaitForSeconds(0.5f);
        }
        
    }
    private void Update()
    {
        if (Input.GetKey(KeyCode.Q))
        {
            if(CompareTag("Player"))
                SetPos(new Vector2(0f, 0f));
        }
    }
    private void OnDrawGizmos()
    {
        if (CompareTag("Player"))
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, v_heading);
            Gizmos.color = Color.green;
            Gizmos.DrawLine(Pos(), v_velocity);
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(Pos(), desiredVelocity);
            Gizmos.color = Color.grey;
            Gizmos.DrawWireSphere(Pos(), pSteering.wanderingRadius);
            Gizmos.color = Color.magenta;
            Gizmos.DrawIcon(pSteering.wanderTargetPoint, "shell");
        }
    }
}
