using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Telegram 
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
    // Extra info.
    public delegate void ExtraInfo();
    public ExtraInfo info;

    public void ConstructTelegram(float delayTime, int _sender, int _receiver, int _msg, Telegram.ExtraInfo _info = null)
    {
        DispatchTime = delayTime;
        sender = _sender;
        receiver = _receiver;
        msg = _msg;
        info = _info;
    }

    public int GetMessageIndex()
    {
        return msg;
    }
}
