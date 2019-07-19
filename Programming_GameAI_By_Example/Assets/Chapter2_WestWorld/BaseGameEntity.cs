using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseGameEntity : MonoBehaviour
{
    // Each entities has own ID
    private int m_ID;
    // Checking Next ID's validity.
    private static int m_iNextValidID;
    
    // Set the ID number.
    private void SetID(int val)
    {
        m_ID = val;
    }

    // Constructor for this Class, And ,set own ID to newer entity.
    public BaseGameEntity(int id = 0)
    {
        SetID(id);
    }

    public int GetNameOfEntity()
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
        Nothing, GoldMine, Bank, Bar, Home
    }
}
