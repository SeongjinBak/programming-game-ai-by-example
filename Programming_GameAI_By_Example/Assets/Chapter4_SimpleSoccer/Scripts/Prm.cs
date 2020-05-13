using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prm : MonoBehaviour
{
    public static Prm instance = null;
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

    [Header("MaxPower")]
    public float MaxPassingForce = .4f;
    public float MaxShootingForce = 0.9f;
    public float PlayerMaxSpeedWithBall = 2f;
    public float PlayerMaxSpeedWithoutBall = 2.8f;
    public float PlayerInTargetRange = 3f;
    public float PlayerKickingDistance = .2f;
    public float MinPassDist = 1f;
    public float MaxDribbleForce = .6f;
    public float GoalKeeperTendingDistance = 2f;
    [Header("Strength")]
    public float Spot_PassSafeStrength = 2f;
    public float Spot_CanScoreStrength = 1f;
    public float Spot_DistFromControllingPlayerStrength = 2f; 

    [Header("Misc")]
    public int NumAttemptsToFindValidStrike = 4;
    public int PlayerKickFrequency = 3;
    public float ChanceOfUsingArriveTypeReceiveBehavior = 0.5f;
    public float ChancePlayerAttemptsPotShot = 0.02f;
    public float BallWithinReceivingRange = 2f;
    public float PlayerKickingAccuracy = 0.99f;
    public float PlayerComfortZone = 2.0f;

    [Header("KEEPER")]
    public float KeeperInBallRange = .5f;
    public float GoalKeeperInterceptRange = 2f;
    public float GoalKeeperMinPassDist = 5f;

}
