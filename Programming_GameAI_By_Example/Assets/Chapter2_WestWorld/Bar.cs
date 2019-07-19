using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bar : MonoBehaviour
{
    // Singleton design pattern implementation.
    public static Bar instance = null;
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

    public void OrderVodkaAndDrink(Miner miner)
    {
        Debug.Log("\n" + miner.GetNameOfEntity() + ": " + "Drinking off a cup of Whisky");
        miner.SetThirst(0);
    }


}
