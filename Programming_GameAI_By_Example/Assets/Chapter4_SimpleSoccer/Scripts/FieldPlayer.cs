using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldPlayer : PlayerBase
{
    [SerializeField]
    // Instance of FSM
    StateMachine<FieldPlayer> m_pStateMachine;

    // Start is called before the first frame update
    void Start()
    {
        // Set the FSM
        m_pStateMachine = new StateMachine<FieldPlayer>(this);
         m_pStateMachine.SetCurrentState(ReturnToHomeRegion.instance);
        //m_pStateMachine.SetCurrentState(EnterMineAndDigForNugget.instance);
        m_pStateMachine.SetGlobalState(GlobalPlayerState.instance);


        EntityManager_CH4.instance.RegisterEntity(this);
    }

    public void SetTarget(Vector2 newPos)
    {
        Debug.Log(" Steering으로 new POs로 가게끔 코딩 해줘야 함." + newPos);
        transform.position = newPos;

    }

    public new bool HandleMessage(Telegram_CH4 msg)
    {
        return m_pStateMachine.HandleMessage(msg);
    }


    // Update is called once per frame
    void Update()
    {
        
    }

    public bool BallWithKickingRange()
    {
        return Vector2.Distance(Ball().transform.position, transform.position) < Prm.instance.PlayerKickingDistance;
    }

    public bool BallWithinReceivingRange()
    {

        return Vector2.Distance(Ball().transform.position, transform.position) < Prm.instance.BallWithinReceivingRange;
    }



    public StateMachine<FieldPlayer> GetFSM()
    {
        return m_pStateMachine;
    }
}
