using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinerGlobalState : State<Miner>
{
    // Singleton design pattern implementation.
    public static MinerGlobalState instance = null;
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
    // Update is called once per frame
    void Update()
    {

    }
}
