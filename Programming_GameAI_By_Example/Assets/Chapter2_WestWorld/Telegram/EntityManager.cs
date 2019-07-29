using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityManager : MonoBehaviour
{
    // Singleton design pattern implementation.
    public static EntityManager instance = null;
    [SerializeField]
    private Dictionary<int, BaseGameEntity> m_EntityMap;
    
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
        {
            Destroy(this.gameObject);
        }
        DontDestroyOnLoad(this.gameObject);

        m_EntityMap = new Dictionary<int, BaseGameEntity>();
    }

    // Same as ' = ' operator
    public EntityManager IsEqual(EntityManager em)
    {
        return em;
    }
    
    public void RegisterEntity(BaseGameEntity newEntity)
    {
        
        m_EntityMap.Add(newEntity.GetIDOfEntity(), newEntity);
        Debug.Log("\nEntity Register done with succeed");

    }

    public BaseGameEntity GetEntityFromID(int id)
    {
        return m_EntityMap[id];
    }

    public void RemoveEntity(BaseGameEntity pEntity)
    {
        m_EntityMap.Remove(pEntity.GetIDOfEntity());
    }


    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
       
    }
}
