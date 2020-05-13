/*
 * Support Attacker Ball State.
 * 공격하고 있는 팀원을 보조하는 상태이다.
 */
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
        // Arrive할 타겟 설정
        player.Steering().SetTarget(player.Team().GetSupportSpot(player.Team().teamColor));
    }

    public override void Execute(FieldPlayer player)
    {
        // 현재 공을 가진 선수가 자신의 팀이 아닌경우, 위치로 복귀
        if (!player.Team().InControl())
        {
            player.GetFSM().ChangeState(ReturnToHomeRegion.instance);
            return;
        }

        // 지원할 장소 위치가 지원 선수위치와 다른경우 
        if(Vector2.Distance(player.Team().GetSupportSpot(player.Team().teamColor),  player.Steering().Target()) > Mathf.Epsilon)
        {
            player.Steering().SetTarget(player.Team().GetSupportSpot(player.Team().teamColor));
            player.Steering().ArriveOn();
        }

        // 공을 가진 선수에게 이 플레이어로 패스를 요청한다.
        Vector2 t = new Vector2();
        if ( player.Ball().GetOwner() != player.gameObject && player.Team().CanShoot(player.transform.position, Prm.instance.MaxShootingForce,ref t)
            && player.Ball().GetOwner () != null && player.Ball().GetOwner().transform.parent.GetComponent<SoccerTeam>().teamColor == player.Team().teamColor)
        {
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
