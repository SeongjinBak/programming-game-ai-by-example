using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Steering_CH4 : MonoBehaviour
{
    void Start()
    {
        player = GetComponent<PlayerBase>();
        ball = GameObject.Find("Ball").GetComponent<SoccerBall>();
    }

    [SerializeField]
   private PlayerBase player;
    [SerializeField]
    private SoccerBall ball;
    [SerializeField]
    private Vector2 steeringForce;
    [SerializeField]
    private Vector2 target;
    [SerializeField]
    private float interposeDist;
    [SerializeField]
    private float multSeparation;
    [SerializeField]
    private float viewDistance;

    [SerializeField]
    int flags;

    [SerializeField]
    enum behavior_type
    {
        none = 0x0000,
        seek = 0x0001,
        arrive = 0x0002,
        separation = 0x0004,
        pursuit = 0x0008,
        interpose = 0x0010
    }

    [SerializeField]
    bool tagged;

    [SerializeField]
    enum Deceleration
    {
        slow = 3, normal = 2, fast = 1
    }


    Vector2 Seek(Vector2 target)
    {
        Vector2 DesiredVelocity = (target - (Vector2)player.transform.position).normalized * player.myMaxSpeed;
        
        return (DesiredVelocity - player.Velocity());
    } 

    Vector2 Arrive(Vector2 target, Deceleration decel)
    {
        Vector2 toTarget = target - (Vector2)player.transform.position;

        //calculate the distance to the target
        float dist = toTarget.magnitude;

        if (dist > 0)
        {
            //because Deceleration is enumerated as an int, this value is required
            //to provide fine tweaking of the deceleration..
            const float DecelerationTweaker = 0.3f;

            //calculate the speed required to reach the target given the desired
            //deceleration
            float speed = dist / ((float)decel * DecelerationTweaker);

            //make sure the velocity does not exceed the max
            speed = Mathf.Min(speed, player.myMaxSpeed);

            //from here proceed just like Seek except we don't need to normalize 
            //the ToTarget vector because we have already gone to the trouble
            //of calculating its length: dist. 
            Vector2 desiredVelocity = toTarget * speed / dist;

            return (desiredVelocity - player.Velocity());
        }

        return Vector2.zero;
    }

    Vector2 Pursuit(SoccerBall ball)
    {
        Vector2 toBall = ball.transform.position - player.transform.position;

        //the lookahead time is proportional to the distance between the ball
        //and the pursuer; 
        float lookAheadTime = 0.0f;

        if (ball.Speed() != 0.0)
        {
            lookAheadTime = toBall.magnitude / ball.Speed();
        }

        //calculate where the ball will be at this time in the future
        target = ball.FuturePosition(lookAheadTime);

        //now seek to the predicted future position of the ball
        return Arrive(target, Deceleration.fast);
    }

    Vector2 Separation()
    {
        //iterate through all the neighbors and calculate the vector from the
        Vector2 steeringForce = Vector2.zero;

        List<PlayerBase> allPlayers = new List<PlayerBase>(transform.parent.GetComponentsInChildren<PlayerBase>());

        foreach (var item in allPlayers)
        {
            if (item != player && item.Steering().Tagged())
            {
                Vector2 toAgent = player.transform.position - item.transform.position;
                steeringForce += (toAgent / toAgent.magnitude).normalized;
            }
        }
        return steeringForce;
    }

    Vector2 Interpose(SoccerBall ball, Vector2 pos, float distFromTarget)
    {
        return Arrive(target + ((Vector2)ball.transform.position - target).normalized *
                distFromTarget, Deceleration.normal);
    }

    void FindNeighbours()
    {
        List<PlayerBase> allPlayers = new List<PlayerBase>(transform.parent.GetComponentsInChildren<PlayerBase>());
        
        foreach(var item in allPlayers)
        {
            item.Steering().UnTag();
            Vector2 to = item.transform.position - player.transform.position;
            if(to.magnitude < (viewDistance))
            {
                item.Steering().Tag();
            }
        }
    }

    bool On(behavior_type bt) { return (flags & (int)bt) == (int)bt; }

    bool AccumulateForce(ref Vector2 sf, Vector2 forceToAdd)
    {
        float magnitudeSoFar = sf.magnitude;
        float magnitudeRemaining = player.myMaxSpeed - magnitudeSoFar;
        Debug.Log("Max Speed of Player is set as. " + player.myMaxSpeed);
        if (magnitudeSoFar <= 0f) return false;

        float magnitudeToAdd = forceToAdd.magnitude;

        if (magnitudeToAdd > magnitudeRemaining)
            magnitudeToAdd = magnitudeRemaining;
        sf += (forceToAdd).normalized * magnitudeToAdd;

        return true;
    }

    public Vector2 Calculate()
    {
        steeringForce = Vector2.zero;

        steeringForce = SumForces();

        if (steeringForce.sqrMagnitude >= player.myMaxForce.sqrMagnitude)
            steeringForce = player.myMaxForce;

        return steeringForce;
        
    }

    Vector2 SumForces()
    {
        Vector2 force = Vector2.zero;
        FindNeighbours();

        if (On( behavior_type.separation))
        {
            force += Separation() * multSeparation;

            if (!AccumulateForce(ref steeringForce, force)) return steeringForce;
        }

        if (On(behavior_type.seek))
        {
            force += Seek(target);

            if (!AccumulateForce(ref steeringForce, force)) return steeringForce;
        }

        if (On(behavior_type.arrive))
        {
            force += Arrive(target, Deceleration.fast);

            if (!AccumulateForce(ref steeringForce, force)) return steeringForce;
        }

        if (On(behavior_type.pursuit))
        {
            force += Pursuit(ball);

            if (!AccumulateForce(ref steeringForce, force)) return steeringForce;
        }

        if (On(behavior_type.interpose))
        {
            force += Interpose(ball, target, interposeDist);

            if (!AccumulateForce(ref steeringForce, force)) return steeringForce;
        }

        return steeringForce;
    }

    public float ForwardComponent()
    {
        return Vector2.Dot(player.Heading(), steeringForce);
    }

    public float SideComponent()
    {
        Debug.Log("SideComponet's max turn rate : " + player.myMaxTurnRate);
        return Vector2.Dot(player.SideVector(), steeringForce * player.myMaxTurnRate);
    }

    public Vector2 Force() { return steeringForce; }

    public Vector2 Target() { return target; }

    public void SetTarget(Vector2 t) { target = t; }

    public float InterposeDistance() { return interposeDist; }

    public void SetInterposeDistance(float d) { interposeDist = d; }

    public bool Tagged() { return tagged; }

    public void Tag() { tagged = true; }
    public void UnTag() { tagged = false; }

    public void SeekOn() { flags |= (int)behavior_type.seek; }

    public void ArriveOn() { flags |= (int)behavior_type.arrive; }

    public void PursuitOn() { flags |= (int)behavior_type.pursuit; }

    public void SeperationOn() { flags |= (int)behavior_type.separation; }

    public void InterposeOn() { flags |= (int)behavior_type.interpose; }

    public void SeekOff() { flags ^= (int)behavior_type.seek; }

    public void ArriveOff() { flags ^= (int)behavior_type.arrive; }

    public void PursuitOff() { flags ^= (int)behavior_type.pursuit; }

    public void SeperationOff() { flags ^= (int)behavior_type.separation; }

    public void InterposeOff() { flags ^= (int)behavior_type.interpose; }

    public bool SeekIsOn() { return On(behavior_type.seek); }

    public bool ArriveIsOn() { return On(behavior_type.arrive); }

    public bool PursuitIsOn() { return On(behavior_type.pursuit); }

    public bool SeperationIsOn() { return On(behavior_type.separation); }

    public bool InterposeIsOn() { return On(behavior_type.interpose); }

  
  
}
