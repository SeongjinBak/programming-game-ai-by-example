using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WifeGlobalState : State<Wife>
{
    // Singleton design pattern implementation.
    public static WifeGlobalState instance = null;
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

    public override bool OnMessage(Wife entityType, Telegram telegram)
    {
        switch (telegram.GetMessageIndex())
        {

            // integer 0 is "Hi Honey Im home" Msg... sorry.
            case 0:
                {
                    Debug.Log("\nMessage handled by " + GameObject.Find("Elsa").GetComponent<Wife>().GetNameOfEntity() + " at time: " + Time.time);
                    Debug.Log("\n" + GameObject.Find("Elsa").GetComponent<Wife>().GetNameOfEntity() + " Hi honey. Let me make you some of mah fine country stew.");
                    GameObject.Find("Elsa").GetComponent<Wife>().GetFSM().ChangeState(CookStew.instance);
                }
                return true;
        }

        return false;
        
    }
}
