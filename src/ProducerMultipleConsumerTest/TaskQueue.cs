﻿using System;
using System.Collections;
using System.Threading;

namespace ProducerMultipleConsumerTest
{
    /// <summary>
    /// Sample code from http://stackoverflow.com/questions/1656404/c-sharp-producer-consumer
    /// </summary>
    public class TaskQueue<T> : IDisposable where T : class
    {
        object locker = new object();
        Thread[] workers;
        Queue taskQ = new Queue();

        public TaskQueue(int workerCount)
        {
            workers = new Thread[workerCount];

            // Create and start a separate thread for each worker
            for (int i = 0; i < workerCount; i++)
                (workers[i] = new Thread(Consume)).Start();
        }

        public void Dispose()
        {
            // Enqueue one null task per worker to make each exit.
            foreach (Thread worker in workers) EnqueueTask(null);
            foreach (Thread worker in workers) worker.Join();
        }

        public void EnqueueTask(T task)
        {
            lock (locker)
            {
                taskQ.Enqueue(task);
                Monitor.PulseAll(locker);
            }
        }

        void Consume()
        {
            while (true)
            {
                object task;
                lock (locker)
                {
                    while (taskQ.Count == 0) Monitor.Wait(locker);
                    task = taskQ.Dequeue();
                }
                if (task == null) return;         // This signals our exit
                Console.Write(task);
                Thread.Sleep(1000);              // Simulate time-consuming task
            }
        }
    }
}