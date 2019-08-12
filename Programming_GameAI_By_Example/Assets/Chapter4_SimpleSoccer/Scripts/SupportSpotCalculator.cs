using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SupportSpotCalculator : MonoBehaviour
{
    

    public static SupportSpotCalculator instance = null;
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

    public List<SupportSpot> blueZone;
    public List<SupportSpot> redZone;
    public Vector2 bestSupportSpot;
    public int frame;
    private void Start()
    {
        var group = GameObject.Find("BlueZone").GetComponentsInChildren<SupportSpot>();
        if(group != null)
        {
            blueZone = new List<SupportSpot>(group);
            blueZone.RemoveAt(0);
        }
        group = GameObject.Find("RedZone").GetComponentsInChildren<SupportSpot>();
        redZone = new List<SupportSpot>(group);
        redZone.RemoveAt(0);

        frame = 0;
        bestSupportSpot = new Vector2(0, 0);

     
    }

    private void Update()
    {
        frame++;
        if (frame > 60)
        {
            frame = 0;
            
        }
    }

    public Vector2 DetermineBestSupportingPosition(TeamColor team)
    {

        bestSupportSpot = new Vector2(0, 0);

        float bestScoreSoFar = 0f;

        List<SupportSpot> list = team == TeamColor.Red ? redZone : blueZone;
        GameObject teamName = team == TeamColor.Red ? GameObject.Find("RedTeam").gameObject : GameObject.Find("BlueTeam").gameObject;
      
        Debug.Log("Prm 패싱, 스코어링 가중치 조절 필요하다/\n 매개변수 teamcolor이다..");
        foreach (var item in list)
        {
            item.SetScore(1f);
            //check 1
            if (teamName.GetComponent<SoccerTeam>().ControllingPlayer() != null)
                if (teamName.GetComponent<SoccerTeam>().IsPassSafeFromAllOpponents(teamName.GetComponent<SoccerTeam>().ControllingPlayer().transform.position, item.transform.position, null, Prm.instance.MaxPassingForce))
                {
                    item.SetScore(item.GetScore() + Prm.instance.Spot_PassSafeStrength);
                }
            Vector2 tmp = new Vector2();
            //check 2
            if (teamName.GetComponent<SoccerTeam>().CanShoot(item.transform.position, Prm.instance.MaxShootingForce, ref tmp))
            {
                item.SetScore(item.GetScore() + Prm.instance.Spot_CanScoreStrength);
            }

            if (teamName.GetComponent<SoccerTeam>().SupportingPlayer())
            {
                
                const float optimalDistance = 4.5f;
                Debug.Log("지원선수와 최소거리는 " + optimalDistance + " 로 설정됨.");
                float dist = Vector2.Distance(teamName.GetComponent<SoccerTeam>().ControllingPlayer().transform.position, item.transform.position);
                float temp = Mathf.Abs(optimalDistance - dist);

                if(temp < optimalDistance)
                {
                    item.SetScore(item.GetScore() + Prm.instance.Spot_DistFromControllingPlayerStrength * (optimalDistance - temp)/optimalDistance);
                }
            }

            if(item.GetScore() > bestScoreSoFar)
            {
                bestScoreSoFar = item.GetScore();
                bestSupportSpot = item.transform.position;
            }


        }
        
        return bestSupportSpot;
    }
}
