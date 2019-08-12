using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attacking : State<SoccerTeam>
{
    public static Attacking instance = null;
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

    public override void Enter(SoccerTeam team)
    {
        Vector2[] blueRegions = GameObject.Find("BlueTeam").GetComponent<SoccerTeam>().initialRegion;
        Vector2[] redRegions = GameObject.Find("RedTeam").GetComponent<SoccerTeam>().initialRegion;

        if (team.teamColor == TeamColor.Blue)
        {
            blueRegions[3] = redRegions[1];
            blueRegions[4] = redRegions[2];
            blueRegions[1] = redRegions[3];
            blueRegions[2] = redRegions[4];
            Defending.instance.ChangePlayerHomeRegion(team, blueRegions);
        }
        else
        {
            redRegions[1] = blueRegions[3];
            redRegions[2] = blueRegions[4];
            redRegions[3] = blueRegions[1];
            redRegions[4] = blueRegions[2];

            Defending.instance.ChangePlayerHomeRegion(team, redRegions);
        }

        team.UpdateTargetsOfWaitingPlayers();
    }

    public override void Execute(SoccerTeam team)
    {
        if (!team.InControl())
        {     
            team.GetFSM().ChangeState(Defending.instance);
        }
        Debug.Log("여기서, BSS 계산했는데 어디다가반환을 안한다.");
        SupportSpotCalculator.instance.DetermineBestSupportingPosition(team.teamColor);
    }

    public override void Exit(SoccerTeam team)
    {
        // Debug.Log("\n" + miner.GetNameOfEntity() + ": " + "I am leaving the gold mine with my pockets full, oh sweet gold");
    }

}
