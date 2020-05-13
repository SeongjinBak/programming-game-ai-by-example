/*
 * 경기에서, 골키퍼가 공으로 멀리 롱패스하는 상태
 */ 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PutBallBackInPlay : State<GoalKeeper>
{
    public static PutBallBackInPlay instance = null;
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

    public override void Enter(GoalKeeper keeper)
    {
        keeper.Team().SetControllingPlayer(gameObject);

        keeper.Team().opponentTeam.ReturnAllFieldPlayersToHome();
        keeper.Team().ReturnAllFieldPlayersToHome();
    }
  
    public override void Execute(GoalKeeper keeper)
    {
        PlayerBase receiver = keeper.Team().players[Random.Range(1,5)].GetComponent<PlayerBase>();
        Vector2 ballTarget = new Vector2(0f,0f);

        if(!keeper.Team().AllPlayersAtHome() || !keeper.Team().opponentTeam.AllPlayersAtHome())
        {
            keeper.Team().opponentTeam.ReturnAllFieldPlayersToHome();
            keeper.Team().ReturnAllFieldPlayersToHome();
            return;
        }


        if (keeper.Team().FindPass(keeper, receiver, ref ballTarget, Prm.instance.MaxPassingForce, Prm.instance.GoalKeeperMinPassDist))
        {
            if(keeper.Team().AllPlayersAtHome() && keeper.Team().opponentTeam.AllPlayersAtHome())
            {
                keeper.Ball().Kick((ballTarget - (Vector2)keeper.Ball().transform.position).normalized, Prm.instance.MaxPassingForce);
                SoccerPitch.instance.SetGoalKeeperHasBall(false);
                GameObject ballTargetTr = new GameObject();
                ballTargetTr.transform.position = ballTarget;
                MessageDispatcher_CH4.instance.DispatchMessage(0f, keeper.Id(), receiver.Id(), SoccerMessages.Msg_ReceiveBall, ballTargetTr.transform);
                keeper.GetFSM().ChangeState(TendGoal.instance);
                return;
            }
        }
        keeper.SetVelocity(new Vector2(0, 0));
    }

    public override void Exit(GoalKeeper keeper)
    {
      
    }
}
