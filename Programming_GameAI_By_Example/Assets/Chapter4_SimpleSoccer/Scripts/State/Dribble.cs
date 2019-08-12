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
        player.Team().SetControllingPlayer(player.transform.parent.gameObject);
    }

    public override void Execute(FieldPlayer player)
    {
        float _dot = Vector2.Dot(player.Team().HomeGoal().Facing(), player.Heading());
        
        // If Ball is located in side of Home goal position, and player heading at home goal.
        if (_dot < 0)
        {
            Vector2 direction = player.Heading();
            float sign = Vector2.Dot(player.Team().HomeGoal().Facing(), player.Heading());
            sign = sign > 0 ? 1f : -1f;
            float angle = Mathf.PI / 4 * -1 * sign;

            player.transform.Rotate(direction, angle);

            const float kickingForce = .3f;

            player.Ball().Kick(direction, kickingForce);

        }
        // Kick the ball to home goal area.
        else
        {
            player.Ball().Kick(player.Team().HomeGoal().Facing(), Prm.instance.MaxDribbleForce);
        }
        //Player has to chase the ball after short term kicking.
        player.GetFSM().ChangeState(ChaseBall.instance);
        return;

    }

    public override void Exit(FieldPlayer player)
    {
        // Debug.Log("\n" + miner.GetNameOfEntity() + ": " + "I am leaving the gold mine with my pockets full, oh sweet gold");
    }
}
