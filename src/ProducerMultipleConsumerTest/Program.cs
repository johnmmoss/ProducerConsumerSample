using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ProducerMultipleConsumerTest
{
    /// <summary>
    /// Sample code from http://stackoverflow.com/questions/1656404/c-sharp-producer-consumer
    /// </summary>
    class Program
    {
        static void Main()
        {
            var queue = new SimpleQueue();

            // Consumer class starts three consumer threads
            var consumers = new Consumers(queue, 10);

            // Then we start a producer in a fourth thread
            // Producer produces 30 objects then a null to finish.
            var producer = new Producer(queue, 100);
            var producerThread = new Thread(producer.Produce);
            producerThread.Name = "Producer";
            
            consumers.Start();
            producerThread.Start();
            
            // We wait for the producer to finish - we know it is finite. 
            producerThread.Join();
            // When the producer stops, we signal 
            consumers.Stop();

            Console.WriteLine("Done...");
            Console.ReadLine();
        }
    }
}
