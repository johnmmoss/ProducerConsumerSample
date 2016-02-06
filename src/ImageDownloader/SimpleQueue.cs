using System;
using System.Collections.Generic;
using System.Threading;

namespace WebImageDownloader.Console
{
    /// <summary>
    /// Threadsafe queue
    /// </summary>
    public class SimpleQueue
    {
        private readonly object listLock = new object();
        private readonly Queue<Uri> queue = new Queue<Uri>();

        public int Count
        {
            get { return queue.Count; }
        }

        public void Enqueue(Uri o)
        {
            lock (listLock)
            {
                queue.Enqueue(o);

                Monitor.Pulse(listLock);
            }
        }

        public Uri Dequeue()
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
