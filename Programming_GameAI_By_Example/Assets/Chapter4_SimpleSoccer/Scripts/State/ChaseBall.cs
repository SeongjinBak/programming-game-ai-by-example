using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseBall : State<FieldPlayer>
{
    public static ChaseBall instance = null;
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
        player.Steering().SeekOn();
    }

    public override void Execute(FieldPlayer player)
    {
        if (player.BallWithKickingRange())
        {
            player.GetFSM().ChangeState(KickBall.instance);
            return;
        }

        if (player.IsClosestTeamMemberToBall())
        {
            player.Steering().SetTarget(player.Ball().transform.position);
            return;
        }

        player.GetFSM().ChangeState(ReturnToHomeRegion.instance);
    }

    public override void Exit(FieldPlayer player)
    {
        player.Steering().SeekOff();
    }
}
