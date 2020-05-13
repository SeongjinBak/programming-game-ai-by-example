using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBase : MovingEntity_CH4
{

    public float myMaxSpeed = 10f;
    public float myMaxTurnRate = 2f;
    public Vector2 myMaxForce = new Vector2(10f, 10f);
    [SerializeField]
    private string playerRole;
    public Vector2 homeRegion;
    [SerializeField]
    private Steering_CH4 steering;
    [SerializeField]
    private SoccerBall soccerBall;
    [SerializeField]
    private float distToBall;
    // Start is called before the first frame update
    void Awake()
    {
        if (transform.name == "1")
        {
            playerRole = "GoalKeeper";
        }
        else
        {
            if (transform.name == "4" || transform.name == "5")
            {
                playerRole = "Attacker";
            }
            else
                playerRole = "Defender";
        }
        homeRegion = gameObject.transform.position;
        steering = GetComponent<Steering_CH4>();


    }

    private void Start()
    {
        soccerBall = GameObject.Find("Ball").GetComponent<SoccerBall>();
    }
    // Update is called once per frame
    void Update()
    {
        SetPos(transform.position);
        v_heading = (Steering().Force().normalized);
    }

    public bool HandleMessage(Telegram_CH4 msg)
    {
        return false;
    }

    public void SetMaxSpeed(float speed)
    {
        myMaxSpeed = speed;

    }

    public float DistToBall()
    {
        return distToBall;
    }
    public void SetDistToBall(float num)
    {
        distToBall = num;
    }
    public SoccerTeam Team()
    {
        return transform.parent.GetComponent<SoccerTeam>();
    }
    public bool IsClosestTeamMemberToBall()
    {
        return Team().PlayerClosestToBall() == this.GetComponent<PlayerBase>();
    }

    public Steering_CH4 Steering()
    {
        return steering;
    }

    public bool AtTarget()
    {
        return Vector2.Distance((Vector2)transform.position, Steering().Target()) < Mathf.Sqrt(Prm.instance.PlayerInTargetRange);
    }


    public SoccerBall Ball()
    {
        return soccerBall;
    }

    public bool IsAheadOfAttacker()
    {
        return Mathf.Abs(transform.position.x - Team().opponentsGoal.Center().x) < Mathf.Abs(Team().ControllingPlayer().transform.position.x - Team().opponentsGoal.Center().x);
    }

    public bool IsControllingPlayer()
    {
        return Team().ControllingPlayer() == this.gameObject;
    }

    public bool InHomeRegion()
    {
        if (Team().teamColor == TeamColor.Red)
            return ((int)transform.position.x == (int)Team().initialRegion[Id() - 1].x && (int)transform.position.y == (int)Team().initialRegion[Id() - 1].y);
        else
            return ((int)transform.position.x == (int)Team().initialRegion[Id() - 6].x && (int)transform.position.y == (int)Team().initialRegion[Id() - 6].y);
    }

    public Vector2 HomeRegion()
    {
        return homeRegion;
    }

    public void SetHomeRegion(Vector2 region)
    {
        homeRegion = region;

    }

    public void SetVelocity(Vector2 v)
    {
        v_velocity = v;
    }

    public SoccerPitch Pitch()
    {
        return SoccerPitch.instance;
    }

    public void TrackBall()
    {
    }

    public string Role()
    {
        return playerRole;
    }

    public bool InHotRegion()
    {
        return Mathf.Abs(transform.position.x - Team().opponentsGoal.Center().x) < Pitch().PlayingAreaLength() / 3;
    }

    public bool PositionInFrontOfPlayer(Vector2 pos)
    {
        Vector2 toSubject = pos - (Vector2)transform.position;
        if (Vector2.Dot(toSubject, Heading()) > 0)
        {
            return true;
        }
        return false;
    }


    public bool IsThreatened()
    {
        foreach (var item in Team().opponentTeam.players)
        {
            if (PositionInFrontOfPlayer(item.transform.position) && (Vector2.Distance(transform.position, item.transform.position) < Prm.instance.PlayerComfortZone))
            {
                return true;
            }
        }
        return false;
    }

    public bool IsClosestPlayerOnPitchToBall()
    {
        return IsClosestTeamMemberToBall() && (Vector2.Distance(Ball().transform.position, transform.position) < Team().opponentTeam.DistToBallOfClosestPlayer());
    }

    public void FindSupport()
    {
        if (Team().SupportingPlayer() == null)
        {
            PlayerBase _bestSupportPlayer = Team().DetermineBestSupportingAttacker();
            Team().SetSuppprtingPlayer(_bestSupportPlayer.gameObject);
            MessageDispatcher_CH4.instance.DispatchMessage(0f, Id(), Team().SupportingPlayer().GetComponent<PlayerBase>().Id(), SoccerMessages.Msg_SupportAttacker, null);

        }
        PlayerBase bestSupportPlayer = Team().DetermineBestSupportingAttacker();

        if (bestSupportPlayer && bestSupportPlayer != Team().SupportingPlayer())
        {
            if (Team().SupportingPlayer())
            {
                MessageDispatcher_CH4.instance.DispatchMessage(0f, Id(), Team().SupportingPlayer().GetComponent<PlayerBase>().Id(), SoccerMessages.Msg_GoHome);
            }

            Team().SetSuppprtingPlayer(bestSupportPlayer.gameObject);

            MessageDispatcher_CH4.instance.DispatchMessage(0f, Id(), Team().SupportingPlayer().GetComponent<PlayerBase>().Id(), SoccerMessages.Msg_SupportAttacker);
        }
    }

}
