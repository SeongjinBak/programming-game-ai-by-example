/*
 * Kick Ball State.
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KickBall : State<FieldPlayer>
{
    public static KickBall instance = null;
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

    IEnumerator ReadyForKick(FieldPlayer fp)
    {
        bool flag = false;
        int num = Prm.instance.PlayerKickFrequency;
        WaitForSeconds ws = new WaitForSeconds((1f / num) * Random.Range(0.1f, .5f));
        for (int i = 0; i < num; i++)
        {
            if (Random.Range(0f, 1f) < .8f)
            {
                flag = true;
                break;
            }
            yield return ws;
        }
        
        // 킥에 실패한 경우
        if (!flag)
        {
            fp.GetFSM().ChangeState(ChaseBall.instance);
        }
            
    }

    public override void Enter(FieldPlayer player)
    {
        player.Team().SetControllingPlayer(player.gameObject);
        StartCoroutine(ReadyForKick(player));
        
    }
    
    // 킥 방향에 약간의 노이즈를 설정한다.
    public Vector2 AddNoiseToKick(Vector2 pos, Vector2 target)
    {
        Vector2 tmp = new Vector2(target.x * Prm.instance.PlayerKickingAccuracy, target.y * Prm.instance.PlayerKickingAccuracy);
        return tmp;
    }

    public override void Execute(FieldPlayer player)
    {
        Vector2 toBall = player.Ball().transform.position - player.transform.position;

        float _dot = Vector2.Dot(player.Heading(), toBall.normalized);

        if (player.Team().Receiver() != null || player.Pitch().GoalKeeperHasBall() || _dot < 0)
        {
            player.GetFSM().ChangeState(ChaseBall.instance);
            return;
        }

        Vector2 ballTarget = new Vector2(0f, 0f);

        float power = Prm.instance.MaxShootingForce * _dot;
        float rf = Random.Range(0f, 1f);

        // 슛을 할 수 있는지 판단 후, 슛을 한다.
        if (player.Team().CanShoot(player.Ball().transform.position, power, ref ballTarget) || rf < Prm.instance.ChancePlayerAttemptsPotShot)
        {
            Debug.Log("Power : " + power);

            ballTarget = AddNoiseToKick(player.Ball().transform.position, ballTarget);

            Vector2 kickDirection = (ballTarget - (Vector2)player.Ball().transform.position);

            player.Ball().SetOwner(player.gameObject);
            player.Ball().Kick(kickDirection, power);

            player.GetFSM().ChangeState(Wait.instance);
            player.Ball().SetOwner(null);

            player.FindSupport();
            return;
        }


        // 슛을 할 수 없다면 패스 시도한다.
        PlayerBase receiver = null;

        power = Prm.instance.MaxPassingForce * _dot;
        // 위협받고 있으며 패스 할 선수가 있을 경우 패스
        if (player.IsThreatened() && player.Team().FindPass(player, receiver, ref ballTarget, power, Prm.instance.MinPassDist))
        {
            Debug.Log("Power : " + power);

            ballTarget = AddNoiseToKick(player.Ball().transform.position, ballTarget);
            Vector2 kickDirection = ballTarget - (Vector2)player.Ball().transform.position;

            player.Ball().SetOwner(player.gameObject);
            player.Ball().Kick(kickDirection, power);
            
            GameObject ballTargetTr = new GameObject();
            ballTargetTr.transform.position = ballTarget;
            
            // 팀원에게 패스 리시브 요청한다.
            MessageDispatcher_CH4.instance.DispatchMessage(0f, player.Id(), receiver.Id(), SoccerMessages.Msg_ReceiveBall, ballTargetTr.transform);
            player.GetFSM().ChangeState(Wait.instance);
            player.FindSupport();
            return;
        }
        else
        {
            player.FindSupport();
            player.GetFSM().ChangeState(Dribble.instance);
        }

    }

    public override void Exit(FieldPlayer player)
    {

        //player.Team().SetControllingPlayer(null);
    }
}
