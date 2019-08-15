using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Defending : State<SoccerTeam>
{
    public static Defending instance = null;
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
        if(team.teamColor == TeamColor.Blue)
        {
            ChangePlayerHomeRegion(team, blueRegions);
        }
        else
        {
            ChangePlayerHomeRegion(team, redRegions);

        }
        
        team.UpdateTargetsOfWaitingPlayers();
        Debug.Log("\n[Team " + team.teamColor + "] has Entered Defending State!");
    }

    public override void Execute(SoccerTeam team)
    {
        if (team.InControl())
        {

            team.GetFSM().ChangeState(Attacking.instance);
            return;
        }
    }

    public override void Exit(SoccerTeam team)
    {
        // Debug.Log("\n" + miner.GetNameOfEntity() + ": " + "I am leaving the gold mine with my pockets full, oh sweet gold");
    }


    public void ChangePlayerHomeRegion(SoccerTeam team, Vector2 [] region)
    {
        for(int p = 0 ; p < 5; p++)
        {
            team.SetPlayerHomeRegion(p, region[p]);
        }
    }
}
