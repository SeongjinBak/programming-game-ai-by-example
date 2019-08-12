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
    public float MaxPassingForce = 3f;
    public float MaxShootingForce = 6f;
    public float PlayerMaxSpeedWithBall = 1.2f;
    public float PlayerMaxSpeedWithoutBall = 1.6f;
    public float PlayerInTargetRange = .4f;
    public float PlayerKickingDistance = .4f;
    public float MinPassDist = 5f;

    [Header("Strength")]
    public float Spot_PassSafeStrength = 2f;
    public float Spot_CanScoreStrength = 1f;
    public float Spot_DistFromControllingPlayerStrength = 2f; 

    [Header("Misc")]
    public int NumAttemptsToFindValidStrike = 5;
    public int PlayerKickFrequency = 8;
    public float ChanceOfUsingArriveTypeReceiveBehavior = 0.5f;
    public float ChancePlayerAttemptsPotShot = 0.05f;
    public float BallWithinReceivingRange = 2f;
    public float PlayerKickingAccuracy = 0.99f;
    public float PlayerComfortZone = 5.0f;

}
