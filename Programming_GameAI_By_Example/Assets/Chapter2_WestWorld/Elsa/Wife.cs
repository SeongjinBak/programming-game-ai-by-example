using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wife : BaseGameEntity
{
    [SerializeField]
    // Instance of FSM
    StateMachine<Wife> m_pStateMachine;

    [SerializeField]
    // A Position of this gameObject located.
    private LocationType m_Location;
    // A Position properety
    public LocationType m_location
    {
        get { return m_Location; }
        set { m_Location = m_location; }
    }
    [SerializeField]
    private bool cooking;
    public bool Cooking
    {
        get => cooking;
        set => cooking = value;
    }
    private void Start()
    {
        SetID(GameManager.instance.entityID, "Wife Elsa_" + GameManager.instance.entityID++);
        
        
        // When this Entity is created, registered to Entity Manager's Dictionary.
        // We can also Use GetEntityFromId Function in EntityManager.cs :
        // Debug.Log( EntityManager.instance.GetEntityFromID(GetIDOfEntity()));
        EntityManager.instance.RegisterEntity(GetComponent<Wife>());

        m_location = LocationType.Nothing;

        Cooking = false;

        // Set the FSM
        m_pStateMachine = new StateMachine<Wife>(this);
        m_pStateMachine.SetCurrentState(WifeGlobalState.instance);
      //  m_pStateMachine.SetCurrentState(EnterMineAndDigForNugget.instance);
     //   m_pStateMachine.SetGlobalState(MinerGlobalState.instance);



        //ChangeState(EnterMineAndDigForNugget.instance);
        StartCoroutine(Updating());

    }

    public override bool HandleMessage(Telegram msg)
    {
        return m_pStateMachine.HandleMessage(msg);
        
    }



    // Revert to previous state
    // It is for "State Blip"
    public void RevertToPreviousState()
    {

    }


    // Chage this miner's location to Argument location.
    public void ChangeLocation(LocationType type)
    {
        Debug.Log("\n" + GetNameOfEntity() + " Location Changed: " + type);
        m_Location = type;
    }

  
    protected override IEnumerator Updating()
    {
        while (true)
        {
            m_pStateMachine.Updating();

            yield return new WaitForSeconds(0.5f);
        }
    }

    public StateMachine<Wife> GetFSM()
    {
        return m_pStateMachine;
    }
}
