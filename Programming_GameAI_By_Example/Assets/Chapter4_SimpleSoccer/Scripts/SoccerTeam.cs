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
    // Instance of FSM
    private StateMachine<SoccerTeam> m_pStateMachine;
    
    
    public TeamColor teamColor;
    public SoccerTeam opponentTeam;
    public Goal opponentsGoal;
    public Goal homeGoal;
    public Vector2[] initialRegion = new Vector2[5];
    public List<GameObject> players;
    private void Awake()
    {
        //players = new List<GameObject>();
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



        //ChangeState(EnterMineAndDigForNugget.instance);
        StartCoroutine(Updating());
    }

    
    public IEnumerator Updating()
    {
        yield return null;
        m_pStateMachine.Updating();
        
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
                Debug.Log("Go Home 메시지 코드 추가했으나 의문 : sender 1이고, Msg 핸들러가 player에는 없다 아직까진.");
                MessageDispatcher_CH4.instance.DispatchMessage(0f, 1, pb.ID(), SoccerMessages.Msg_GoHome, null);
            }
        }
    }

    public void RequestPass(FieldPlayer requester)
    {
        float randFloat = Random.Range(0f, 1f);
        if (randFloat > .1f) return;
        if (IsPassSafeFromAllOpponents(ControllingPlayer().transform.position, requester.transform.position, requester.gameObject, Prm.instance.MaxPassingForce))
        {
            Debug.Log("패스요청 메시지 처리하는 기능 구현완료. 테스트 필요하다..");
            MessageDispatcher_CH4.instance.DispatchMessage(0f, ControllingPlayer().GetComponent<FieldPlayer>().ID(), requester.ID(), SoccerMessages.Msg_PassToMe);
        }
    }

    public PlayerBase DetermineBestSupportingAttacker()
    {
        float closestSoFar = 987654321f;

        PlayerBase bestPlayer = null;

        foreach(var item in players)
        {
            if(item.GetComponent<PlayerBase>().Role() == "Attacker" && item.GetComponent<FieldPlayer>() != ControllingPlayer())
            {
                Debug.Log("bestSupportSpot 은 맨 처음 0,0 으로 설정되어 있음.. 확인 필요하다.: "+ SupportSpotCalculator.instance.bestSupportSpot);
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
                    
                    fp.gameObject.GetComponent<PlayerBase>().Steering().SetTarget(fp.HomeRegion());
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
        Vector2 target = Vector2.zero;

        foreach(var item in players)
        {
            if((item.GetComponent<PlayerBase>() != passer) && Vector2.Distance(passer.transform.position, item.transform.position) > minPassingDistance)
            {
                if (GetBestPassToReceiver(passer, item.GetComponent<PlayerBase>(), ref target, power)){
                    float dist2Goal = Mathf.Abs(target.x - opponentsGoal.Center().x);

                    if(dist2Goal < closestToGoalSoFar)
                    {
                        closestToGoalSoFar = dist2Goal;
                        receiver = item.GetComponent<PlayerBase>();
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
        ip1 = new Vector2();
        ip2 = new Vector2();
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

    public Goal HomeGoal() { return homeGoal; }

    public bool CanShoot(Vector2 ballPos, float power, ref Vector2 shotTarget)
    {
        int numAttempts = Prm.instance.NumAttemptsToFindValidStrike;

        while (numAttempts-- > 0)
        {
            shotTarget = opponentsGoal.Center();

            int minYVal = (int)(opponentsGoal.RightPost().y + .15f);
            int maxYVal = (int)(opponentsGoal.LeftPost().y - .15f);
            Debug.Log("책과 다르게 한 부분. ");
            shotTarget.y = (float)Random.Range(minYVal, maxYVal);

            float time = GameObject.Find("Ball").GetComponent<SoccerBall>().TimeToCoverDistance(ballPos, shotTarget, power);

            if(time >= 0f)
            {
                if(IsPassSafeFromAllOpponents(ballPos, shotTarget, null, power))
                {
                    return true;
                }
            }
        }
        return false;
    }


    public bool IsPassSafeFromOpponent(Vector2 from, Vector2 target, GameObject receiver, GameObject opp, float passingForce)
    {
        Vector2 toTarget = target - from;
        Vector2 toTargetNormalized = toTarget.normalized;
        Vector2 localPosOpp = receiver.transform.InverseTransformVector(opp.transform.position);
        Debug.Log(localPosOpp);
   

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
                else return false;
            }
        }
        else
        {
            return true;
        }

        SoccerBall ball = GameObject.Find("Ball").GetComponent<SoccerBall>();
        float timeForBall =
            ball.TimeToCoverDistance(new Vector2(0, 0), new Vector2(localPosOpp.x, 0), passingForce);
        Debug.Log("상대 플레이어의 MAX SPEED는 " + opp.GetComponent<PlayerBase>().myMaxSpeed + " 로 설정됨.");
        float reach = opp.GetComponent<PlayerBase>().myMaxSpeed * timeForBall + .15f + .3f;

        if (Mathf.Abs(localPosOpp.y) < reach)
        {
            return false;
        }
        else return true;
    }


    public bool IsPassSafeFromAllOpponents(Vector2 from, Vector2 target, GameObject receiver, float passingForce)
    {
        string color = "";
        if (TeamColor.Red == teamColor)
        {
            color = "red";
        }
        else color = "brue";

        var group = GameObject.Find(color).GetComponentsInChildren<GameObject>();
        List<GameObject> opponents = new List<GameObject>(group);

        foreach(var item in opponents)
        {
            if (IsPassSafeFromOpponent(from, target, receiver, item, passingForce))
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
