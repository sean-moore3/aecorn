using System.Threading;

namespace NationalInstruments.Aecorn.Threading
{
    public class ResetToken
    {
        internal readonly ManualResetEventSlim resetEvent;

        internal ResetToken(ManualResetEventSlim resetEvent)
        {
            this.resetEvent = resetEvent;
        }
    }
}
