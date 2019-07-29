using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CookStew : State<Wife>
{
    public static CookStew instance = null;
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

            // integer 1 is "StewReady" Msg... sorry.
            case 1:
                {
                    Debug.Log("\nMessage received by " + GameObject.Find("Elsa").GetComponent<Wife>().GetNameOfEntity() + " at time: " + Time.time);
                    Debug.Log("\n" + GameObject.Find("Elsa").GetComponent<Wife>().GetNameOfEntity() + " Stew Ready! Let's eat.");

                    // Inform her husband stew is ready.
                    MessageDispatcher.instance.DispatchMessage(0f, GameObject.Find("Elsa").GetComponent<Wife>().GetIDOfEntity(), GameObject.Find("Miner").GetComponent<Miner>().GetIDOfEntity(), MessageDispatcher.instance.messageType["Msg_StewReady"]);

                    GameObject.Find("Elsa").GetComponent<Wife>().GetFSM().ChangeState(DoHouseWork.instance);
              
                    GameObject.Find("Elsa").GetComponent<Wife>().Cooking = false;
                   
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
        Debug.Log("\n" + wife.GetNameOfEntity() + ": Cooking for ma' husband.");

    }

    public override void Exit(Wife wife)
    {
      
    }
}
