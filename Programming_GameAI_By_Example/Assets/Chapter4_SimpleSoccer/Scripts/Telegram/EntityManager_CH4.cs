using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityManager_CH4 : MonoBehaviour
{
    // Singleton design pattern implementation.
    public static EntityManager_CH4 instance = null;
    [SerializeField]
    private Dictionary<int, FieldPlayer> m_EntityMap;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
        {
            Destroy(this.gameObject);
        }
        DontDestroyOnLoad(this.gameObject);

        m_EntityMap = new Dictionary<int, FieldPlayer>();
    }

    // Same as ' = ' operator
    public EntityManager_CH4 IsEqual(EntityManager_CH4 em)
    {
        return em;
    }

    public void RegisterEntity(FieldPlayer newEntity)
    {
        m_EntityMap.Add((newEntity as BaseGameEntity_CH3).ID(), newEntity);
        Debug.Log("\nEntity Register done with succeed ID : " +(newEntity as BaseGameEntity_CH3).ID());

    }

    public FieldPlayer GetEntityFromID(int id)
    {
        return m_EntityMap[id];
    }

    public void RemoveEntity(FieldPlayer pEntity)
    {
        m_EntityMap.Remove(pEntity.ID());
    }

}
