using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReturnToHomeRegion : State<FieldPlayer>
{
    public static ReturnToHomeRegion instance = null;
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
    public override void Enter(FieldPlayer team)
    {
       
    }

    public override void Execute(FieldPlayer team)
    {
        
    }

    public override void Exit(FieldPlayer team)
    {
        // Debug.Log("\n" + miner.GetNameOfEntity() + ": " + "I am leaving the gold mine with my pockets full, oh sweet gold");
    }
}
