using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrepareForKickOff : State<SoccerTeam>
{
    public static PrepareForKickOff instance = null;
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
        team.SetControllingPlayer(null);
        team.SetSuppprtingPlayer(null);
        team.SetReceiver(null);
        team.SetPlayerClosestToBall(null);

        team.ReturnAllFieldPlayersToHome();
    }

    public override void Execute(SoccerTeam team)
    {
        if(team.AllPlayersAtHome() && team.opponentTeam.AllPlayersAtHome())
        {
            team.GetFSM().ChangeState(Defending.instance);
        }
    }

    public override void Exit(SoccerTeam team)
    {
       // Debug.Log("\n" + miner.GetNameOfEntity() + ": " + "I am leaving the gold mine with my pockets full, oh sweet gold");
    }

}
