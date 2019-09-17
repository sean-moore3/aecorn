using System.Threading;

namespace NationalInstruments.Aecorn.Threading
{
    public class ResetEventToken
    {
        internal readonly ManualResetEventSlim resetEvent;

        internal ResetEventToken()
        {
            resetEvent = new ManualResetEventSlim();
        }
    }
}
