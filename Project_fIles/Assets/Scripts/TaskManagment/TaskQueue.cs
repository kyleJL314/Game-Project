using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskQueue
{
    LinkedList<Itask> tasks = new LinkedList<Itask>();

    public Itask Dequeue()
    {
        int smallestPriority = int.MaxValue;
        Itask mostImportant=null;
        lock (tasks)
        {
            foreach (Itask task in tasks)
            {
                int priority = task.priority();
                if (priority < smallestPriority)
                {
                    if (task.ready())
                    {
                        mostImportant = task;
                        smallestPriority = priority;
                    }
                }

            }
        }
        if(mostImportant != null)
        {
            lock (tasks)
            {
                tasks.Remove(mostImportant);
            }
        }
        return mostImportant;
    }
    public void Enqueue(Itask task)
    {
        lock (tasks)
        {
            if (!tasks.Contains(task))
            {
             
                tasks.AddFirst(task);
                
            }
        }
    }
    public int Count()
    {
        return tasks.Count;
    }
}
