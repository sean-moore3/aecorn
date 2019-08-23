using System.Collections.Concurrent;
using System.Threading;

namespace NationalInstruments.Aecorn.Threading
{
    public class ConsumerThread
    {
        private readonly CancellationTokenSource threadCancellationTokenSource = new CancellationTokenSource();
        private readonly CancellationToken threadCancellationToken;

        private readonly Thread consumerThread;
        private readonly BlockingCollection<ICallable> callbackQueue;
        
        private readonly ManualResetEventSlim threadStartedEvent = new ManualResetEventSlim();

        /// <summary>
        /// Creates a new consumer thread that uses the specified <see cref="ICallable"/> blocking collection.
        /// </summary>
        public ConsumerThread(BlockingCollection<ICallable> callbackQueue)
        {
            threadCancellationToken = threadCancellationTokenSource.Token;
            this.callbackQueue = callbackQueue;

            consumerThread = new Thread(new ThreadStart(Consumer));
            consumerThread.Start();

            Enqueue(Callback.New((startedEvent) => startedEvent.Set(), threadStartedEvent));
        }

        /// <summary>
        /// Requests a new thread from the operating system.
        /// This method may return before the thread is ready to begin executing callbacks.
        /// Call <see cref="WaitUntiThreadStart(int)"/> to wait until the thread has completed startup.
        /// </summary>
        public ConsumerThread() : this(new BlockingCollection<ICallable>(new ConcurrentQueue<ICallable>())) { }

        /// <summary>
        /// Blocks until the thread is ready to start executing callbacks.
        /// </summary>
        /// <param name="millisecondsTimeout">Time to wait before timing out.</param>
        public void WaitUntiThreadStart(int millisecondsTimeout = -1)
        {
            threadStartedEvent.Wait(millisecondsTimeout);
        }

        /// <summary>
        /// Enqueues a <see cref="ICallable"/> onto the consumer thread's callback queue.
        /// </summary>
        public void Enqueue(ICallable callback)
        {
            callbackQueue.Add(callback);
        }

        /// <summary>
        /// Waits for all current callbacks in the thread's callback queue to execute before returning.
        /// </summary>
        /// <param name="millisecondsTimeout">Amount of time to wait before timing out.</param>
        public void Wait(int millisecondsTimeout = -1)
        {
            ManualResetEventSlim resetEvent = new ManualResetEventSlim(false);
            Enqueue(Callback.New((manualResetEvent) => manualResetEvent.Set(), resetEvent));
            resetEvent.Wait(millisecondsTimeout);
        }

        /// <summary>
        /// Waits for all current callbacks in the thread's callback queue to execute then gracefully stops the thread.
        /// </summary>
        public void Join()
        {
            Enqueue(new Callback(Stop));
            consumerThread.Join();
        }

        /// <summary>
        /// Gracefully stops the consumer thread.
        /// </summary>
        public void Stop()
        {
            threadCancellationTokenSource.Cancel();
        }

        /// <summary>
        /// Immediately shuts down the thread.
        /// Any code that is currently executing will be interrupted.
        /// </summary>
        public void Kill()
        {
            consumerThread.Abort();
        }

        private void Consumer()
        {
            try
            {
                while (!threadCancellationToken.IsCancellationRequested)
                    if (callbackQueue.TryTake(out ICallable callback, -1, threadCancellationToken))
                        callback.Call();
            }
            catch (ThreadAbortException)
            {
                // placeholder
            }
        }

        ~ConsumerThread()
        {
            Join();
        }
    }
}
