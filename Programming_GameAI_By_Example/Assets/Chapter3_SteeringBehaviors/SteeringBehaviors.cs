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
    public List<Vehicle> neighbors;
    public Vector2 offsetPursuitPos;
    

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
        Debug.Log("Hiding ..  spot is " + bestHidingSpot);
        hidePoint = bestHidingSpot;
        return Arrive(bestHidingSpot, Deceleration.fast);
    }

    public Vector2 OffsetPursuit(Vehicle leader, Vector2 offset)
    {
        Vector2 worldOffsetPos = leader.Pos()+  offset;

        Vector2 ToOffset = worldOffsetPos - vehicle.Pos();

        float lookAheadTime = ToOffset.magnitude / (vehicle.GetMaxSpeed() + leader.GetVelocity().magnitude);
    
        return Arrive(worldOffsetPos + leader.GetVelocity() * lookAheadTime, Deceleration.fast);


    }

    public void FindNeighborsInScene()
    {
        var group = GameObject.Find("Neighbors").GetComponentsInChildren<Vehicle>();
        if (group != null)
        {
            neighbors = new List<Vehicle>(group);
            neighbors.RemoveAt(0);
        }
        
    }

    public void TagNeighbors<T>(T entity, List<T> containerOfEntities, float radius)
    {

        foreach (var item in containerOfEntities)
        {
            if (item is Vehicle v && entity is Vehicle c)
            {

                v.UnTag();

                Vector2 to = v.Pos() - c.Pos();

                float range = radius + v.BRadius();

                if (v != c && to.SqrMagnitude() < range * range)
                {
                    v.Tag();
                }
            }
        }

    }
    public Vector2 Seperation(List<Vehicle> neighbors)
    {
        Vector2 steeringForce = new Vector2(0,0);
        for(int a = 0; a < neighbors.Count; ++a)
        {
            if(neighbors[a] != vehicle && neighbors[a].IsTagged())
            {
                Vector2 toAgent = vehicle.Pos() - neighbors[a].Pos();

                steeringForce += toAgent.normalized / toAgent.magnitude;
            }
        }

        return steeringForce;
    }

    public Vector2 Alignment(List<Vehicle> neighbors)
    {
        Vector2 averageHeading = new Vector2(0,0);

        int neighborCount = 0;

        for(int a = 0; a < neighbors.Count; a++)
        {
            if(neighbors[a] != vehicle && neighbors[a].IsTagged())
            {
                averageHeading += neighbors[a].Heading();
                ++neighborCount;
            }
        }

        if(neighborCount > 0)
        {
            averageHeading /= (float)neighborCount;

            averageHeading -= vehicle.Heading();
        }
        return averageHeading;
    }

    public Vector2 Cohesion(List<Vehicle> neighbors)
    {
        Vector2 centerOfMass, steeringForce;
        centerOfMass = new Vector2(0, 0);
        steeringForce = new Vector2(0, 0);
        int neighborCount = 0;

        for(int i = 0; i < neighbors.Count; i++)
        {
            if(neighbors[i]!=vehicle && neighbors[i].IsTagged())
            {
                centerOfMass += neighbors[i].Pos();
                ++neighborCount;
            }
        }

        if (neighborCount > 0)
        {
            centerOfMass /= neighborCount;
            steeringForce = Seek(centerOfMass);
        }
        Debug.Log(centerOfMass);
        return steeringForce;
    }

    public Vector2 Calculate()
    {
        if (vehicle.CompareTag("Follower"))
        {
            // if Offset value is zero, we have to calculate the length between Leader and this follower.
            // After calculating, we'll get the Length. This Length will be maintained during Moving. 
            if(offsetPursuitPos.x == 0 && offsetPursuitPos.y == 0)
            {
                 offsetPursuitPos = new Vector2 (vehicle.Pos().x - GameObject.FindGameObjectWithTag("Player").GetComponent<Vehicle>().WorldPos().x, vehicle.Pos().y - GameObject.FindGameObjectWithTag("Player").GetComponent<Vehicle>().WorldPos().y) ;
            }
            return OffsetPursuit(GameObject.FindGameObjectWithTag("Player").GetComponent<Vehicle>(), offsetPursuitPos);
        }
        if (vehicle.CompareTag("Neighbor"))
        {
            if (neighbors == null)
            {
                FindNeighborsInScene();
            }

            if (Input.GetKey(KeyCode.R))
            {
                TagNeighbors<Vehicle>(vehicle, neighbors, 15f);
                Debug.Log("Seperation");
                return Seperation(neighbors);
            }
            else if (Input.GetKey(KeyCode.E))
            {
                TagNeighbors<Vehicle>(vehicle, neighbors, 100f);
                Debug.Log("Cohesion");

                return Cohesion(neighbors);
            }
        }


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
            else if (Input.GetKey(KeyCode.W))
            {
                if (neighbors == null)
                {
                    FindNeighborsInScene();
                }
                TagNeighbors<Vehicle>(vehicle, neighbors, 8f);

                return Alignment(neighbors);
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
