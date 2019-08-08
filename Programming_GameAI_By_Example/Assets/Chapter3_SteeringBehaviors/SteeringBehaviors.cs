using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteeringBehaviors
{
    [SerializeField]
    private Vehicle vehicle;

    public float wanderingRadius;
    public Vector2 interposePoint;
    public Vector2 wanderTargetPoint;
    public Vector2 hidePoint;
    public List<Transform> obstacles;
    public List<Transform> wayPoints;

    public SteeringBehaviors(GameObject gameObject)
    {
        Debug.Log("Constructor on : " + gameObject.name);
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

        float wanderJitter = 1f;
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

    
    public void FindObstaclesInScene()
    {
        // At this time, there are rigidbody and collider attached. so after write this code, Must remove those.
        var group = GameObject.Find("Obstacles").GetComponentsInChildren<Transform>();
        if(group != null)
        {
            obstacles = new List<Transform>(group);
            obstacles.RemoveAt(0);
        }
    }

    public void FindWayPointsInScene()
    {
        var group = GameObject.Find("WayPoint").GetComponentsInChildren<Transform>();
        if (group != null)
        {
            wayPoints = new List<Transform>(group);
            wayPoints.RemoveAt(0);
        }
    }

    /*
    public Vector2 ObstacleAvoidance()
    {
        if(obstacles ==null)
        {
            FindObstaclesInScene();
        }

        // Let assume that MinDetectionBoxLength is 2f.
        float boxLength = 2f + (vehicle.GetVelocity().magnitude / vehicle.GetMaxSpeed()) * 2f;
        obstacleDetectionLength = boxLength;
        Debug.Log("D");
        Vector2[] boxRanges = new Vector2[3];

        boxRanges[0] = vehicle.Pos() + vehicle.Heading() * boxLength;
        boxRanges[1] = vehicle.Pos() + vehicle.SideVector() + vehicle.Heading() * boxLength;
        boxRanges[2] = vehicle.Pos() + (-1) * vehicle.SideVector() + vehicle.Heading() * boxLength;

        // Detect an obstacle inside of the detection box.
        foreach (var item in obstacles)
        {
            if(item.position.x > vehicle.Pos().x && item.position.x < boxRanges[0].x && item.position.y > vehicle.Pos().y && item.position.y < boxRanges[0].y)
            {
                Debug.Log("D");
                float a = -(boxRanges[2].y - (vehicle.Pos().y + (-1) * vehicle.SideVector().y));
                float b = boxRanges[2].x - (vehicle.Pos().x + (-1) * vehicle.SideVector().x);
                float c = -(a* (vehicle.Pos().x + (-1) * vehicle.SideVector().x) + b * vehicle.Pos().y + (-1) * vehicle.SideVector().y);
                if (a * item.position.x + b * item.position.y + c < 0)
                {
                    Debug.Log("D");
                    a = -(boxRanges[1].y - (vehicle.Pos().y + vehicle.SideVector().y));
                    b = boxRanges[1].x - (vehicle.Pos().x + vehicle.SideVector().x);
                    c = -(a * (vehicle.Pos().x + vehicle.SideVector().x) + b * vehicle.Pos().y + vehicle.SideVector().y);
                    if (a * item.position.x + b * item.position.y + c > 0)
                    {
                        if(item.position.x < boxRanges[0].x)
                        {
                            // tagging
                            item.GetComponent<BaseGameEntity_CH3>().Tag();
                            Debug.Log("Tagged : " + item.name);
                        }
                    }
                }
            }
        }



        return vehicle.GetVelocity();

    }
    */

    public Vector2 Interpose(Vehicle agentA, Vehicle agentB)
    {
        Vector2 midPoint = (agentA.Pos() + agentB.Pos()) / 2f;

        float timeToReachMidPoint = Vector2.Distance(vehicle.Pos(), midPoint) / vehicle.GetMaxSpeed();

        Vector2 aPos = agentA.Pos() + agentA.GetVelocity() * timeToReachMidPoint;
        Vector2 bPos = agentB.Pos() + agentB.GetVelocity() * timeToReachMidPoint;

        midPoint = (aPos + bPos) / 2f;
        interposePoint = midPoint;
        Debug.Log("Interposing");
        return Arrive(midPoint, Deceleration.fast);
    }


    public Vector2 GetHidingPosition(Vector2 posOb, float radiusOb, Vector2 posTarget)
    {
        float distanceFromBoundary = 1.0f;
        float distAway = radiusOb + distanceFromBoundary;

        // Calculate the direction from Target to object.
        Vector2 toOb = (posOb - posTarget).normalized;

        return (toOb * distAway) + posOb;
    }

    public Vector2 Hide(Vehicle target)
    {
        if (obstacles == null)
        {
            FindObstaclesInScene();
        }

        // Let's assume that 987654321 => MaxFloat
        float distToClosest = 987654321f;
        Vector2 bestHidingSpot = new Vector2(0f,0f);

        foreach (var item in obstacles)
        {
            Vector2 hidingSpot = GetHidingPosition(item.position, 2f, target.Pos());

            // Finding the nearest hiding spot to agent.
            float dist = Vector2.Distance(hidingSpot, vehicle.Pos());

            if(dist < distToClosest)
            {
                distToClosest = dist;
                bestHidingSpot = hidingSpot;
            }
        }

        if(distToClosest == 987654321f)
        {
            return Evade(target);
        }
        Debug.Log("Hiding ..  at " + bestHidingSpot);
        hidePoint = bestHidingSpot;
        return Arrive(bestHidingSpot, Deceleration.fast);
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
            else if (Input.GetKey(KeyCode.C))
            {
                return Interpose(GameObject.Find("Agent_A").GetComponent<Vehicle>(), GameObject.Find("Agent_B").GetComponent<Vehicle>());
            }
            else if (Input.GetKey(KeyCode.V))
            {
                return Hide(GameObject.Find("Agent_C").GetComponent<Vehicle>());
            }
            else
                //return new Vector2(Random.Range(.3f,.8f), Random.Range(.3f, .8f));
                return new Vector2(0, 0);
        }
        else
        {
            //return new Vector2(Random.Range(.3f,.8f), Random.Range(.3f, .8f));
            if (!vehicle.name.Contains("Agent"))
            {
                return new Vector2(0, 0);
            }
            return vehicle.GetVelocity();
        }
    }

    
}
