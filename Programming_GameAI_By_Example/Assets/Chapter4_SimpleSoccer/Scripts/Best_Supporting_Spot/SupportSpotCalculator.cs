/*
 * 선수 지원을 위해 Best Supporting Spot(BSS)을 계산합니다.
 * BSS는 아군의 패스를 받을 수 있는지, 해당 위치에서 슛을 할 수 있는지, 거리가 가까운지에 따라 다른 가중치를 받으며, 가장 높은 스코어를 가진 지점을 반환합니다.
 */

using System.Collections.Generic;
using UnityEngine;

public class SupportSpotCalculator : MonoBehaviour
{
    public static SupportSpotCalculator instance = null;

    public List<SupportSpot> blueZone;
    public List<SupportSpot> redZone;
    public Vector2 bestSupportSpot;
    public int frame;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        // 청팀의 스팟 저장
        var group = GameObject.Find("BlueZone").GetComponentsInChildren<SupportSpot>();
        if (group != null)
        {
            blueZone = new List<SupportSpot>(group);
        }

        // 적팀의 스팟 저장
        group = GameObject.Find("RedZone").GetComponentsInChildren<SupportSpot>();
        if (group != null)
        {
            redZone = new List<SupportSpot>(group);
        }

        bestSupportSpot = new Vector2(0, 0);
    }

    public Vector2 GetBestSupportingSpot(TeamColor team)
    {
        if (bestSupportSpot != Vector2.zero)
            return bestSupportSpot;
        else
            return DetermineBestSupportingPosition(team);
    }

    // 아군을 위해 각 Spot의 Score를 최신화 하여 Supporting Spot을 계산한다. 
    public Vector2 DetermineBestSupportingPosition(TeamColor team)
    {

        bestSupportSpot = new Vector2(0, 0);

        float bestScoreSoFar = 0f;

        List<SupportSpot> list = team == TeamColor.Red ? redZone : blueZone;
        GameObject teamName = team == TeamColor.Red ? GameObject.Find("RedTeam").gameObject : GameObject.Find("BlueTeam").gameObject;

        foreach (var item in list)
        {
            // 가중치 초기화
            item.SetScore(1f);

            // 1. 패스 경로가 안전한지 검사한다.
            if (teamName.GetComponent<SoccerTeam>().ControllingPlayer() != null)
            {
                if (teamName.GetComponent<SoccerTeam>().IsPassSafeFromAllOpponents(teamName.GetComponent<SoccerTeam>().ControllingPlayer().transform.position, item.transform.position, null, Prm.instance.MaxPassingForce))
                {
                    // 가중치 계산
                    item.SetScore(item.GetScore() + Prm.instance.Spot_PassSafeStrength);
                }

                Vector2 tmp = new Vector2();
                // 슛 할 수 있는지 검사 한다.
                if (teamName.GetComponent<SoccerTeam>().CanShoot(item.transform.position, Prm.instance.MaxShootingForce, ref tmp))
                {
                    // 가중치 계산
                    item.SetScore(item.GetScore() + Prm.instance.Spot_CanScoreStrength);
                }

                if (teamName.GetComponent<SoccerTeam>().SupportingPlayer())
                {
                    // 3. 지원하는 선수와의 최적 거리 계산
                    const float optimalDistance = 5f;
                    float dist = Vector2.Distance(teamName.GetComponent<SoccerTeam>().ControllingPlayer().transform.position, item.transform.position);
                    float temp = Mathf.Abs(optimalDistance - dist);

                    if (temp < optimalDistance)
                    {
                        // 가중치 계산. 먼 거리의 선수에게 더 높은 가중치를 둔다.
                        item.SetScore(item.GetScore() + Prm.instance.Spot_DistFromControllingPlayerStrength * (optimalDistance - temp) / optimalDistance);
                    }
                }
            }

            // 가장 높은 점수를 가진 BSS를 탐색한다.
            if (item.GetScore() > bestScoreSoFar)
            {
                bestScoreSoFar = item.GetScore();
                bestSupportSpot = item.transform.position;
            }
        }
        return bestSupportSpot;
    }
}
