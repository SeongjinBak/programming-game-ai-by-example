using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalKeeper : FieldPlayer
{
    [SerializeField]
    // Instance of FSM
    StateMachine<GoalKeeper> m_pStateMachine;
   

    // Start is called before the first frame update
    void Start()
    {
        // Set the FSM
        m_pStateMachine = new StateMachine<GoalKeeper>(this);
        m_pStateMachine.SetCurrentState(ReturnHome.instance);
        m_pStateMachine.SetGlobalState(GlobalKeeperState.instance);


        EntityManager_CH4.instance.RegisterEntity(this);


        StartCoroutine(Updating());
    }

    public Vector2 GetRearInterposeTarget()
    {
        float x = Team().HomeGoal().Center().x;
        float y = 0f - 1 * .5f + (Ball().transform.position.y * 10f) / 20f;
       // Debug.Log("Rear Interpose Taget : " + x + ", " + y);
        return new Vector2(x, y);
    }

    public new void SetTarget(Vector2 newPos)
    {
        Debug.Log(" Steering으로 new POs로 가게끔 코딩 해줘야 함." + newPos);
        transform.position = newPos;

    }

    public new bool HandleMessage(Telegram_CH4 msg)
    {
        return m_pStateMachine.HandleMessage(msg);
    }

   
    public new IEnumerator SteeringUpdate()
    {
        while (true)
        {
            // mass를 15함.
            Vector2 desiredVelocity = Steering().Calculate() / 70f;
            transform.position = (Vector2)transform.position + desiredVelocity;
            yield return new WaitForSeconds(0.05f);
        }

    }
    public new IEnumerator Updating()
    {
        StartCoroutine(SteeringUpdate());
        while (true)
        {

           // CurStateForDebug();
            m_pStateMachine.Updating();
            yield return new WaitForSeconds(0.2f);
        }
    }

    public new bool BallWithKickingRange()
    {
        return Vector2.Distance(Ball().transform.position, transform.position) < Prm.instance.PlayerKickingDistance;
    }

    public new  bool BallWithinReceivingRange()
    {

        return Vector2.Distance(Ball().transform.position, transform.position) < Prm.instance.BallWithinReceivingRange;
    }
    public bool BallWithinKeeperRange()
    {
        return Vector2.Distance(transform.position, Ball().transform.position) < Prm.instance.KeeperInBallRange;
    }
    public bool BallWithinRangeForIntercept()
    {
        return (Vector2.Distance(Team().HomeGoal().Center(), Ball().transform.position) <= Prm.instance.GoalKeeperInterceptRange);
    }
    public bool TooFarFromGoalMouth()
    {
        return Vector2.Distance(transform.position, GetRearInterposeTarget()) > Prm.instance.GoalKeeperInterceptRange;
    }

    public new StateMachine<GoalKeeper> GetFSM()
    {
        return m_pStateMachine;
    }
}
