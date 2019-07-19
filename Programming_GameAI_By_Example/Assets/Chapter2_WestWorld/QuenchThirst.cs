using UnityEngine;

public class QuenchThirst : State<Miner>
{
    // Singleton design pattern implementation.
    public static QuenchThirst instance = null;
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

    public override void Enter(Miner miner)
    {
        //If the miner isn't be in the Bar, the miner should change his location to go to the Bar.
        if (miner.m_location != BaseGameEntity.LocationType.Bar)
        {
            Debug.Log("\n" + miner.GetNameOfEntity() + ": " + "Walking to the Bar");
            miner.ChangeLocation(BaseGameEntity.LocationType.Bar);
        }
    }

    public override void Execute(Miner miner)
    {
        // A bottle of bodka would be sweet enough for the Minger.
        Bar.instance.OrderVodkaAndDrink(miner);
  
        Debug.Log("\n" + miner.GetNameOfEntity() + ": " + "Drinking off a Whisky : " + miner.GetThirst() + " / " + miner.GetLimitThirst());

        // If he doesnt feel thirsty, go back to the Mine.
        if (!miner.Thirsty())
        {
            Debug.Log("갈증 해소 완료!\n");
            miner.GetFSM().ChangeState(EnterMineAndDigForNugget.instance);
        }

    }

    public override void Exit(Miner miner)
    {
        Debug.Log("\n" + miner.GetNameOfEntity() + ": " + "I am leaving the Bar with my wet tongue!");
    }


}