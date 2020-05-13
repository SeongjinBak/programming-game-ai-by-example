using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldPlayer : PlayerBase
{
    [SerializeField]
    // Instance of FSM
    StateMachine<FieldPlayer> m_pStateMachine;
    [Header("CurState")]
    [SerializeField]
    private string state;

    // Start is called before the first frame update
    void Start()
    {
        // Set the FSM
        m_pStateMachine = new StateMachine<FieldPlayer>(this);
         m_pStateMachine.SetCurrentState(ReturnToHomeRegion.instance);
        m_pStateMachine.SetGlobalState(GlobalPlayerState.instance);


        EntityManager_CH4.instance.RegisterEntity(this);


        StartCoroutine(Updating());
    }

    public void SetTarget(Vector2 newPos)
    {
        transform.position = newPos;
    }

    public new bool HandleMessage(Telegram_CH4 msg)
    {
        return m_pStateMachine.HandleMessage(msg);
    }

    public void CurStateForDebug()
    {
        if (m_pStateMachine.IsInstate(Wait.instance))
        {
            state = "Wait";
        }
        else if (m_pStateMachine.IsInstate(ReturnToHomeRegion.instance))
        {
            state = "ReturnToHomeRegion";
        }
        else if (m_pStateMachine.IsInstate(Dribble.instance))
        {
            state = "Dribble";
        }
        else if (m_pStateMachine.IsInstate(KickBall.instance))
        {
            state = "KickBall";
        }
        else if (m_pStateMachine.IsInstate(ReceiveBall.instance))
        {
            state = "ReceiveBall";
        }
        else if (m_pStateMachine.IsInstate(SupportAttacker.instance))
        {
            state = "SupportAttacker";
        }
        else if (m_pStateMachine.IsInstate(ChaseBall.instance))
        {
            state = "ChaseBall";
        }
        else if (m_pStateMachine.IsInstate(GlobalPlayerState.instance))
        {
            state = "GlobalPlayerState";
        }
        else
        {
            state = "Nothing";
        }
    }
    public IEnumerator SteeringUpdate()
    {
        while (true)
        {
            // mass를 15로 지정함.
            Vector2 desiredVelocity = Steering().Calculate() / 70f;
            transform.position = (Vector2)transform.position + desiredVelocity;
            yield return new WaitForSeconds(0.05f);
        }
        
    }
    public IEnumerator Updating()
    {
        StartCoroutine(SteeringUpdate());
        while (true)
        {
            
            CurStateForDebug();
            m_pStateMachine.Updating();
            yield return new WaitForSeconds(0.2f);
        }
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
