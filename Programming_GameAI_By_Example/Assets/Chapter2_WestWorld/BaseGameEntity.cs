using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseGameEntity : MonoBehaviour
{
    // Each entities has own ID
    private int m_ID;
    // Entity's own name
    private string m_Name;

    // Checking Next ID's validity.
    private static int m_iNextValidID;

    // Set the ID number.
    public void SetID(int val, string name)
    {
        m_ID = val;
        m_Name = name;
    }

    public abstract bool HandleMessage(Telegram msg);
  


    // Constructor for this Class, And ,set own ID to newer entity.
    public BaseGameEntity(int id = 0, string name = "")
    {
       // SetID(id, name);
    }

    public string GetNameOfEntity()
    {
        return m_Name;
    }

    public int GetIDOfEntity()
    {
        return m_ID;
    }

    // All of entities should have Updating function. 
    protected virtual IEnumerator Updating()
    {
        yield return null;
    }

    public enum LocationType
    {
        Nothing, GoldMine, Bank, Bar, Home, Toilet
    }
}
