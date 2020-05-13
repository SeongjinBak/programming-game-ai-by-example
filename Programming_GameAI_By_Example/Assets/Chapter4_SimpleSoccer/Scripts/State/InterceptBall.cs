/*
 * Intercept ball state
 */ 

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InterceptBall : State<GoalKeeper>
{
    public static InterceptBall instance = null;
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
        keeper.Steering().PursuitOn();
    }

    public override void Execute(GoalKeeper keeper)
    {
        // 골대에서 너무 멀리 나온경우
        if(keeper.TooFarFromGoalMouth() && !keeper.IsClosestPlayerOnPitchToBall())
        {
            keeper.GetFSM().ChangeState(ReturnHome.instance);
            return;
        }

        // 골키퍼가 공을 막은 경우
        if (keeper.BallWithinKeeperRange())
        {
            keeper.Ball().Trap(gameObject);
            SoccerPitch.instance.SetGoalKeeperHasBall(true);
            keeper.GetFSM().ChangeState(PutBallBackInPlay.instance);
            return;
        }

    }

    public override void Exit(GoalKeeper keeper)
    {
        keeper.Steering().PursuitOff();
    }
}
