using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SupportAttacker : State<FieldPlayer>
{
    public static SupportAttacker instance = null;
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
    public override void Enter(FieldPlayer player)
    {
        player.Steering().ArriveOn();
        player.Steering().SetTarget(player.Team().GetSupportSpot(player.Team().teamColor));
    }

    public override void Execute(FieldPlayer player)
    {
        if (!player.Team().InControl())
        {
            player.GetFSM().ChangeState(ReturnToHomeRegion.instance);
            return;
        }
        if(player.Team().GetSupportSpot(player.Team().teamColor) != player.Steering().Target())
        {
            player.Steering().SetTarget(player.Team().GetSupportSpot(player.Team().teamColor));
            player.Steering().ArriveOn();
        }
        Vector2 t = new Vector2();
        if ( player.Ball().GetOwner() != player.gameObject && player.Team().CanShoot(player.transform.position, Prm.instance.MaxShootingForce,ref t)
            && player.Ball().GetOwner () != null && player.Ball().GetOwner().transform.parent.GetComponent<SoccerTeam>().teamColor == player.Team().teamColor)
        {
            Debug.Log(player.name + " 의 PASS 요청 1");
            player.Team().RequestPass(player);
          
        }

        if (player.AtTarget())
        {
            player.Steering().ArriveOff();

            player.TrackBall();

            player.SetVelocity(Vector2.zero);

            if (!player.IsThreatened() && player.Ball().GetOwner() != player.gameObject && player.Ball().GetOwner() != null 
                && player.Ball().GetOwner().transform.parent.GetComponent<SoccerTeam>().teamColor == player.Team().teamColor)
            {
                Debug.Log(player.name + " 의 PASS 요청 2");
                player.Team().RequestPass(player);
            }
        }
    }

    public override void Exit(FieldPlayer player)
    {
        player.Team().SetSuppprtingPlayer(null);
        player.Steering().ArriveOff();
    }
}
