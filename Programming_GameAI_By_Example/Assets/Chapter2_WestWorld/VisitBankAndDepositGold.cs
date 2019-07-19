using UnityEngine;

public class VisitBankAndDepositGold : State<Miner>
{
    // Singleton design pattern implementation.
    public static VisitBankAndDepositGold instance = null;
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
        //If the miner isn't be in the Mine, the miner should change his location to go to the Mine.
        if (miner.m_location != BaseGameEntity.LocationType.Bank)
        {
            Debug.Log("\n" + miner.GetNameOfEntity() + ": " + "Visiting to the Bank for deposit!");
            miner.ChangeLocation(BaseGameEntity.LocationType.Bank);
        }
    }

    public override void Execute(Miner miner)
    {
        // Save all the nuggets at the Bank
        Bank.instance.SaveDeposit(miner.GetNuggets(), miner);
        miner.SetNuggets(0);
        
        Debug.Log("\n" + miner.GetNameOfEntity() + ": " + "Depositing a nugget, Deposit : " + Bank.instance.GetDeposit());

        // If he got a enough nuggets, go to the bank and make a deposit.
        if (miner.GetNuggets() <= 0)
        {
            Debug.Log("예금 완료!\n");
            miner.GetFSM().ChangeState(EnterMineAndDigForNugget.instance);
        }

    }

    public override void Exit(Miner miner)
    {
        Debug.Log("\n" + miner.GetNameOfEntity() + ": " + "I am leaving the Bank with my pockets empty, I have to go to the Mine");
    }


}
