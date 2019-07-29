using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MessageDispatcher : MonoBehaviour
{
    // Singleton design pattern implementation.
    public static MessageDispatcher instance = null;
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
    private void Start()
    {
        messageType.Add("Msg_HiHoneyImHome", 0);
        messageType.Add("Msg_StewReady", 1);

        StartCoroutine(Updating());

    }

    IEnumerator Updating()
    {
        WaitForSeconds ws = new WaitForSeconds(0.5f);
        while (true)
        {
            DispatchDelayedMessages();
            yield return ws;
        }
    }

    [SerializeField]
    private Queue<Telegram> priorityQ = new Queue<Telegram>();
    public Dictionary<string, int> messageType = new Dictionary<string, int>();

    private void Discharge(BaseGameEntity pReceiver, Telegram msg = null)
    {
        if (msg == null || !pReceiver.HandleMessage(msg))
        {
            Debug.Log("Empty Msg Detected");
        }
    }
    
    


    public void DispatchMessage(float delay, int sender, int receiver, int msg, Telegram.ExtraInfo info = null)
    {
        Telegram telegram = new Telegram();
        telegram.ConstructTelegram(delay, sender, receiver, msg, info);

        if(delay <= 0.0f)
        {
            Discharge(EntityManager.instance.GetEntityFromID(receiver), telegram);
        }
        else
        {
            float currentTime = Time.time;
            telegram.DispatchTime = currentTime + delay;

            // Input telegram into pq.
            priorityQ.Enqueue(telegram);

           
        }
    }

    // This method should be called from Main loop of the Game.
    public void DispatchDelayedMessages()
    {
        // Get a current time.
        double currentTime = Time.time;
        if(priorityQ.Count > 0)
        {
            while ((priorityQ.Peek().DispatchTime < currentTime) && (priorityQ.Peek().DispatchTime > 0))
            {
                // Get Front of the queue.
                Telegram telegram = priorityQ.Peek();
                // Find Receiver.
                BaseGameEntity pReceiver = EntityManager.instance.GetEntityFromID(telegram.Receiver);
                // Send Telegram to recevier
                Discharge(pReceiver, telegram);
                // Pop the telegram from queue.
                priorityQ.Dequeue();
                if(priorityQ.Count == 0)
                {
                    break;
                }
            }
        }
        
    }
   
}
