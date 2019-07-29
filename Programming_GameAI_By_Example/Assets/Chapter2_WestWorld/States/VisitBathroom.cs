using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisitBathroom : State<Wife>
{
    public static VisitBathroom instance = null;
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
    public override void Enter(Wife wife)
    {
        Debug.Log("\n" + wife.GetNameOfEntity() + ": walking to the bin.. I'm going to pee.");
    }

    public override void Execute(Wife wife)
    {
        Debug.Log("\n" + wife.GetNameOfEntity() + ": Ahhh! Sweet relief!");
        wife.ChangeLocation(BaseGameEntity.LocationType.Home);
        wife.GetFSM().RevertToPreviousState();
    }

    public override void Exit(Wife wife)
    {
        Debug.Log("\n" + wife.GetNameOfEntity() + ": Getting out from the toilet.");
    }

}
