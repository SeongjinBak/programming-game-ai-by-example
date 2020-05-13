/*
 * Dribble State.
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dribble : State<FieldPlayer>
{
    public static Dribble instance = null;
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

    // 컨트롤 할 플레이어 변경
    public override void Enter(FieldPlayer player)
    {
        player.Team().SetControllingPlayer(player.gameObject);
    }

    public override void Execute(FieldPlayer player)
    {
        // 본인팀의 골대와 플레이어의 방향 벡터 내적하여 각도 판별
        float _dot = Vector2.Dot(player.Team().HomeGoal().Facing(), player.Heading());
        
        // 플레이어가 본인 골대를 향해있다면, 상대편 골대 쪽으로 방향을 튼다.
        if (_dot < 0)
        {
            Vector2 direction = -player.Heading();
            float sign = Vector2.Dot(player.Team().HomeGoal().Facing(), player.Heading());
            sign = sign > 0 ? 1f : -1f;
            float angle = Mathf.PI / 4 * -1 * sign;

            player.transform.Rotate(direction, angle);

            // 드리블 속도
            const float kickingForce = .2f;

            player.Ball().SetOwner(player.gameObject);
            player.Ball().Kick(direction, kickingForce);
        }
        else
        {
            player.Ball().SetOwner(player.gameObject);
            player.Ball().Kick(player.Team().HomeGoal().Facing(), .5f);
        }

        //Player has to chase the ball after short term kicking.
        player.GetFSM().ChangeState(ChaseBall.instance);
        return;
    }

    public override void Exit(FieldPlayer player)
    {
    }
}
