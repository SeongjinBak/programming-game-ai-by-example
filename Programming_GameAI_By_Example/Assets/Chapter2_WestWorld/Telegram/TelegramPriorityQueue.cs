using System.Collections.Generic;
using UnityEngine;
public class TelegramPriorityQueue 
{

    private List<Telegram> priorityQueue = new List<Telegram>();



    public void Enqueue(Telegram telegram)
    {
        float delayTime = telegram.DispatchTime;
        int index = 0;
        bool isInserted = false;
        for(int iter = 0; iter < priorityQueue.Count; iter++)
        {
            if (priorityQueue[iter].DispatchTime >= delayTime)
            {
                priorityQueue.Insert(index, telegram);
                isInserted = true;
                break;
            }
        }
        if (!isInserted)
        {
            priorityQueue.Add(telegram);
        }
    }

    public Telegram Peek()
    {
        if (priorityQueue.Count > 0)
        {
            return priorityQueue[0];
        }
        else
        {
            Telegram dummy = new Telegram();
            return dummy;
        }
    }

    public void Dequeue()
    {
        if (priorityQueue.Count > 0)
        {
            priorityQueue.RemoveAt(0);
        }
    }

    public int Count()
    {
        return priorityQueue.Count;
    }
}
