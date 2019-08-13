using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dribble : State<FieldPlayer>
{
    public static Dribble instance = null;
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
        {
            Destroy(this.gameObject);
        }
        DontDestroyOnLoad(this.gameObject);
    }
    public override void Enter(FieldPlayer player)
    {
        player.Team().SetControllingPlayer(player.gameObject);
    }

    public override void Execute(FieldPlayer player)
    {
        float _dot = Vector2.Dot(player.Team().HomeGoal().Facing(), player.Heading());
        
        // If Ball is located in side of Home goal position, and player heading at home goal.
        if (_dot < 0)
        {
            Vector2 direction = -player.Heading();
            float sign = Vector2.Dot(player.Team().HomeGoal().Facing(), player.Heading());
            sign = sign > 0 ? 1f : -1f;
            float angle = Mathf.PI / 4 * -1 * sign;

            player.transform.Rotate(direction, angle);

            const float kickingForce = .2f;
            print("Drilble1 1 " + kickingForce);
            player.Ball().SetOwner(player.gameObject);
            player.Ball().Kick(direction, kickingForce);

        }
        // Kick the ball to home goal area.
        else
        {
            print("Drilble2  " + Prm.instance.MaxDribbleForce);
            player.Ball().SetOwner(player.gameObject);
            player.Ball().Kick(player.Team().HomeGoal().Facing(), .5f);
        }
        //Player has to chase the ball after short term kicking.
        player.GetFSM().ChangeState(ChaseBall.instance);
        return;

    }

    public override void Exit(FieldPlayer player)
    {
     
    }
}
