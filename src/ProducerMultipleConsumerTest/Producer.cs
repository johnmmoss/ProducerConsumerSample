using System;
using System.Threading;

namespace ProducerMultipleConsumerTest
{
    public class Producer
    {
        private readonly SimpleQueue queue;
        private readonly int max;

        public Producer(SimpleQueue queue, int max)
        {
            this.queue = queue;
            this.max = max;
        }

        public void Produce()
        {
            var random = new Random(3);
            for (int i = 0; i < max; i++)
            {
                Console.WriteLine("Producing {0}", i);
                queue.Produce(i);
                Thread.Sleep(random.Next(100));
            }
        }
    }
}