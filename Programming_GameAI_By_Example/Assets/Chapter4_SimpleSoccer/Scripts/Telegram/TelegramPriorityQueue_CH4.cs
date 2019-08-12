using System.Collections.Generic;
using UnityEngine;
public class TelegramPriorityQueue_CH4
{

    private List<Telegram_CH4> priorityQueue = new List<Telegram_CH4>();



    public void Enqueue(Telegram_CH4 telegram)
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

    public Telegram_CH4 Peek()
    {
        if (priorityQueue.Count > 0)
        {
            return priorityQueue[0];
        }
        else
        {
            Telegram_CH4 dummy = new Telegram_CH4();
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
