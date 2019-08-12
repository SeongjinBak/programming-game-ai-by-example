using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wait : State<FieldPlayer>
{
    public static Wait instance = null;
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

    }

    public override void Execute(FieldPlayer player)
    {
        if (!player.AtTarget())
        {
            player.Steering().ArriveOn();
            return;
        }
        else
        {
            player.Steering().ArriveOff();
            player.SetVelocity(Vector2.zero);

            player.TrackBall();
        }
        
        if(player.Team().InControl() && !player.IsControllingPlayer() && player.IsAheadOfAttacker())
        {
            player.Team().RequestPass(player);
            return;
        }

        if (player.Pitch().GameOn())
        {
            if(player.IsClosestTeamMemberToBall() && player.Team().Receiver() == null && !player.Pitch().GoalKeeperHasBall())
            {
                player.GetFSM().ChangeState(ChaseBall.instance);
                return;
            }
        }

    }

    public override void Exit(FieldPlayer player)
    {
        // Debug.Log("\n" + miner.GetNameOfEntity() + ": " + "I am leaving the gold mine with my pockets full, oh sweet gold");
    }
}
