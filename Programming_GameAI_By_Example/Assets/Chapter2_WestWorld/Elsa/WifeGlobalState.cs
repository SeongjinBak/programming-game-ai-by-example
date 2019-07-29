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

    public override void Enter(Wife wife)
    {
        if (!wife.Cooking)
        {
            Wife tmpWife = GameObject.Find("Elsa").GetComponent<Wife>();
            Debug.Log("\n" + tmpWife.GetNameOfEntity() + " Puttin' the stew in the oven");

            // Send delayed msg, and inform ** later ** .
            MessageDispatcher.instance.DispatchMessage(1.5f, tmpWife.GetIDOfEntity(), tmpWife.GetIDOfEntity(), MessageDispatcher.instance.messageType["Msg_StewReady"]);

            tmpWife.Cooking = true;
        }
    }

    public override void Execute(Wife wife)
    { 
        float random = Random.Range(0.0f, 10.0f);
        if (random >= 9.0f)
        {
            if (!wife.GetFSM().IsInstate(VisitBathroom.instance))
            {
                wife.GetFSM().ChangeState(VisitBathroom.instance);
                wife.ChangeLocation(BaseGameEntity.LocationType.Toilet);
            }
        }
    }

    public override void Exit(Wife wife)
    {
    }
}
