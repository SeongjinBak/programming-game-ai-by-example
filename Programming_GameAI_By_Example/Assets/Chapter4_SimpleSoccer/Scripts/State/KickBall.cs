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
        WaitForSeconds ws = new WaitForSeconds(Time.deltaTime / num);
        Debug.Log(ws + " 번당 1번 킥 시도함. p185참고..!");
        for (int i = 0; i < num; i++)
        {
            if(Random.Range(0f,1f)< .8f)
            {
                flag = true;
                Debug.Log(fp.gameObject.name + " 킥 성공 at " + Time.time);
                break;
            }
            yield return ws;
        }
        if(!flag)
            fp.GetFSM().ChangeState(ChaseBall.instance);
    }

    public override void Enter(FieldPlayer player)
    {
        player.Team().SetControllingPlayer(player.transform.parent.gameObject);
        StartCoroutine(ReadyForKick(player));
        
    }
    
    public Vector2 AddNoiseToKick(Vector2 pos, Vector2 target)
    {
        Vector2 tmp = new Vector2(target.x * Prm.instance.PlayerKickingAccuracy, target.y * Prm.instance.PlayerKickingAccuracy);
        return tmp;
    }
    
    public override void Execute(FieldPlayer player)
    {
        Vector2 toBall = player.Ball().transform.position - player.transform.position;
        float _dot = Vector2.Dot(player.Heading(), toBall.normalized);
        Debug.Log("KickBall 에서, _dot은 Heading()을 사용하는데 우리가 Heading()값을 바꾸는것을 했는지?\n 아니라면, 그냥 x좌표로만 앞뒤 판별 가능하다.\n근데 _dot은 아래서 power계산에쓰임..");
        if(player.Team().Receiver()!=null || player.Pitch().GoalKeeperHasBall() | _dot < 0)
        {
            player.GetFSM().ChangeState(ChaseBall.instance);
        }

        // if a shot is possible, 
        Vector2 ballTarget = Vector2.zero;

        float power = Prm.instance.MaxShootingForce * _dot;

        if (player.Team().CanShoot(player.Ball().transform.position, power, ref ballTarget) ||
            Random.Range(0f, 1f) < Prm.instance.ChancePlayerAttemptsPotShot)
        {
            ballTarget = AddNoiseToKick(player.Ball().transform.position, ballTarget);


            Vector2 kickDirection = ballTarget - (Vector2)player.Ball().transform.position;

            player.Ball().Kick(kickDirection, power);

            player.GetFSM().ChangeState(Wait.instance);

            player.FindSupport();
            return;
        }


        /* Trial for pass to player */
        PlayerBase receiver = null;

        power = Prm.instance.MaxPassingForce * _dot;

        if (player.IsThreatened() && player.Team().FindPass(player, receiver, ref ballTarget, power, Prm.instance.MinPassDist))
        {
            ballTarget = AddNoiseToKick(player.Ball().transform.position, ballTarget);
            Vector2 kickDirection = ballTarget - (Vector2)player.Ball().transform.position;
            player.Ball().Kick(kickDirection, power);
            GameObject ballTargetTr = new GameObject(); 
            ballTargetTr.transform.position = ballTarget;
            MessageDispatcher_CH4.instance.DispatchMessage(0f, player.ID(), receiver.ID(), SoccerMessages.Msg_ReceiveBall, ballTargetTr.transform);
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
        // Debug.Log("\n" + miner.GetNameOfEntity() + ": " + "I am leaving the gold mine with my pockets full, oh sweet gold");
    }
}
