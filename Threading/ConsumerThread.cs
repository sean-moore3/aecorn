using System;
using System.Collections.Concurrent;
using System.Threading;

namespace NationalInstruments.Aecorn.Threading
{
    public class ConsumerThread
    {
        private readonly CancellationTokenSource threadCancellationTokenSource = new CancellationTokenSource();
        private readonly CancellationToken threadCancellationToken;

        private readonly Thread consumerThread;
        private readonly BlockingCollection<Callback> callbackQueue;
        
        private readonly ManualResetEventSlim threadStartedEvent = new ManualResetEventSlim();
        private readonly ManualResetEventSlim threadSleepEvent = new ManualResetEventSlim(true);

        internal ConsumerThread(BlockingCollection<Callback> callbackQueue)
        {
            threadCancellationToken = threadCancellationTokenSource.Token;
            this.callbackQueue = callbackQueue;

            consumerThread = new Thread(new ThreadStart(Consumer));
            consumerThread.Start();

            EnqueueAction((startedEvent) => startedEvent.Set(), threadStartedEvent);
        }

        /// <summary>
        /// Requests a new thread from the operating system.
        /// This method may return before the thread is ready to begin executing callbacks.
        /// Call <see cref="WaitUntilThreadStart()"/> to wait until the thread has completed startup.
        /// </summary>
        public ConsumerThread() : this(new BlockingCollection<Callback>(new ConcurrentQueue<Callback>())) { }

        #region Queue Methods
        /// <summary>
        /// For private use only.
        /// </summary>
        private void Enqueue(Callback callback)
        {
            callbackQueue.Add(callback);
        }

        /// <summary>
        /// Enqueues an <see cref="Action"/> and its parameters onto the consumer thread's queue.
        /// </summary>
        public void EnqueueAction(Action action)
        {
            Enqueue(Callback.New(action));
        }

        /// <summary>
        /// Enqueues an <see cref="Action{T}"/> and its parameters onto the consumer thread's queue.
        /// </summary>
        public void EnqueueAction<T>(Action<T> action, T param1)
        {
            Enqueue(Callback.New(action, param1));
        }

        /// <summary>
        /// Enqueues an <see cref="Action{T1, T2}"/> and its parameters onto the consumer thread's queue.
        /// </summary>
        public void EnqueueAction<T1, T2>(Action<T1, T2> action, T1 param1, T2 param2)
        {
            Enqueue(Callback.New(action, param1, param2));
        }

        /// <summary>
        /// Enqueues an <see cref="Action{T1, T2, T3}"/> and its parameters onto the consumer thread's queue.
        /// </summary>
        public void EnqueueAction<T1, T2, T3>(Action<T1, T2, T3> action, T1 param1, T2 param2, T3 param3)
        {
            Enqueue(Callback.New(action, param1, param2, param3));
        }

        /// <summary>
        /// Enqueues an <see cref="Action{T1, T2, T3, T4}"/> and its parameters onto the consumer thread's queue.
        /// </summary>
        public void EnqueueAction<T1, T2, T3, T4>(Action<T1, T2, T3, T4> action, T1 param1, T2 param2, T3 param3, T4 param4)
        {
            Enqueue(Callback.New(action, param1, param2, param3, param4));
        }

        /// <summary>
        /// Enqueues an <see cref="Action{T1, T2, T3, T4, T5}"/> and its parameters onto the consumer thread's queue.
        /// </summary>
        public void EnqueueAction<T1, T2, T3, T4, T5>(Action<T1, T2, T3, T4, T5> action, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5)
        {
            Enqueue(Callback.New(action, param1, param2, param3, param4, param5));
        }

        /// <summary>
        /// Enqueues an <see cref="Func{TResult}"/> and its parameters onto the consumer thread's queue.
        /// </summary>
        public FuncResultToken<TResult> EnqueueFunc<TResult>(Func<TResult> func)
        {
            var callback = Callback.New(func);
            Enqueue(callback);
            return callback.resultToken;
        }

        /// <summary>
        /// Enqueues an <see cref="Func{T1, TResult}"/> and its parameters onto the consumer thread's queue.
        /// </summary>
        public FuncResultToken<TResult> EnqueueFunc<T1, TResult>(Func<T1, TResult> func, T1 param1)
        {
            var callback = Callback.New(func, param1);
            Enqueue(callback);
            return callback.resultToken;
        }

        /// <summary>
        /// Enqueues an <see cref="Func{T1, T2, TResult}"/> and its parameters onto the consumer thread's queue.
        /// </summary>
        public FuncResultToken<TResult> EnqueueFunc<T1, T2, TResult>(Func<T1, T2, TResult> func, T1 param1, T2 param2)
        {
            var callback = Callback.New(func, param1, param2);
            Enqueue(callback);
            return callback.resultToken;
        }

        /// <summary>
        /// Enqueues an <see cref="Func{T1, T2, T3, TResult}"/> and its parameters onto the consumer thread's queue.
        /// </summary>
        public FuncResultToken<TResult> EnqueueFunc<T1, T2, T3, TResult>(Func<T1, T2, T3, TResult> func, T1 param1, T2 param2, T3 param3)
        {
            var callback = Callback.New(func, param1, param2, param3);
            Enqueue(callback);
            return callback.resultToken;
        }

        /// <summary>
        /// Enqueues an <see cref="Func{T1, T2, T3, T4, TResult}"/> and its parameters onto the consumer thread's queue.
        /// </summary>
        public FuncResultToken<TResult> EnqueueFunc<T1, T2, T3, T4, TResult>(Func<T1, T2, T3, T4, TResult> func, T1 param1, T2 param2, T3 param3, T4 param4)
        {
            var callback = Callback.New(func, param1, param2, param3, param4);
            Enqueue(callback);
            return callback.resultToken;
        }

