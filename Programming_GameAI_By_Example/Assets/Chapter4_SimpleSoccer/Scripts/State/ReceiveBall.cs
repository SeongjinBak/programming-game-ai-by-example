using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReceiveBall : State<FieldPlayer>
{
   
    public static ReceiveBall instance = null;
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
        player.Team().SetReceiver(player.transform.parent.gameObject);

        player.Team().SetControllingPlayer(player.gameObject);

        const float passThreatRadius =3f;
        Debug.Log("passThreatRadius: " + passThreatRadius);

        if ((player.InHotRegion() || Random.Range(0f, 1f) < Prm.instance.ChanceOfUsingArriveTypeReceiveBehavior) && !player.Team().IsOpponentWithInRadius(player.transform.position, passThreatRadius))
        {
            player.Steering().ArriveOn();
        }
        else
        {
            player.Steering().PursuitOn();
        }

    }

    public override void Execute(FieldPlayer player)
    {
        if(player.BallWithinReceivingRange() || !player.Team().InControl())
        {
            player.GetFSM().ChangeState(ChaseBall.instance);
            return;
        }

        if (player.Steering().PursuitIsOn())
        {
            player.Steering().SetTarget(player.Ball().transform.position);
        }

        if (player.AtTarget())
        {
            Debug.Log("AtTarget Range check is needeed");
            player.Steering().ArriveOff();
            player.Steering().PursuitOff();
            Debug.Log("Track Ball is executed. I think we don't have to add this code.");
            player.TrackBall();
            player.SetVelocity(Vector2.zero);

        }
    }

    public override void Exit(FieldPlayer player)
    {
        player.Steering().ArriveOff();
        player.Steering().PursuitOff();

        player.Team().SetReceiver(null);
    }
}
