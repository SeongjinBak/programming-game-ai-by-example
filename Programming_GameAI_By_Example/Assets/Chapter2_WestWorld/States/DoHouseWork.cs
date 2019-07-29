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

}
