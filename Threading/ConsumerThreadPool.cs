using System;
using System.Collections.Generic;
using System.Collections.Concurrent;

namespace NationalInstruments.Aecorn.Threading
{
    public class ConsumerThreadPool : IDisposable
    {
        private List<ConsumerThread> consumerThreads = new List<ConsumerThread>();
        private BlockingCollection<Action> sharedTaskQueue = new BlockingCollection<Action>(new ConcurrentQueue<Action>());

        /// <summary>
        /// Creates a new consumer thread pool with the specified number of threads.
        /// </summary>
        /// <param name="numThreads">Number of <see cref="ConsumerThread"/> to spawn.</param>
        public ConsumerThreadPool(int numThreads)
        {
            for (int i = 0; i < numThreads; i++)
                consumerThreads.Add(new ConsumerThread(sharedTaskQueue));
        }

        /// <summary>
        /// Enqueues a task to be executed by the thread pool.
        /// </summary>
        /// <param name="task"></param>
        public void Enqueue(Action task)
        {
            sharedTaskQueue.Add(task);
        }

        /// <summary>
        /// Disposes of all <see cref="ConsumerThread"/> in the thread pool.
        /// </summary>
        public void Dispose()
        {
            foreach (ConsumerThread consumerThread in consumerThreads)
                consumerThread.Dispose();
        }

        ~ConsumerThreadPool()
        {
            Dispose();
        }
    }
}
