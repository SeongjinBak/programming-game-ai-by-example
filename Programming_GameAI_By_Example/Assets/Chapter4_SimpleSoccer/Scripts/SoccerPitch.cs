using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoccerPitch : MonoBehaviour
{
    // Role as a game manager.

    public static SoccerPitch instance = null;
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
        {
            Destroy(this.gameObject);
        }
        DontDestroyOnLoad(this.gameObject);

        entityCount = 0;
    }

   

    public int entityCount;

    public SoccerBall ball;
    public SoccerTeam redTeam;
    public SoccerTeam blueTeam;

    public Goal redGoal;
    public Goal blueGoal;
    [SerializeField]
    //true if a goal keeper has possession
    bool m_bGoalKeeperHasBall;
    [SerializeField]
    //true if the game is in play. Set to false whenever the players
    //are getting ready for kickoff
    bool m_bGameOn;
    [SerializeField]
    //set true to pause the motion
    bool m_bPaused;

    public bool Inside(Vector2 v)
    {
        if (v.x >= -25f && v.x <= 25f && v.y <=9.3f && v.y >= 9.3f)
        {
            return true;
        }
        return false;
    }
    public bool GoalKeeperHasBall() { return m_bGoalKeeperHasBall; }
    public void SetGoalKeeperHasBall(bool b) { m_bGoalKeeperHasBall = b; }
    public bool GameOn() { return m_bGameOn; }
    public void SetGameOn() { m_bGameOn = true; }
    public void SetGameOff() { m_bGameOn = false; }
    public void TogglePause() { m_bPaused = !m_bPaused; }
    public bool Paused() { return m_bPaused; }
    public float PlayingAreaLength()
    {
        return 50f;
    }
}
