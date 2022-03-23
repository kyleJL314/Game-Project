using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

public class JobManager : MonoBehaviour
{
    public TaskQueue tasks = new TaskQueue();
    Thread[] workerTreads = new Thread[20];
    void empty()
    {

    }
    void Start()
    {
        for (int i = 0; i < workerTreads.Length; i++)
        {
            workerTreads[i] = new Thread(empty);
            workerTreads[i].Start();
        }
    }
    void FixedUpdate()
    {
        checkForTasks();
    }
    void checkForTasks()
    {
        for (int i = 0; i < workerTreads.Length; i++)
        {
            if (!workerTreads[i].IsAlive && tasks.Count() != 0)
            {
                workerTreads[i] = new Thread(doTask);
                workerTreads[i].Start();
            }
        }
    }
    void doTask()
    {

        Itask task;

        if (tasks.Count() == 0)
        {
            return;
        }
        task = tasks.Dequeue();

        if (task != null)
        {
            task.start();
        }
    }

}
