using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Miner : BaseGameEntity
{
    [SerializeField]
    // Instance of FSM
    StateMachine<Miner> m_pStateMachine;

    /*
    [SerializeField]
    // Directing an instance of State.
    private State<Miner> m_pCurrentState;
    [SerializeField]
    private State<Miner> m_pGlobalState;
    [SerializeField]
    private State<Miner> m_pPreviousState;
    */

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
    // The maximum amount that the miner can carry in.
    private int m_iMaximumGold;
    [SerializeField]
    // The maximum amount that the miner can be patient.
    private int m_iMaximumThirsty;
    [SerializeField]
    private int m_iMaximumFatigue;
    [SerializeField]
    // An amount of gold that miner have.
    private int m_iGoldCarried;
    [SerializeField]
    // An amount of gold that saved in a bank.
    private int m_iMoneyInBank;
    [SerializeField]
    // Higher the value, higher the thirsty.
    private int m_iThirst;
    [SerializeField]
    // Higher the value, higher the fatigue.
    private int m_iFatigue;



    private void Awake()
    {
        SetID(GameManager.instance.entityID, "Miner Bob_" + GameManager.instance.entityID++);
        
    }

    private void Start()
    {
        // When this Entity is created, registered to Entity Manager's Dictionary.
        // We can also Use GetEntityFromId Function in EntityManager.cs :
        // Debug.Log( EntityManager.instance.GetEntityFromID(GetIDOfEntity()));
        EntityManager.instance.RegisterEntity(GetComponent<Miner>());

        m_location = LocationType.Nothing;
        m_iMaximumGold = 5;
        m_iMaximumThirsty = 10;
        m_iMaximumFatigue = 15;
        m_iGoldCarried = 0;
        m_iMoneyInBank = 0;
        m_iThirst = 0;
        m_iFatigue = 0;
        

        // Set the FSM
        m_pStateMachine = new StateMachine<Miner>(this);
        // m_pStateMachine.SetCurrentState(GoHomeAndSleepTilRested.instance);
        m_pStateMachine.SetCurrentState(EnterMineAndDigForNugget.instance);
        m_pStateMachine.SetGlobalState(MinerGlobalState.instance);



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


    /*
    // Change a state of Object.
    public void ChangeState(State<Miner> pNewState) {
        if(m_pCurrentState == null)
        {
            
            // Change state into new state
            m_pCurrentState = pNewState;
            m_pCurrentState.Enter(this);
        }
        // verity both state's validity before calling both states.
        else if(m_pCurrentState != null && pNewState != null)
        {
            // Exit existed state
            m_pCurrentState.Exit(this);
            // Change state into new state
            m_pCurrentState = pNewState;
            // call entry method which is new state
            m_pCurrentState.Enter(this);
        }
    }
    */

    // Constructor for BaseEntity.(Set an ID for Object)
    public Miner(int ID) : base(ID) { }

    // Chage this miner's location to Argument location.
    public void ChangeLocation(LocationType type)
    {
        Debug.Log("\n" + GetNameOfEntity() + " Location Changed: " + type);
        m_Location = type;
    }

    // Add a gold nugget to Miner's pocket.
    public void AddToGoldCarried(int amount)
    {
        m_iGoldCarried += amount;
    }

    // Increase a fatigue
    public void IncreaseFatigue()
    {
        m_iFatigue += 1;
    }
    // Decrease a fatigue
    public void DecreaseFatigue()
    {
        m_iFatigue -= 1;
    }

    // Return the Miner's current fatigue
    public int GetFatigue()
    {
        return m_iFatigue;
    }

    // Set the miner's fatigue by argument
    public void SetFatigue(int fatigue)
    {
        m_iFatigue = fatigue;
    }

    // Return the miner's current nuggets in pocket
    public int GetNuggets()
    {
        return m_iGoldCarried;
    }

    // Set the amount of the Gold in Bank
    public void SetGoldInBank(int gold)
    {
        m_iMoneyInBank = gold;
    } 

    // Set the amount of the nugget
    public void SetNuggets(int nuggets)
    {
        m_iGoldCarried = nuggets;
    }

    // Get limit for carried nugget
    public int GetLimitNugget()
    {
        return m_iMaximumGold;
    }

    // Get limit of endurable thirs
    public int GetLimitThirst()
    {
        return m_iMaximumThirsty;
    }

    // Set the amount of the Thirst.
    public void SetThirst(int thirst)
    {
        m_iThirst = thirst;
    }

    // Get the amount of the Thirst.
    public int GetThirst()
    {
        return m_iThirst;
    } 

    // return whether miner's pocket is full or not.
    public bool PocketsFull()
    {
        return (m_iGoldCarried > m_iMaximumGold ? true : false);
    }

    // return whether miner's patient is full or not.
    public bool Thirsty()
    {
        return (m_iThirst > m_iMaximumThirsty ? true : false);
    }

    // return whether miner's fatigue is full or not.
    public bool Fatigue()
    {
        return (m_iFatigue > m_iMaximumFatigue ? true : false);
    }
    // return true when fatigue is zero.
    public bool IsFatigueQuenched()
    {
        return (m_iFatigue == 0 ? true : false);
    }

    /*
    protected override IEnumerator Updating()
    {
        while (true)
        {
            // increase miner's fatigue each updating
            m_iThirst += 1;
            if (m_pCurrentState != null)
            {
                m_pCurrentState.Execute(this);
            }

            yield return new WaitForSeconds(0.5f);
        }
    }
    */
    protected override IEnumerator Updating()
    {
        while (true)
        {
            // increase miner's fatigue each updating
            m_iThirst += 1;
            m_pStateMachine.Updating();
           
            yield return new WaitForSeconds(0.5f);
        }
    }

    public StateMachine<Miner> GetFSM()
    {
        return m_pStateMachine;
    }
}
