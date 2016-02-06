using System;
using System.Threading;

namespace ProducerConsumerTest
{
    /// <summary>
    /// Sample code from http://stackoverflow.com/questions/1656404/c-sharp-producer-consumer
    /// </summary>
    public class Producer
    {
        private readonly SimpleQueue queue;

        public Producer(SimpleQueue queue)
        {
            this.queue = queue;
        }

        public void Produce()
        {
            var random = new Random(0);
            for (int i = 0; i < 10; i++)
            {
                Console.WriteLine("Producing {0}", i);
                queue.Produce(i);
                Thread.Sleep(random.Next(1000));
            }
        }
    }
}