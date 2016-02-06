using System;
using System.Linq;
using System.Threading;

namespace ProducerMultipleConsumerTest
{
    public class Consumers
    {
        private readonly SimpleQueue queue;
        private readonly Thread[] threads;
        Random random;

        public Consumers(SimpleQueue queue, int count)
        {
            this.queue = queue;
            threads = new Thread[count];
            random = new Random(100);
        }

        public void Start()
        {
            for (int i = 0; i < threads.Count(); i++)
            {
                var thread = new Thread(Consume);
                thread.Name = "Consumer " + i;
                threads[i] = thread;
            }
            
            foreach (var thread in threads)
            {
                thread.Start();
            }
        }

        public void Stop()
        {
            // Signal each thread to stop
            for (int i = 0; i < threads.Length; i++)
            {
                queue.Produce(null);
            }

            // Wait for each thread to stop
            foreach (var thread in threads)
            {
                thread.Join();
            }
        }

        private void Consume()
        {
            for (;;)
            {
                object o = queue.Consume();

                if (o == null)
                {
                    Console.WriteLine("{0} recieved a null... stopping consumer", Thread.CurrentThread.Name);
                    break;
                }

                Console.WriteLine("\t\t\t\t[{0}] Consuming {1}", Thread.CurrentThread.Name, o);
                Thread.Sleep(random.Next(1000));
            }
        }
    }
}