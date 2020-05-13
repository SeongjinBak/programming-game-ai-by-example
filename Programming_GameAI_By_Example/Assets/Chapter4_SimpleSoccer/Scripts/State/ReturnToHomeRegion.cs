/*
 * Return to home region State.
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReturnToHomeRegion : State<FieldPlayer>
{
    public static ReturnToHomeRegion instance = null;
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
        player.Steering().ArriveOn();

        if(player.transform.parent.GetComponent<SoccerTeam>().teamColor == TeamColor.Blue)
        {
            if(player.Steering().Target().x < 0f)
            {
                player.Steering().SetTarget(player.Team().initialRegion[player.Id() - 6]);
            }
        }
        else
        {
            if (player.Steering().Target().x > 0f)
            {
                player.Steering().SetTarget(player.Team().initialRegion[player.Id() - 1]);
            }
        }
    }

    public override void Execute(FieldPlayer player)
    {
        // 게임이 시작되고, 본인이 공과 가장 가까운 경우 공을 추적
        if (SoccerPitch.instance.GameOn())
        {
            if (player.IsClosestTeamMemberToBall() && (player.Team().Receiver() == null) && !SoccerPitch.instance.GoalKeeperHasBall())
            {
                player.GetFSM().ChangeState(ChaseBall.instance);
                return;
            }
        }

        // 게임 시작시 복귀 지점으로 이동
        if (SoccerPitch.instance.GameOn())
        {
            if (player.transform.parent.GetComponent<SoccerTeam>().teamColor == TeamColor.Blue)
            {
                player.Steering().SetTarget(player.Team().initialRegion[player.Id() - 6]);
            }
            else
            {
                player.Steering().SetTarget(player.Team().initialRegion[player.Id() - 1]);
            }
        }
        else if(!SoccerPitch.instance.GameOn() && player.AtTarget())
        {
            player.GetFSM().ChangeState(Wait.instance);
        }

    }

    public override void Exit(FieldPlayer player)
    {
        player.Steering().ArriveOff();
    }
}
