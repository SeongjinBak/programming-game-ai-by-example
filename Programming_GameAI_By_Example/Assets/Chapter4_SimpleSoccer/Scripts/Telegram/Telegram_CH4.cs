using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Telegram_CH4
{
    // sender entity.
    int sender;
    // receiver entity.
    int receiver;
    public int Receiver {
        get => receiver;
        set => receiver = value;
    }

    // The Message.
    int msg;

    // Message would be delayed or dispatched immediately
    // If it should be delayed, this field must Timestamped before dispatching.
    private float dispatchTime;
    public float DispatchTime
    {
        get { return dispatchTime; }
        set { dispatchTime = value; }
    }

    public Transform infos;

    public void ConstructTelegram(float delayTime, int _sender, int _receiver, int _msg, Transform info = null)
    {
        DispatchTime = delayTime;
        sender = _sender;
        receiver = _receiver;
        msg = _msg;
        infos = info;
    }

    public int GetMessageIndex()
    {
        return msg;
    }
}
