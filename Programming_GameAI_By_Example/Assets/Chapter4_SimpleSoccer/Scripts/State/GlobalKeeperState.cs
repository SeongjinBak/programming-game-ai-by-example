/*
 * 골키퍼 상태
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalKeeperState : State<GoalKeeper>
{
    public static GlobalKeeperState instance = null;
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

    // 골키퍼의 글로벌 상태는 수신한 메시지의 내용에 따라 달라진다.
    public override bool OnMessage(GoalKeeper keeper, Telegram_CH4 telegram)
    {
        switch (telegram.GetMessageIndex())
        {
            // 본 위치로 돌아가라는 명령
            case SoccerMessages.Msg_GoHome:
                {
                    // set the target for this player(original location of keeper)
                    keeper.Steering().SetTarget(keeper.Team().initialRegion[0]);
                    // change the state.
                    keeper.GetFSM().ChangeState(ReturnHome.instance);

                    return true;
                }

            // 공 가로채라는 명령
            case SoccerMessages.Msg_ReceiveBall:
                {
                    keeper.GetFSM().ChangeState(InterceptBall.instance);
                    return true;
                }
        }
        return false;
    }

}
