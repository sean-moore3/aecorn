namespace NationalInstruments.Aecorn.Threading
{
    public class FuncResultToken<TResult> : ResetEventToken
    {
        public TResult Result { get; internal set; }

        internal FuncResultToken() { }

        public void WaitOnResult()
        {
            resetEvent.Wait();
        }
    }
}
