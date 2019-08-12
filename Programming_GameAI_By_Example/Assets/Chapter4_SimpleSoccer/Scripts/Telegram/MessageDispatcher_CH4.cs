using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MessageDispatcher_CH4 : MonoBehaviour
{
    // Singleton design pattern implementation.
    public static MessageDispatcher_CH4 instance = null;
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
        //messageType.Add("Msg_HiHoneyImHome", 0);
        //messageType.Add("Msg_StewReady", 1);
        StartCoroutine(Updating());

    }
    


    IEnumerator Updating()
    {
        WaitForSeconds ws = new WaitForSeconds(0.05f);
        while (true)
        {
            DispatchDelayedMessages();
            yield return ws;
        }
    }

    [SerializeField]
    private TelegramPriorityQueue_CH4 priorityQ = new TelegramPriorityQueue_CH4();
    public Dictionary<string, int> messageType = new Dictionary<string, int>();

    private void Discharge(FieldPlayer pReceiver, Telegram_CH4 msg = null)
    {
        if (msg == null || !pReceiver.HandleMessage(msg))
        {
            Debug.Log("Empty Msg Detected");
        }
    }
    
    


    public void DispatchMessage(float delay, int sender, int receiver, int msg, Transform info = null)
    {
        Telegram_CH4 telegram = new Telegram_CH4();
        telegram.ConstructTelegram(delay, sender, receiver, msg, info);

        if(delay <= 0.0f)
        {
            Discharge(EntityManager_CH4.instance.GetEntityFromID(receiver), telegram);
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
        if(priorityQ.Count() > 0)
        {
            while ((priorityQ.Peek().DispatchTime < currentTime) && (priorityQ.Peek().DispatchTime > 0))
            {
                // Get Front of the queue.
                Telegram_CH4 telegram = priorityQ.Peek();
                // Find Receiver.
                FieldPlayer pReceiver = EntityManager_CH4.instance.GetEntityFromID(telegram.Receiver);
                // Send Telegram to recevier
                Discharge(pReceiver, telegram);
                // Pop the telegram from queue.
                priorityQ.Dequeue();
                if(priorityQ.Count() == 0)
                {
                    break;
                }
            }
        }
        
    }
   
}
