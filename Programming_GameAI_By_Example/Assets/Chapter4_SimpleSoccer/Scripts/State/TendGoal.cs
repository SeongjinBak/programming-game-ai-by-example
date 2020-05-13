/*
 * 골키퍼로 하여금 골을 막도록 한다.
 */ 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TendGoal : State<GoalKeeper>
{
    public static TendGoal instance = null;
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
        keeper.Steering().InterposeOn( Prm.instance.GoalKeeperTendingDistance);
        keeper.Steering().SetTarget(keeper.GetRearInterposeTarget());
    }

    public override void Execute(GoalKeeper keeper)
    {
        keeper.Steering().SetTarget(keeper.GetRearInterposeTarget());

        if (keeper.BallWithinKeeperRange())
        {
            keeper.Ball().Trap(keeper.gameObject);
            SoccerPitch.instance.SetGoalKeeperHasBall(true);
            keeper.GetFSM().ChangeState(PutBallBackInPlay.instance);
            return;
        }

        if (keeper.BallWithinRangeForIntercept() && !keeper.Team().InControl())
        {
            keeper.GetFSM().ChangeState(InterceptBall.instance);
        }

        if(keeper.TooFarFromGoalMouth() && keeper.Team().InControl())
        {
            keeper.GetFSM().ChangeState(ReturnHome.instance);
            return;
        }
    }

    public override void Exit(GoalKeeper keeper)
    {
        keeper.Steering().InterposeOff();
    }
}
