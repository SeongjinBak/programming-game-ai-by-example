using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoHouseWork : State<Wife>
{
    public static DoHouseWork instance = null;
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

    public override void Enter(Wife wife)
    {
        Debug.Log("\n" + wife.GetNameOfEntity() + ": It's time to do homeworks!");
    }

    public override void Execute(Wife wife)
    {
        int random = Random.Range(0, 2);
        switch (random)
        {
            case 0:
                Debug.Log("\n" + wife.GetNameOfEntity() + ": Sweeping the bottom.");
                break;
            case 1:
                Debug.Log("\n" + wife.GetNameOfEntity() + ": Setting the bed.");
                break;
        }

    }

    public override void Exit(Wife wife)
    {
    }

}
