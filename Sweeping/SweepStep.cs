using System;
using System.Collections.Generic;

namespace NationalInstruments.Aecorn.Sweeping
{
    public class SweepStep<T> : ISweepStep
    {
        private IEnumerable<T> _sweepPoints;
        private IEnumerator<T> sweepPointsEnumerator;
        /// <summary>
        /// Collection of points to sweep across.
        /// </summary>
        public IEnumerable<T> SweepPoints
        {
            get { return _sweepPoints; }
            set
            {
                _sweepPoints = value ?? throw new ArgumentNullException();
                sweepPointsEnumerator = value.GetEnumerator();
            }
        }

        private Action<T> _operation;
        /// <summary>
        /// The operation to be performed on each point in <see cref="SweepStep{T}.SweepPoints"/>.
        /// </summary>
        public Action<T> Operation
        {
            get { return _operation; }
            set { _operation = value ?? throw new ArgumentNullException(); }
        }

        private T CurrentPoint
        {
            get { return sweepPointsEnumerator.Current; }
        }

        /// <summary>
        /// Creates a new sweep step.
        /// </summary>
        /// <param name="sweepPoints">Collection of points to sweep across.</param>
        /// <param name="operation">The operation to perform at each point in the sweep step.</param>
        public SweepStep(IEnumerable<T> sweepPoints, Action<T> operation)
        {
            SweepPoints = sweepPoints;
            Operation = operation;
        }

        /// <summary>
        /// Advances the <see cref="SweepStep{T}.CurrentPoint"/> to the next point in <see cref="SweepStep{T}.SweepPoints"/>.
        /// </summary>
        /// <returns>True if the sweep step was successfully advanced to the next sweep point; 
        /// False if the sweep step has passed the end of the sweep point collection.</returns>
        public bool MoveToNextPoint()
        {
            return sweepPointsEnumerator.MoveNext();
        }

        /// <summary>
        /// Executes the configured <see cref="SweepStep{T}.Operation"/> on <see cref="SweepStep{T}.CurrentPoint"/>.
        /// </summary>
        public void ExecuteCurrentPoint()
        {
            Operation(CurrentPoint);
        }

        /// <summary>
        /// Positions the current sweep point before the first point in the collection.
        /// Call <see cref="SweepStep{T}.MoveToNextPoint"/> before operating on the first sweep point.
        /// </summary>
        public void Reset()
        {
            sweepPointsEnumerator = SweepPoints.GetEnumerator();
        }

        /// <summary>
        /// Gets the current point that will be passed to <see cref="SweepStep{T}.Operation"/> upon calling <see cref="SweepStep{T}.ExecuteCurrentPoint"/>.
        /// </summary>
        object ISweepStep.GetCurrentPoint()
        {
            return CurrentPoint;
        }

        /// <summary>
        /// Gets the current point that will be passed to <see cref="SweepStep{T}.Operation"/> upon calling <see cref="SweepStep{T}.ExecuteCurrentPoint"/>.
        /// </summary>
        public T GetCurrentPoint()
        {
            return CurrentPoint;
        }
    }
}
