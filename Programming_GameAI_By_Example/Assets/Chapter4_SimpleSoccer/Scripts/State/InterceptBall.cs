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
        if(keeper.TooFarFromGoalMouth() && !keeper.IsClosestPlayerOnPitchToBall())
        {
            keeper.GetFSM().ChangeState(ReturnHome.instance);
            return;
        }

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
