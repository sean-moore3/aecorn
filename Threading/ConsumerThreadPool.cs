using System.Collections.Concurrent;

namespace NationalInstruments.Aecorn.Threading
{
    public class ConsumerThreadPool
    {
        private ConsumerThread[] consumerThreads;

        // derived attributes
        public int ThreadCount
        {
            get { return consumerThreads.Length; }
        }

        /// <summary>
        /// Creates a new consumer thread pool with the specified number of threads.
        /// </summary>
        /// <param name="numThreads">Number of <see cref="ConsumerThread"/> to spawn.</param>
        public ConsumerThreadPool(int numThreads)
        {
            consumerThreads = new ConsumerThread[numThreads];
            var sharedCallbackQueue = new BlockingCollection<ICallable>(new ConcurrentQueue<ICallable>());
            for (int i = 0; i < numThreads; i++)
                consumerThreads[i] = new ConsumerThread(sharedCallbackQueue);
        }

        /// <summary>
        /// Enqueues a task to be executed by the thread pool.
        /// </summary>
        /// <param name="callback"></param>
        public void Enqueue(ICallable callback)
        {
            consumerThreads[0].Enqueue(callback); // since the queues are shared we can enqueue onto any thread
        }

        /// <summary>
        /// Disposes of all <see cref="ConsumerThread"/> in the thread pool.
        /// </summary>
        public void Close()
        {
            foreach (ConsumerThread consumerThread in consumerThreads)
                consumerThread.Stop();
        }

        ~ConsumerThreadPool()
        {
            Close();
        }
    }
}
