using System;
using System.Collections.Concurrent;
using System.Threading;

namespace NationalInstruments.Aecorn.Threading
{
    class ConsumerThread : IDisposable
    {
        private readonly Thread consumer;
        private readonly BlockingCollection<Action> taskQueue;
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private readonly CancellationToken cancellationToken;

        /// <summary>
        /// Creates a new consumer thread that uses the specified task queue.
        /// </summary>
        /// <param name="taskQueue">Task queue for the consumer thread to use.</param>
        public ConsumerThread(BlockingCollection<Action> taskQueue)
        {
            consumer = new Thread(new ThreadStart(Consumer));
            this.taskQueue = taskQueue;
            consumer.Start();
            cancellationToken = cancellationTokenSource.Token;
        }

        /// <summary>
        /// Creates a new consumer thread.
        /// </summary>
        public ConsumerThread() : this(new BlockingCollection<Action>(new ConcurrentQueue<Action>())) { }

        /// <summary>
        /// Enqueues a task onto the consumer thread's task queue.
        /// </summary>
        /// <param name="task"></param>
        public void Enqueue(Action task)
        {
            taskQueue.Add(task);
        }

        /// <summary>
        /// Closes the consumer thread.
        /// </summary>
        public void Dispose()
        {
            cancellationTokenSource.Cancel();
        }

        private void Consumer()
        {
            while (!cancellationToken.IsCancellationRequested)
                if (taskQueue.TryTake(out Action task, -1, cancellationToken))
                    task();
        }

        ~ConsumerThread()
        {
            Dispose();
        }
    }
}
