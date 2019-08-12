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
                    

                    player.Ball().Kick(receiver.transform.position - player.Ball().transform.position, Prm.instance.MaxPassingForce);
                    Debug.Log("p179 공 패스 및 메시지 전송 구현 했음. 특이사항 : extra info 로 receiver의 transform을 넘겼음. ");
                    MessageDispatcher_CH4.instance.DispatchMessage(0f, player.ID(), receiver.ID(), SoccerMessages.Msg_ReceiveBall, receiver.transform);

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
        // Debug.Log("\n" + miner.GetNameOfEntity() + ": " + "I am leaving the gold mine with my pockets full, oh sweet gold");
    }
}
