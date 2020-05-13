using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    public Text redScore;
    public Text blueScore;
    private bool isScored;

    private void Start()
    {
        isScored = false;
    }
    // 득점
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isScored)
            return;

        isScored = true;

        if(gameObject.name == "RedWall")
        {
            int score = int.Parse(blueScore.text) + 1;
            blueScore.text = score.ToString();
        }
        else
        {
            int score = int.Parse(redScore.text) + 1;
            redScore.text = score.ToString();
        }
        GoalKeeper keeper = GameObject.Find("1").GetComponent<GoalKeeper>();
        keeper.Team().SetControllingPlayer(null);

        keeper.Team().opponentTeam.ReturnAllFieldPlayersToHome();
        keeper.Team().ReturnAllFieldPlayersToHome();
        StartCoroutine(WaitTillWallPlayerAtHome());
        isScored = false;
    }

    // 모든 플레이어가 본래 위치에 있을 경우, 경기 재개
    private IEnumerator WaitTillWallPlayerAtHome()
    {
        GoalKeeper keeper = GameObject.Find("1").GetComponent<GoalKeeper>();
        while (!keeper.Team().AllPlayersAtHome() || !keeper.Team().opponentTeam.AllPlayersAtHome())
        {
            keeper.Team().opponentTeam.ReturnAllFieldPlayersToHome();
            keeper.Team().ReturnAllFieldPlayersToHome();
            yield return new WaitForSeconds(0.09f);
        }
        SoccerBall ball = GameObject.Find("Ball").GetComponent<SoccerBall>();
        ball.ResetBallState();
    }
}
