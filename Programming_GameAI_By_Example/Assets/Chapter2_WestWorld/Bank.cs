using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bank : MonoBehaviour
{
    // Singleton design pattern implementation.
    public static Bank instance = null;
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

    // A deposits which is the Miner saved.
    private int deposit;


    private void Start()
    {
        deposit = 0;
    }

    public void SaveDeposit(int nugget, Miner miner)
    {
        deposit += nugget;
        miner.SetGoldInBank(deposit);
    }

    public int GetDeposit()
    {
        return deposit;
    }


}
