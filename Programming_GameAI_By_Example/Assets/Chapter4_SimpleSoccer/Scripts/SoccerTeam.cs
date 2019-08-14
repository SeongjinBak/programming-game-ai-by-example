using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoccerTeam : MonoBehaviour
{
    [SerializeField]
    private GameObject receivingPlayer;
    [SerializeField]
    private GameObject playerClosestToBall;
    [SerializeField]
    private GameObject controllingPlayer;
    [SerializeField]
    private GameObject supportingPlayer;
    [SerializeField]
    SoccerBall ball;
    [SerializeField]
    // Instance of FSM
    private StateMachine<SoccerTeam> m_pStateMachine;



    public TeamColor teamColor;
    public SoccerTeam opponentTeam;
    public Goal opponentsGoal;
    public Goal homeGoal;
    public Vector2[] initialRegion = new Vector2[5];
    public List<GameObject> players;
    [Header("Current_State")]
    [SerializeField]
    private string state;
    [SerializeField]
    private float distToBallOfClosestPlayer;
    private void Awake()
    {
        //players = new List<GameObject>();
    }

    public void CurStateForDebug()
    {
        if (m_pStateMachine.IsInstate(Defending.instance))
        {
            state = "Defending";   
        }
        else if (m_pStateMachine.IsInstate(Attacking.instance))
        {
            state = "Attacking";
        }
        else
        {
            state = "Nothing";
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        if (teamColor == TeamColor.Blue)
        {
            opponentsGoal = GameObject.Find("Reds").transform.GetChild(2).GetComponent<Goal>();
            homeGoal = GameObject.Find("Blues").transform.GetChild(2).GetComponent<Goal>();
            opponentTeam = GameObject.Find("RedTeam").GetComponent<SoccerTeam>();  
        }
        else
        {
            opponentsGoal = GameObject.Find("Blues").transform.GetChild(2).GetComponent<Goal>();
            homeGoal= GameObject.Find("Reds").transform.GetChild(2).GetComponent<Goal>();
            opponentTeam = GameObject.Find("BlueTeam").GetComponent<SoccerTeam>();
        }

        //players = new List<GameObject>();

        for (int i = 0; i < 5; i++)
        {
          //  players.Add(gameObject.transform.GetChild(i).gameObject);
            initialRegion[i] = gameObject.transform.GetChild(i).transform.position;
        }

        // Set the FSM
        m_pStateMachine = new StateMachine<SoccerTeam>(this);
        m_pStateMachine.SetCurrentState(PrepareForKickOff.instance);
        // m_pStateMachine.SetGlobalState(MinerGlobalState.instance);


        ball = GameObject.Find("Ball").GetComponent<SoccerBall>();
        //ChangeState(EnterMineAndDigForNugget.instance);
        StartCoroutine(Updating());
    }

    void CheckItIsOurTeam()
    {
        //  bool flag = false;
        if (controllingPlayer != null)
        {
            if (controllingPlayer != ball.GetOwner())
            {
                controllingPlayer = null;
            }

        }
        else
        {
            for (int i = 0; i < 5; i++)
            {
                if (ball.GetOwner() == players[i])
                {
                    controllingPlayer = players[i];
                }
            }
        }
    }
    
    public IEnumerator Updating()
    {
        while (true)
        {
            CalculateClosestPlayerToBall();
            yield return null;
            CurStateForDebug();
            m_pStateMachine.Updating();
            CheckItIsOurTeam();
        }
    }

    public float DistToBallOfClosestPlayer()
    {
        return distToBallOfClosestPlayer;
    }

    public void CalculateClosestPlayerToBall()
    {
        float closestSoFar = 987654321f;

        foreach(var item in players)
        {
            PlayerBase pb = item.GetComponent<PlayerBase>();
            float dist = Vector2.Distance(pb.transform.position, SoccerPitch.instance.ball.transform.position);
            item.GetComponent<PlayerBase>().SetDistToBall(dist);
            if(dist< closestSoFar)
            {
                closestSoFar = dist;
                playerClosestToBall = item;
            }
        }
        distToBallOfClosestPlayer = closestSoFar;
    }

    public bool IsOpponentWithInRadius(Vector2 pos, float rad)
    {
        foreach(var item in opponentTeam.players)
        {
            if (Vector2.Distance(pos, item.transform.position) < rad)
                return true;
        }
        return false;
    }

    public bool HandleMessage(Telegram_CH4 msg)
    {
        return m_pStateMachine.HandleMessage(msg);
    }

    public void ReturnAllFieldPlayersToHome()
    {
        foreach (var item in players)
        {
            PlayerBase pb = item.GetComponent<PlayerBase>();
            if (pb.Role() != "GoalKeeper")
            {
               
                if(teamColor == TeamColor.Red)
                    MessageDispatcher_CH4.instance.DispatchMessage(0f, 1, pb.ID(), SoccerMessages.Msg_GoHome, null);
                else
                    MessageDispatcher_CH4.instance.DispatchMessage(0f, 6, pb.ID(), SoccerMessages.Msg_GoHome, null);

            }
        }
    }

    public void RequestPass(FieldPlayer requester)
    {
        float randFloat = Random.Range(0f, 1f);
        if (randFloat > .5f) return;
        if (IsPassSafeFromAllOpponents(ControllingPlayer().transform.position, requester.transform.position, requester.gameObject, Prm.instance.MaxPassingForce))
        {
            Debug.Log("패스요청 메시지 처리하는 기능 구현완료. 테스트 필요하다..");
            MessageDispatcher_CH4.instance.DispatchMessage(0f, ControllingPlayer().GetComponent<FieldPlayer>().ID(), requester.ID(), SoccerMessages.Msg_PassToMe, requester.transform);
        }
    }

    public PlayerBase DetermineBestSupportingAttacker()
    {
        float closestSoFar = 987654321f;

        PlayerBase bestPlayer = null;

        foreach(var item in players)
        {
            if(item.GetComponent<PlayerBase>().Role() == "Attacker" && item != ControllingPlayer())
            {
                //Debug.Log("bestSupportSpot 은 맨 처음 0,0 으로 설정되어 있음.. 확인 필요하다.: "+ SupportSpotCalculator.instance.bestSupportSpot);
                float dist = Vector2.Distance(item.transform.position, SupportSpotCalculator.instance.bestSupportSpot);
                if(dist < closestSoFar)
                {
                    closestSoFar = dist;
                    bestPlayer = item.GetComponent<FieldPlayer>();
                }
            }
        }
        return bestPlayer;
    }

    public bool InControl()
    {
        if(controllingPlayer != null)
        {
            return true;
        }
        return false;
    }

    public void SetPlayerHomeRegion(int plyr, Vector2 newRegion)
    {
        players[plyr].GetComponent<PlayerBase>().SetHomeRegion(newRegion);
    }
    
 
    public PlayerBase PlayerClosestToBall()
    {
        return playerClosestToBall.GetComponent<PlayerBase>();
    }

    public void UpdateTargetsOfWaitingPlayers()
    {
        
        foreach (var item in players)
        {
            PlayerBase pb = item.GetComponent<PlayerBase>();
            if (pb.Role() != "GoalKeeper")
            {
                FieldPlayer fp = item.GetComponent<FieldPlayer>();
                if(fp.GetFSM() == null)
                {
                    Debug.Log("No FSM");
                }
                if (fp.GetFSM().IsInstate(Wait.instance) || fp.GetFSM().IsInstate(ReturnToHomeRegion.instance))
                {
                    //Debug.Log("원래는 fp.steering().settarget(~~)\n https://github.com/wangchen/Programming-Game-AI-by-Example-src/blob/master/Buckland_Chapter4-SimpleSoccer/SoccerTeam.cpp");

                    if (fp.Team().teamColor == TeamColor.Blue)
                        fp.Steering().SetTarget((fp.Team().initialRegion[fp.ID() - 6]));
                    else
                        fp.Steering().SetTarget((fp.Team().initialRegion[fp.ID() - 1]));
                //    fp.gameObject.GetComponent<PlayerBase>().Steering().SetTarget(fp.HomeRegion());
                }

            }
        }
    }

    public bool AllPlayersAtHome()
    {
        for(int i = 0; i < 5; i++)
        {
            if(initialRegion[i] != (Vector2)gameObject.transform.GetChild(i).transform.position)
            {
                return false;
            }
        }
        return true;
    }


    public GameObject ControllingPlayer()
    {
        return controllingPlayer;
    }
    public GameObject SupportingPlayer()
    {
        return supportingPlayer;
    }

    public void SetControllingPlayer(GameObject player)
    {
        controllingPlayer = player;
    }

    public void SetSuppprtingPlayer(GameObject player)
    {
        supportingPlayer = player;
    }

    public void SetReceiver(GameObject player)
    {
        receivingPlayer = player;
    }

    public GameObject Receiver()
    {
        return receivingPlayer;
    }
    public void SetPlayerClosestToBall(GameObject player)
    {
        playerClosestToBall = player;
    }

    public bool FindPass(PlayerBase passer, PlayerBase receiver, ref Vector2 passTarget, float power, float minPassingDistance)
    {
        float closestToGoalSoFar = 987654321f;
        Vector2 target = new Vector2(0f,0f);

        foreach(var item in players)
        {
            if((item.GetComponent<PlayerBase>() != passer) && Vector2.Distance(passer.transform.position, item.transform.position) > minPassingDistance)
            {
                Debug.Log(receiver + "  확인중");
                if (GetBestPassToReceiver(passer, item.GetComponent<PlayerBase>(), ref target, power)){
                    float dist2Goal = Mathf.Abs(target.x - opponentsGoal.Center().x);
                    
                    if (dist2Goal < closestToGoalSoFar)
                    {
                        closestToGoalSoFar = dist2Goal;
                        receiver = item.GetComponent<PlayerBase>();
                        Debug.Log(receiver + "  확인됨");
                        passTarget = target;
                    }
                }
            }
        }
        if (receiver) return true;
        else return false;
    }

    public bool GetTangentPoints(Vector2 c, float r, Vector2 p, ref Vector2 t1, ref Vector2 t2)
    {
        Vector2 PmC = p - c;
        float sqrLen = PmC.SqrMagnitude();
        float rSqr = r * r;

        if (sqrLen <= rSqr)
            return false;

        float invSqrLen = 1 / sqrLen;
        float root = Mathf.Sqrt(Mathf.Abs(sqrLen - rSqr));
        
        t1.x = c.x + r * (r * PmC.x - PmC.y * root) * invSqrLen;
        t1.y = c.y + r * (r * PmC.y - PmC.y * root) * invSqrLen;
        t2.x = c.x + r * (r * PmC.x - PmC.y * root) * invSqrLen;
        t2.y = c.y + r * (r * PmC.y - PmC.y * root) * invSqrLen;

        return true;
    }

    public bool GetBestPassToReceiver(PlayerBase passer, PlayerBase receiver, ref Vector2 passTarget, float power)
    {
        float time = SoccerPitch.instance.ball.TimeToCoverDistance(SoccerPitch.instance.ball.transform.position, receiver.transform.position, power);

        if (time < 0) return false;

        float interceptRange = time * receiver.myMaxSpeed;

        float scalingFactor = .3f;
        interceptRange *= scalingFactor;

        Vector2 ip1, ip2;
        ip1 = new Vector2(0, 0);
        ip2 = new Vector2(0,0);
        GetTangentPoints(receiver.transform.position, interceptRange, SoccerPitch.instance.ball.transform.position, ref ip1, ref ip2);

        const int numPassesToTry = 3;
        Vector2[] passes = new Vector2[numPassesToTry] { ip1, receiver.transform.position, ip2 };

        // this pass is the best found so far if it is:
        //
        //  1. Further upfield than the closest valid pass for this receiver
        //     found so far
        //  2. Within the playing area
        //  3. Cannot be intercepted by any opponents

        float closestSoFar = 987654321f;
        bool result = false;
        for(int pass = 0; pass < numPassesToTry; ++pass)
        {
            float dist = Mathf.Abs(passes[pass].x - opponentsGoal.Center().x);
            if (dist < closestSoFar &&
                IsPassSafeFromAllOpponents(SoccerPitch.instance.ball.transform.position, passes[pass], receiver.transform.gameObject, power)
                && SoccerPitch.instance.Inside(passes[pass]))
            {
                closestSoFar = dist;
                passTarget = passes[pass];
                result = true;
            }
        }

        return result;
    }

    public Vector2 GetSupportSpot(TeamColor team) { return SupportSpotCalculator.instance.GetBestSupportingSpot(team); }
    public Goal HomeGoal() { return homeGoal; }

    public bool CanShoot(Vector2 ballPos, float power, ref Vector2 shotTarget)
    {
        int numAttempts = Prm.instance.NumAttemptsToFindValidStrike;

        while (numAttempts-- > 0)
        {
            shotTarget = opponentsGoal.Center();
            
            int minYVal = (int)(opponentsGoal.RightPost().y + .15f);
            int maxYVal = (int)(opponentsGoal.LeftPost().y - .15f);
           // Debug.Log("책과 다르게 한 부분. ");
            shotTarget.y = (float)Random.Range(minYVal, maxYVal);
            /*
            int minYVal = (int)(opponentsGoal.RightPost().x + .15f);
            int maxYVal = (int)(opponentsGoal.LeftPost().x - .15f);
            //Debug.Log("책과 다르게 한 부분. ");
            shotTarget.x = (float)Random.Range(minYVal, maxYVal);
            */
            float time = ball.TimeToCoverDistance(ballPos, shotTarget, power);

            if(time >= 0f)
            {
               
                if(IsPassSafeFromAllOpponents(ballPos, shotTarget, null, power))
                {
                    return true;
                }
            }
            if ((ballPos - shotTarget).magnitude < 20f)
            {
                //Debug.Log("COUD: SHOOT");
                return false;

            }

        }
        return false;
    }


    public bool IsPassSafeFromOpponent(Vector2 from, Vector2 target, GameObject receiver, GameObject opp, float passingForce)
    {
        Vector2 toTarget = target - from;
        Vector2 toTargetNormalized = toTarget.normalized;

        Vector2 localPosOpp = new Vector2(0, 0);
        if (receiver)
        {
            localPosOpp.x = receiver.transform.position.x;
            localPosOpp.y = receiver.transform.position.y;

         //   localPosOpp.x = receiver.transform.InverseTransformVector(opp.transform.position).x;
          // localPosOpp.y = receiver.transform.InverseTransformVector(opp.transform.position).y;
        }



        if (localPosOpp.x < 0)
        {
            return true;
        }

        if(Vector2.Distance(from,target) < Vector2.Distance(opp.transform.position, from))
        {
            if (receiver)
            {
                if (Vector2.Distance(target, opp.transform.position) > Vector2.Distance(target, receiver.transform.position))
                {
                    return true;
                }
                else
                    return false;
            }
            else
            {
                return true;
            }
        }
        
      
        //SoccerBall ball = GameObject.Find("Ball").GetComponent<SoccerBall>();
        float timeForBall =
            ball.TimeToCoverDistance(new Vector2(0, 0), target, passingForce);
        


        float reach = opp.GetComponent<PlayerBase>().myMaxSpeed / Random.Range(4.9f, 5.2f)  * timeForBall + .4f + .3f;
        float expectedBall = timeForBall * Prm.instance.MaxShootingForce + .4f + .3f;
        //Debug.Log("Time for ball " + expectedBall + " Reach : " + reach + " BALLTUNE : " + timeForBall);
        if (expectedBall < reach)
        {
            
            return false;
        }
        else
            return true;
    }


    public bool IsPassSafeFromAllOpponents(Vector2 from, Vector2 target, GameObject receiver, float passingForce)
    {
        string color = "";
        if (TeamColor.Red == teamColor)
        {
            color = "BlueTeam";
        }
        else color = "RedTeam";

        var group = GameObject.Find(color).GetComponentsInChildren<PlayerBase>();
        List<PlayerBase> opponents = new List<PlayerBase>(group);
        opponents.RemoveAt(0);
        foreach(var item in opponents)
        {
            if (!IsPassSafeFromOpponent(from, target, receiver, item.gameObject, passingForce))
            {
                return false;
            }
        }

        return true;
    }

    public StateMachine<SoccerTeam> GetFSM()
    {
        return m_pStateMachine;
    }
}
