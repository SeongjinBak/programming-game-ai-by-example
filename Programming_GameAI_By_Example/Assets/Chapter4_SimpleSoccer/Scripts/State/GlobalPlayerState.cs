/*
 * 플레이어가 메시지를 수신하였을 때의 상태를 정의
 */ 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalPlayerState : State<FieldPlayer>
{
    public static GlobalPlayerState instance = null;
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

    public override bool OnMessage(FieldPlayer player, Telegram_CH4 telegram)
    {
        switch (telegram.GetMessageIndex())
        {
            case SoccerMessages.Msg_ReceiveBall:
                {
                    // set the target for this player
                    player.Steering().SetTarget(telegram.infos.position);
                    // change the state.
                    player.GetFSM().ChangeState(ReceiveBall.instance);

                    return true;
                }
            case SoccerMessages.Msg_SupportAttacker:
                {
                    if (player.GetFSM().IsInstate(SupportAttacker.instance))
                    {
                        return true;
                    }
                    player.Steering().SetTarget(SupportSpotCalculator.instance.DetermineBestSupportingPosition(player.transform.parent.GetComponent<SoccerTeam>().teamColor));

                    player.GetFSM().ChangeState(SupportAttacker.instance);
                    return true;
                }
            case SoccerMessages.Msg_GoHome:
                {
                    player.SetHomeRegion(player.transform.parent.GetComponent<SoccerTeam>().initialRegion[int.Parse(player.transform.name) - 1]);
                    player.GetFSM().ChangeState(ReturnToHomeRegion.instance);
                    return true;
                }
            case SoccerMessages.Msg_Wait:
                {
                    player.GetFSM().ChangeState(Wait.instance);
                    return true;
                }
            case SoccerMessages.Msg_PassToMe:
                {
                    FieldPlayer receiver = telegram.infos.gameObject.GetComponent<FieldPlayer>();

                    if (player.Team().Receiver() != null && !player.BallWithKickingRange())
                    {
                        return true;
                    }

                    player.Ball().SetOwner(receiver.gameObject);

                    player.Ball().Kick((receiver.transform.position - player.Ball().transform.position).normalized, Prm.instance.MaxPassingForce);
                    MessageDispatcher_CH4.instance.DispatchMessage(0f, player.Id(), receiver.Id(), SoccerMessages.Msg_ReceiveBall, receiver.transform);

                    player.GetFSM().ChangeState(Wait.instance);

                    player.FindSupport();
                    return true;
                }
        }

        return false;

    }



    public override void Enter(FieldPlayer player)
    {

    }

    public override void Execute(FieldPlayer player)
    {
        if(player.BallWithinReceivingRange() && player.Team().ControllingPlayer() == player)
        {
            player.SetMaxSpeed(Prm.instance.PlayerMaxSpeedWithBall);
        }
        else
        {
            player.SetMaxSpeed(Prm.instance.PlayerMaxSpeedWithoutBall);
        }
    }

    public override void Exit(FieldPlayer player)
    {
    }
}
