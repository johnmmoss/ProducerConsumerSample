using System;
using System.Threading;

namespace ProducerConsumerTest
{
    /// <summary>
    /// Sample code from http://stackoverflow.com/questions/1656404/c-sharp-producer-consumer
    /// </summary>
    public class Test
    {
        static SimpleQueue queue;

        static void Main()
        {
            queue = new SimpleQueue();

            var consumer = new Consumer(queue);
            var producer = new Producer(queue);

            var consumerThread = new Thread(consumer.Consume);
            var producerThread = new Thread(producer.Produce);

            consumerThread.Start();
            producerThread.Start();

            consumerThread.Join();
            producerThread.Join();

            Console.ReadLine();
        }
    }
}
