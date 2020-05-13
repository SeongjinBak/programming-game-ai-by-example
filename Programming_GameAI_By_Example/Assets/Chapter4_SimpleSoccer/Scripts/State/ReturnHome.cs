/*
 * 골키퍼의 골대 본래 위치로 가는 상태
 */ 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReturnHome : State<GoalKeeper>
{
    public static ReturnHome instance = null;
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
        keeper.Steering().ArriveOn();
    }

    public override void Execute(GoalKeeper keeper)
    {
        keeper.Steering().SetTarget(keeper.HomeRegion());

        if(keeper.InHomeRegion() || !keeper.Team().InControl())
        {
            keeper.GetFSM().ChangeState(TendGoal.instance);
        }

    }

    public override void Exit(GoalKeeper keeper)
    {
        keeper.Steering().ArriveOff();
    }
}
