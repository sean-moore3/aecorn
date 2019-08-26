/// <summary>
/// Expiring callbacks are useful when managing multiple consumer threads with distributed work queues.
/// If a callback is needed that can be executed on any thread,
/// simply enqueue an expiring callback onto each queue you would like 
/// </summary>

namespace NationalInstruments.Aecorn.Threading
{
    /// <summary>
    /// Imposes a limit on the number of times a callback can be invoked.
    /// Callbacks that have reached the expired state noop on <see cref="Call"/>.
    /// </summary>
    public class ExpiringCallback : ICallable
    {
        private readonly ICallable callback;
        private readonly Box<int> expireCount;

        /// <summary>
        /// Creates a new expiring callback that can only be called once.
        /// </summary>
        public ExpiringCallback(ICallable callback)
        {
            this.callback = callback;
            expireCount = new Box<int>(1);
        }

        /// <summary>
        /// Creates a new expiring callback that can only be called a specific number of times.
        /// </summary>
        /// <param name="expireCount">Amount of times the callback can be called before it expires.</param>
        public ExpiringCallback(ICallable callback, int expireCount) : this(callback)
        {
            this.expireCount.item1 = expireCount;
        }

        /// <summary>
        /// Invokes the callback if it hasn't expired.
        /// </summary>
        public void Call()
        {
            bool notExpired;
            lock (expireCount)
            {
                notExpired = expireCount.item1 > 0;
                if (notExpired)
                    expireCount.item1--;
            }
            if (notExpired)
                callback.Call();
        }
    }
}
