using System.Collections;
using System.Threading;

namespace ProducerConsumerTest
{
    /// <summary>
    /// Sample code from http://stackoverflow.com/questions/1656404/c-sharp-producer-consumer
    /// </summary>
    public class SimpleQueue
    {
        readonly object listLock = new object();
        readonly Queue queue = new Queue();

        public void Produce(object o)
        {
            lock (listLock)
            {
                queue.Enqueue(o);

                Monitor.Pulse(listLock);
            }
        }

        public object Consume()
        {
            lock (listLock)
            {
                while (queue.Count == 0)
                {
                    Monitor.Wait(listLock);
                }
                return queue.Dequeue();
            }
        }
    }
}