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

    public TeamColor teamColor;

    private void Awake()
    {
        
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public GameObject ControllingPlayer()
    {
        return controllingPlayer;
    }
    public GameObject SupportingPlayer()
    {
        return supportingPlayer;
    }

    public bool CanShoot(Vector2 ballPos, float power, Vector2 shotTarget)
    {
        int numAttempts = Prm.instance.NumAttemptsToFindValidStrike;

        Goal opponentsGoal;
        if (teamColor == TeamColor.Blue)
        {
            opponentsGoal = GameObject.Find("Reds").GetComponent<GameObject>().transform.GetChild(2).GetComponent<Goal>();
        }
        else
        {
            opponentsGoal = GameObject.Find("Blues").GetComponent<GameObject>().transform.GetChild(2).GetComponent<Goal>();
        }
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
                if(IsPassSageFromAllOpponents(ballPos, shotTarget, null, power))
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


    public bool IsPassSageFromAllOpponents(Vector2 from, Vector2 target, GameObject receiver, float passingForce)
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

}
