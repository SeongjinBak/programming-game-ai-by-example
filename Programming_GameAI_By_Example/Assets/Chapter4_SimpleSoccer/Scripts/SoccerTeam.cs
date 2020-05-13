/*
 * 각 팀을 관리하는 클래스
 */
using System;
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

    // 디버그 용더의 현재 상태 출력 함수
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

    
    // 각 팀의 초기설정 지정
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

        for (int i = 0; i < 5; i++)
        {
            initialRegion[i] = gameObject.transform.GetChild(i).transform.position;
        }

        // Set the FSM
        m_pStateMachine = new StateMachine<SoccerTeam>(this);
        m_pStateMachine.SetCurrentState(PrepareForKickOff.instance);

        ball = GameObject.Find("Ball").GetComponent<SoccerBall>();
        StartCoroutine(UpdateTeamState());
    }

    // 현재 공을 소유하고 관리하는 선수가 팀에 속해있는지 확인하는 함수
    void CheckItIsOurTeam()
    {
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
    
    // 팀의 상태를 최신화 하는 코루틴
    public IEnumerator UpdateTeamState()
    {
        while (true)
        {
            CalculateClosestPlayerToBall();
            yield return new WaitForSeconds(0.1f);
            CurStateForDebug();
            m_pStateMachine.Updating();
            CheckItIsOurTeam();
        }
    }

    // 공 가장 가까운 플레이어와의 거리 계산
    public float DistToBallOfClosestPlayer()
    {
        return distToBallOfClosestPlayer;
    }

    // 공과 가장 가까운 플레이어 반환
    public void CalculateClosestPlayerToBall()
    {
        float closestSoFar = Mathf.Infinity;

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

    // 플레이어의 반경 안에 상대편이 있을 경우
    public bool IsOpponentWithInRadius(Vector2 pos, float rad)
    {
        foreach(var item in opponentTeam.players)
        {
            if (Vector2.Distance(pos, item.transform.position) < rad)
                return true;
        }
        return false;
    }

    // 텔레그램 처리
    public bool HandleMessage(Telegram_CH4 msg)
    {
        return m_pStateMachine.HandleMessage(msg);
    }

    // 모든 필드 플레이어를 본래의 위치로 돌아가라고 명령을 내림.
    public void ReturnAllFieldPlayersToHome()
    {
        foreach (var item in players)
        {
            PlayerBase pb = item.GetComponent<PlayerBase>();
            if (pb.Role() != "GoalKeeper")
            {
               
                if(teamColor == TeamColor.Red)
                    MessageDispatcher_CH4.instance.DispatchMessage(0f, 1, pb.Id(), SoccerMessages.Msg_GoHome, null);
                else
                    MessageDispatcher_CH4.instance.DispatchMessage(0f, 6, pb.Id(), SoccerMessages.Msg_GoHome, null);

            }
        }
    }

    // 패스 요청
    public void RequestPass(FieldPlayer requester)
    {
        // 패스 요청 빈도
        const float requestFrequency = 0.7f;
        float randFloat = UnityEngine.Random.Range(0f, 1f);
        
        if (randFloat > requestFrequency) return;
        
        // 가로챔 당할 걱정 없을 경우 패스
        if (IsPassSafeFromAllOpponents(ControllingPlayer().transform.position, requester.transform.position, requester.gameObject, Prm.instance.MaxPassingForce))
        {
            MessageDispatcher_CH4.instance.DispatchMessage(0f, ControllingPlayer().GetComponent<FieldPlayer>().Id(), requester.Id(), SoccerMessages.Msg_PassToMe, requester.transform);
        }
    }

    // 최적의 BSS 찾는다.
    public PlayerBase DetermineBestSupportingAttacker()
    {
        float closestSoFar = Mathf.Infinity;

        PlayerBase bestPlayer = null;

        foreach(var item in players)
        {
            if(item.GetComponent<PlayerBase>().Role() == "Attacker" && item != ControllingPlayer())
            {
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

    // 컨트롤 중인 플레이어가 있는지 확인
    public bool InControl()
    {
        if(controllingPlayer != null)
        {
            return true;
        }
        return false;
    }

    // 플레이어의 복귀 위치 지정
    public void SetPlayerHomeRegion(int plyr, Vector2 newRegion)
    {
        players[plyr].GetComponent<PlayerBase>().SetHomeRegion(newRegion);
    }
    
    // 공과 가장 가까운 플레이어 반환
    public PlayerBase PlayerClosestToBall()
    {
        return playerClosestToBall.GetComponent<PlayerBase>();
    }

    // 대기중인 플레이어의 목표 지점을 최신화
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
                    Debug.LogError("No FSM");
                }

                // 본래의 위치로 돌아가게 한다.
                if (fp.GetFSM().IsInstate(Wait.instance) || fp.GetFSM().IsInstate(ReturnToHomeRegion.instance))
                {
                    if (fp.Team().teamColor == TeamColor.Blue)
                        fp.Steering().SetTarget((fp.Team().initialRegion[fp.Id() - 6]));
                    else
                        fp.Steering().SetTarget((fp.Team().initialRegion[fp.Id() - 1]));
                }

            }
        }
    }

    // 모든 플레이어가 본래 위치에 있는지 확인
    public bool AllPlayersAtHome()
    {
        for(int i = 1; i < 5; i++)
        {
            Vector2 playerPos = gameObject.transform.GetChild(i).transform.position;

            if (Vector2.Distance(initialRegion[i], playerPos) > Mathf.Epsilon + 1f)
            {
                return false;
            }
        }
        return true;
    }

    // 컨트롤 중인 플레이어 반환
    public GameObject ControllingPlayer()
    {
        return controllingPlayer;
    }
    
    // 지원중인 선수 반환
    public GameObject SupportingPlayer()
    {
        return supportingPlayer;
    }

    // 컨트롤 선수 지정
    public void SetControllingPlayer(GameObject player)
    {
        controllingPlayer = player;
    }
    
    // 지원 선수 지정
    public void SetSuppprtingPlayer(GameObject player)
    {
        supportingPlayer = player;
    }

    // 리시버 지정
    public void SetReceiver(GameObject player)
    {
        receivingPlayer = player;
    }
    
    // 리시버 반환
    public GameObject Receiver()
    {
        return receivingPlayer;
    }

    // 공과 가장 가까운 선수 지정
    public void SetPlayerClosestToBall(GameObject player)
    {
        playerClosestToBall = player;
    }

    public bool FindPass(PlayerBase passer, PlayerBase receiver, ref Vector2 passTarget, float power, float minPassingDistance)
    {
        float closestToGoalSoFar = Mathf.Infinity;
        Vector2 target = new Vector2(0f,0f);

        foreach(var item in players)
        {
            if((item.GetComponent<PlayerBase>() != passer) && Vector2.Distance(passer.transform.position, item.transform.position) > minPassingDistance)
            {

                if (GetBestPassToReceiver(passer, item.GetComponent<PlayerBase>(), ref target, power)){
                    float dist2Goal = Mathf.Abs(target.x - opponentsGoal.Center().x);
                    
                    if (dist2Goal < closestToGoalSoFar)
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

    // 패스 받기에 가장 최적인 경로 얻는다.
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
        //  1. Further upfield than the closest valid pass for this receiver found so far
        //  2. Within the playing area
        //  3. Cannot be intercepted by any opponents

        float closestSoFar = Mathf.Infinity;

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

    // 슛을 할 수 있는지 반환하는 함수
    public bool CanShoot(Vector2 ballPos, float power, ref Vector2 shotTarget)
    {
        int numAttempts = Prm.instance.NumAttemptsToFindValidStrike;

        while (numAttempts-- > 0)
        {
            shotTarget = opponentsGoal.Center();
            
            int minYVal = (int)(opponentsGoal.RightPost().y + .5f);
            int maxYVal = (int)(opponentsGoal.LeftPost().y - .5f);

            shotTarget.y = (float)UnityEngine.Random.Range(minYVal, maxYVal);
           
            // 목적지 까지 도달할 시간 예측
            float time = ball.TimeToCoverDistance(ballPos, shotTarget, power);

            // 도달 할 수 있다면
            if(time >= 0f)
            {   
                if(IsPassSafeFromAllOpponents(ballPos, shotTarget, null, power))
                {
                    return true;
                }
            }

            // 거리가 너무 멀 경우
            if ((ballPos - shotTarget).magnitude < 20f)
            {
                return false;
            }
        }
        return false;
    }

    // 해당 적으로 부터 패스 경로가 안전한지 확인
    public bool IsPassSafeFromOpponent(Vector2 from, Vector2 target, GameObject receiver, GameObject opp, float passingForce)
    {
        Vector2 toTarget = target - from;
        Vector2 toTargetNormalized = toTarget.normalized;

        Vector2 localPosOpp = new Vector2(0, 0);

        if (receiver)
        {
            localPosOpp.x = receiver.transform.position.x;
            localPosOpp.y = receiver.transform.position.y;
        }

        if (localPosOpp.x < 0)
        {
            return true;
        }

        if (Vector2.Distance(from, target) < Vector2.Distance(opp.transform.position, from))
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

        // 패스 도달시간 예측
        float timeForBall = ball.TimeToCoverDistance(new Vector2(0, 0), target, passingForce);
        float reach = opp.GetComponent<PlayerBase>().myMaxSpeed / UnityEngine.Random.Range(4.9f, 5.2f) * timeForBall + 5f;
        float expectedBall = timeForBall * Prm.instance.MaxShootingForce + .7f;

        if (expectedBall < reach)
        {
            return false;
        }
        else
            return true;
    }


    // 모든 적으로 부터 패스 경로가 안전한지 확인
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
