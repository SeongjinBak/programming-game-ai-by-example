using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingEntity_CH4 : BaseGameEntity_CH3
{
    [SerializeField]
    protected Vector2 v_velocity;
    [SerializeField]
    protected Vector2 v_heading;
    [SerializeField]
    protected Vector2 v_side;
    [SerializeField]
    protected float mass;

    protected float maxSpeed;

    protected float maxForce;

    protected float maxTurnRate;
    
    protected Vector2 Truncate(float maxSpeed)
    {
        float maxSpeedSqr = maxSpeed * maxSpeed;
        if(v_velocity.sqrMagnitude > maxSpeedSqr)
        {
            return v_velocity.normalized * maxSpeed;
        }
        else
        {
            return v_velocity;
        }
    }

    protected float LengthSq()
    {
        return v_velocity.sqrMagnitude;
    }

    public Vector2 Heading()
    {
        return v_heading;
    }
    public Vector2 SideVector()
    {
        return v_side;
    }
}
