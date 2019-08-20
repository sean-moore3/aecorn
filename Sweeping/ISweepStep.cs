namespace NationalInstruments.Aecorn.Sweeping
{
    public interface ISweepStep
    {
        bool MoveToNextPoint();
        void ExecuteCurrentPoint();
        void Reset();
        object GetCurrentPoint();
    }
}