        /// <summary>
        /// Enqueues an <see cref="Func{T1, T2, T3, T4, T5, TResult}"/> and its parameters onto the consumer thread's queue.
        /// </summary>
        public FuncResultToken<TResult> EnqueueFunc<T1, T2, T3, T4, T5, TResult>(Func<T1, T2, T3, T4, T5, TResult> func, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5)
        {
            var callback = Callback.New(func, param1, param2, param3, param4, param5);
            Enqueue(callback);
            return callback.resultToken;
        }

        /// <summary>
        /// Immediately puts the thread to sleep, removes all actions from the queue, then wakes the thread.
        /// </summary>
        public void Flush()
        {
            Sleep();
            while (callbackQueue.Count > 0)
                callbackQueue.Take();
            Wake();
        }
        #endregion

        #region Synchronization
        /// <summary>
        /// Blocks until the thread is ready to start executing actions.
        /// </summary>
        public void WaitUntilThreadStart()
        {
            threadStartedEvent.Wait();
        }

        /// <summary>
        /// Enqueues a unique <see cref="ResetEventToken"/> that can be used to block the calling thread until the <see cref="ResetEventToken"/> has been dequeued.
        /// Call <see cref="WaitOnResetEventToken(ResetEventToken)"/> to wait until the <see cref="ResetEventToken"/> has been dequeued.
        /// </summary>
        public ResetEventToken EnqueueResetEventToken()
        {
            ResetEventToken resetToken = new ResetEventToken();
            EnqueueAction((resetEvent) => resetEvent.Set(), resetToken.resetEvent);
            return resetToken;
        }

        /// <summary>
        /// Waits until the provided <see cref="ResetEventToken"/> has been dequeued.
        /// </summary>
        public void WaitOnResetEventToken(ResetEventToken resetToken)
        {
            resetToken.resetEvent.Wait();
        }

        /// <summary>
        /// Waits for all current actions in the queue to execute before returning.
        /// Encapsulates consecutive calls to <see cref="EnqueueResetEventToken"/> and <see cref="WaitOnResetEventToken(ResetEventToken)"/>.
        /// </summary>
        public void Wait()
        {
            var resetToken = EnqueueResetEventToken();
            WaitOnResetEventToken(resetToken);
        }

        /// <summary>
        /// Waits for all current actions in the queue to execute then gracefully stops the thread.
        /// This method blocks until the thread has completely shut down before returning.
        /// Call <see cref="Finish"/> instead if you do not want to wait for the thread to completely shut down before returning.
        /// </summary>
        public void Join()
        {
            if (consumerThread.IsAlive)
                EnqueueAction(Stop);
            consumerThread.Join();
        }

        /// <summary>
        /// Waits for all current actions in the thread's queue to execute then gracefully stops the thread.
        /// This method does not block until the thread has completely shut down before returning.
        /// Call <see cref="Join"/> instead if you would like to wait for the thread to completely shut down before returning.
        /// </summary>
        public void Finish()
        {
            ManualResetEventSlim resetEvent = new ManualResetEventSlim(false);
            EnqueueAction((manualResetEvent) => { manualResetEvent.Set(); Stop(); }, resetEvent);
            resetEvent.Wait();
        }

        /// <summary>
        /// Enqueues a pause action onto the consumer thread's queue.
        /// Items currently in the queue execute before the thread is paused.
        /// A unique token is returned that can be used to resume the thread with a call to <see cref="Resume(ResetEventToken)"/>
        /// </summary>
        public ResetEventToken Pause()
        {
            ResetEventToken resetToken = new ResetEventToken();
            EnqueueAction((threadPauseEvent) => threadPauseEvent.Wait(), resetToken.resetEvent);
            return resetToken;
        }

        /// <summary>
        /// Uses the provided <see cref="ResetEventToken"/> to resume the thread from the point in the queue when it was paused.
        /// </summary>
        public void Resume(ResetEventToken pauseResetToken)
        {
            pauseResetToken.resetEvent.Reset();
        }
        #endregion

        #region Thread Control
        /// <summary>
        /// Immediately puts the thread to sleep.
        /// No actions can be dequeued until a call to <see cref="Wake"/> is made.
        /// </summary>
        public void Sleep()
        {
            threadSleepEvent.Reset();
        }

        /// <summary>
        /// Wakes up a sleeping thread.
        /// </summary>
        public void Wake()
        {
            threadSleepEvent.Set();
        }

        /// <summary>
        /// Gracefully shuts down the consumer thread.
        /// Any actions remaining in the queue will not be executed unless the thread is restarted with a call to <see cref="Restart"/>.
        /// </summary>
        public void Stop()
        {
            threadCancellationTokenSource.Cancel();
        }

        /// <summary>
        /// Starts the consumer thread if it has been previously stopped.
        /// If the thread is already running, this method has no effect.
        /// </summary>
        public void Restart()
        {
            consumerThread.Start();
        }

        /// <summary>
        /// Immediately shuts down the thread.
        /// Any code that is currently executing will be interrupted.
        /// </summary>
        public void Kill()
        {
            consumerThread.Abort();
        }
        #endregion

        private void Consumer()
        {
            try
            {
                while (!threadCancellationToken.IsCancellationRequested)
                {
                    threadSleepEvent.Wait(threadCancellationToken);
                    if (callbackQueue.TryTake(out Callback callback, -1, threadCancellationToken))
                        callback.Call();
                }
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
