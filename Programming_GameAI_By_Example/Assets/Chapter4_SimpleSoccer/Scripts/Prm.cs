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
    public float MaxPassingForce = 5f;
    public float MaxShootingForce = 7f;

    [Header("Strength")]
    public float Spot_PassSafeStrength = 2f;
    public float Spot_CanScoreStrength = 1f;
    public float Spot_DistFromControllingPlayerStrength = 2f; 

    [Header("Misc")]
    public int NumAttemptsToFindValidStrike = 5;


}
