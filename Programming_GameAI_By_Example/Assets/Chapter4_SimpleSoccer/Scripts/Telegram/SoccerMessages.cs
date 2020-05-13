using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoccerMessages : MonoBehaviour
{
    public static SoccerMessages instance = null;
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

    public const int Msg_ReceiveBall = 0;
    public const int Msg_PassToMe = 1;
    public const int Msg_SupportAttacker = 2;
    public const int Msg_GoHome = 3;
    public const int Msg_Wait = 4;


    public string MessageToString(int msg)
    {
        switch (msg)
        {
            case Msg_ReceiveBall:

                return "Msg_ReceiveBall";

            case Msg_PassToMe:

                return "Msg_PassToMe";

            case Msg_SupportAttacker:

                return "Msg_SupportAttacker";

            case Msg_GoHome:

                return "Msg_GoHome";

            case Msg_Wait:

                return "Msg_Wait";
            default:
                return "INVALID MESSAGE";
        }
    }
}