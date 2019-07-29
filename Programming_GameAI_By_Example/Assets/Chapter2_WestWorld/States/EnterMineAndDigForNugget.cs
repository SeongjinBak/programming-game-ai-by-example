using UnityEngine;

public class EnterMineAndDigForNugget : State<Miner>
{
    // Singleton design pattern implementation.
    public static EnterMineAndDigForNugget instance = null;
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if(instance != this)
        {
            Destroy(this.gameObject);
        }
        DontDestroyOnLoad(this.gameObject);
    }

    public override void Enter(Miner miner)
    {
        //If the miner isn't be in the Mine, the miner should change his location to go to the Mine.
        if(miner.m_location != BaseGameEntity.LocationType.GoldMine)
        {
            Debug.Log("\n" + miner.GetNameOfEntity() + ": " + "Walking to the gold mine!");
            miner.ChangeLocation(BaseGameEntity.LocationType.GoldMine);
        }
    }

    public override void Execute(Miner miner)
    {
        // The miner will dig the mine until his carrying exceeded a MaxNuggets 
        // When he feel thirsty while digging, he stop working and go to the Bar to drink a whiskey.
        // Change the state for he to go to the bar.
        miner.AddToGoldCarried(1);

        // Digging is hard work to do.
        miner.IncreaseFatigue();

        Debug.Log("\n" + miner.GetNameOfEntity() + ": " + "Picking up a nugget : " + miner.GetNuggets() + " / " + miner.GetLimitNugget());

        // If he got a enough nuggets, go to the bank and make a deposit.
        if (miner.PocketsFull())
        {
            Debug.Log("소지량 한도 초과!\n");
            miner.GetFSM().ChangeState(VisitBankAndDepositGold.instance);
        }

        // If he feels thirsty, go to the bar and drink whisky.
        if (miner.Thirsty())
        {
            Debug.Log("목마름 한도 초과!\n");
            miner.GetFSM().ChangeState(QuenchThirst.instance);
        }

        // if miner feels a bunch of fatigue, go to home to take a sleep.
        if (miner.Fatigue())
        {
            Debug.Log("피로 한도 초과!\n");
            miner.GetFSM().ChangeState(GoHomeAndSleepTilRested.instance);
        }

    }

    public override void Exit(Miner miner)
    {
        Debug.Log("\n" + miner.GetNameOfEntity() + ": " + "I am leaving the gold mine with my pockets full, oh sweet gold");
    }


}
