using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BlockingCollectionSample
{
    /// <summary>
    /// Sample code from https://msdn.microsoft.com/en-us/library/dd997306(v=vs.110).aspx
    /// </summary>
    public class Program
    {
        static int inputs = 2000;

        static void Main()
        {
            Console.SetBufferSize(80, 2000);
            
            var blockingCollection = new BlockingCollection<int>(100); // Bounded to 100
            var consumerTask = Task.Run(() => NonBlockingConsumer(blockingCollection));
            var producerTask = Task.Run(() => NonBlockingProducer(blockingCollection));

            Task.WaitAll(consumerTask, producerTask);

            Console.WriteLine("Press the Enter key to exit.");
            Console.ReadLine();
        }

        static void NonBlockingConsumer(BlockingCollection<int> collection)
        {
            
            while (!collection.IsCompleted) // IsCompleted == (IsAddingCompleted && Count == 0)
            {
                int nextItem = 0;

                if (!collection.TryTake(out nextItem, 0))
                {
                    Console.WriteLine(" Take Blocked");
                }
                else
                    Console.WriteLine(" Take:{0}", nextItem);

                // Slow down consumer just a little to cause
                // collection to fill up faster, and lead to "AddBlocked"
                Thread.SpinWait(500000);
            }

            Console.WriteLine("\r\nNo more items to take.");
        }

        static void NonBlockingProducer(BlockingCollection<int> bc)
        {
            int itemToAdd = 0;
            bool success = false;

            do
            {
                // A shorter timeout causes more failures.
                success = bc.TryAdd(itemToAdd, 2);

                if (success)
                {
                    Console.WriteLine(" Add:{0}", itemToAdd);
                    itemToAdd++;
                }
                else
                {
                    Console.Write(" AddBlocked:{0} Count = {1} ", itemToAdd.ToString(), bc.Count);
                    // Don't increment nextItem. Try again on next iteration.

                    //Do something else useful instead.
                    UpdateProgress(itemToAdd);
                }

            } while (itemToAdd < inputs);

            // No lock required here because only one producer.
            bc.CompleteAdding();
        }

        static void UpdateProgress(int i)
        {
            double percent = ((double)i / inputs) * 100;
            Console.WriteLine("Percent complete: {0}", percent);
        }
    }

    public class Data
    {
        
    }
}
