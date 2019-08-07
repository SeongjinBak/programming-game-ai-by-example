using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteeringBehaviors
{
    [SerializeField]
    private Vehicle vehicle;

    public float wanderingRadius;
    public Vector2 wanderTargetPoint;

    public SteeringBehaviors(GameObject gameObject)
    {
        Debug.Log("Constructor on");
        vehicle = gameObject.GetComponent<Vehicle>();
        
    }

    public Vector2 Seek(Vector2 targetPos)
    {
        Vector2 DesiredVelocity = (targetPos - vehicle.Pos()).normalized * vehicle.GetMaxSpeed();
        Debug.Log("Seeking");
        return DesiredVelocity - vehicle.GetVelocity();
    }

    public Vector2 Flee(Vector2 targetPos)
    {
        /*
        // If the target located in inside of PanicDistance area, vehicle only flee away from that.
        // Operated within powered space.
        const float panicDistanceSq = 10.0f * 10.0f;
        if((Vector2.Distance(vehicle.Pos(), targetPos)* Vector2.Distance(vehicle.Pos(), targetPos)) > panicDistanceSq)
        {
            return new Vector2(0f, 0f);
        }
        */

        Vector2 DesiredVelocity = (vehicle.Pos() - targetPos).normalized * vehicle.GetMaxSpeed();
        Debug.Log("Fleeing");
        return DesiredVelocity - vehicle.GetVelocity();
    }

    public Vector2 Arrive(Vector2 targetPos, Deceleration deceleration)
    {
        Vector2 toTarget = targetPos - vehicle.Pos();

        // calculate a distance to destination
        // Length()(a.k.a .magnitude) is using sqrt computation. It might cause overhead.
        float dist = toTarget.magnitude;
        if (dist > 0)
        {
            const float decelerationTweaker = .3f;

            float speed = dist / (float)deceleration * decelerationTweaker;

            speed = Mathf.Min(speed, vehicle.GetMaxSpeed());

            Vector2 desiredVelocity = toTarget * speed / dist;
            Debug.Log("Arriving");
            return desiredVelocity - vehicle.GetVelocity();
        }

        return new Vector2(0, 0f);
    }

    public Vector2 Pursuit(Vehicle evader)
    {
        Vector2 toEvader = evader.Pos() - vehicle.Pos();

        float relativeHeading = Vector2.Dot(vehicle.Heading(), evader.Heading());

        if (Vector2.Dot(toEvader, vehicle.Heading()) > 0 && 
            (relativeHeading < -0.95f))
        {
            return Seek(evader.Pos());
        }

        float lookAheadTime = toEvader.magnitude / (vehicle.GetMaxSpeed() + evader.GetVelocity().magnitude);
        Debug.Log("Pursuiting");
        return Seek(evader.Pos() + evader.GetVelocity() * lookAheadTime);
    }

    public Vector2 Evade(Vehicle pursuer)
    {
        Vector2 toPursuer = pursuer.Pos() - vehicle.Pos();

        float lookAheadTime = toPursuer.magnitude / (vehicle.GetMaxSpeed() + pursuer.GetVelocity().magnitude);
        Debug.Log("Evading");
        return Flee(pursuer.Pos() + pursuer.GetVelocity() * lookAheadTime);
    }

    public Vector2 Wander()
    {

        Vector2 wanderTarget = vehicle.Pos();
        float wanderRadius = 3f;
        float wanderDistance = 1f;
        
        // To draw gizmos.
        wanderingRadius = wanderRadius;

        float wanderJitter = 3f;
        wanderTarget += new Vector2(Random.Range(-1f, 1f) * wanderJitter, Random.Range(-1f, 1f) * wanderJitter);

        wanderTarget.Normalize();

        wanderTarget *= wanderRadius;
        
        // To show gizmos
        wanderTargetPoint = wanderTarget;

        Vector2 targetLocal = wanderTarget + new Vector2(wanderDistance, 0);
           

        //Vector2 targetWorld = GameObject.Find("Main Camera").GetComponent<Camera>().WorldToScreenPoint(targetLocal);
        Debug.Log("Wandering");
        return targetLocal - vehicle.Pos();
    }



    public Vector2 Calculate()
    {
        // Let's assume that calculate() returns just random value between .3f~.8f.
        // It needs to be updated when the chapter is finished.
        if (vehicle.CompareTag("Player"))
        {
            if (Input.GetKey(KeyCode.A))
            {
                return Seek(vehicle.target.transform.position);
            }
            else if (Input.GetKey(KeyCode.S))
            {
                return Flee(vehicle.target.transform.position);
            }
            else if (Input.GetKey(KeyCode.D))
            {
                if (Vector2.Distance(vehicle.Pos(), vehicle.target.transform.position) > 20f)
                {
                    return Arrive(vehicle.target.transform.position, Deceleration.fast);
                }
                else if (Vector2.Distance(vehicle.Pos(), vehicle.target.transform.position) > 10f)
                {
                    return Arrive(vehicle.target.transform.position, Deceleration.normal);
                }
                else
                {
                    return Arrive(vehicle.target.transform.position, Deceleration.slow);
                }
            }
            else if (Input.GetKey(KeyCode.F))
            {
                return Pursuit(vehicle.target.GetComponent<Vehicle>());
            }
            else if (Input.GetKey(KeyCode.Z))
            {
                return Evade(vehicle.target.GetComponent<Vehicle>());
            }
            else if (Input.GetKey(KeyCode.X))
            {
                return Wander();
            }
            else
                //return new Vector2(Random.Range(.3f,.8f), Random.Range(.3f, .8f));
                return new Vector2(0, 0);
        }
        else
        {
            //return new Vector2(Random.Range(.3f,.8f), Random.Range(.3f, .8f));
            return new Vector2(0, 0);
        }
    }

    
}
