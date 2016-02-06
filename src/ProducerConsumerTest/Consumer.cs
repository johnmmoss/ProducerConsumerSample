using System;
using System.Threading;

namespace ProducerConsumerTest
{
    /// <summary>
    /// Sample code from http://stackoverflow.com/questions/1656404/c-sharp-producer-consumer
    /// </summary>
    public class Consumer
    {
        private readonly SimpleQueue queue;

        public Consumer(SimpleQueue queue)
        {
            this.queue = queue;
        }

        public void Consume()
        {
            // Make sure we get a different random seed from the
            // first thread
            var random = new Random(100);
            // We happen to know we've only got 10 
            // items to receive
            for (int i = 0; i < 10; i++)
            {
                object o = queue.Consume();

                Console.WriteLine("\t\t\t\tConsuming {0}", o);
                Thread.Sleep(random.Next(1000));
            }
        }
    }
}