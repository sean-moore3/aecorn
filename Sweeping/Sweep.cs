using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace NationalInstruments.Aecorn.Sweeping
{
    public class Sweep : IEnumerable<ISweepStep>
    {
        private IEnumerable<ISweepStep> _sweepSteps;
        /// <summary>
        /// Collection of <see cref="SweepStep{T}"/> to sweep across.
        /// </summary>
        public IEnumerable<ISweepStep> SweepSteps
        {
            get { return _sweepSteps; }
            set { _sweepSteps = value ?? throw new ArgumentNullException(); }
        }

        /// <summary>
        /// Operation to be performed after the operations for each sweep step have completed.
        /// Set this value to null to perform no operation.
        /// </summary>
        public Action Operation;

        /// <summary>
        /// Creates a new sweep from a collection of <see cref="SweepStep{T}"/>.
        /// </summary>
        /// <param name="sweepSteps">Collection of <see cref="SweepStep{T}"/> to sweep across.</param>
        public Sweep(IEnumerable<ISweepStep> sweepSteps)
        {
            SweepSteps = sweepSteps;
        }

        /// <summary>
        /// Creates a new sweep from a collection of <see cref="SweepStep{T}"/>.
        /// </summary>
        /// <param name="sweepSteps">Collection of <see cref="SweepStep{T}"/> to sweep across.</param>
        /// <param name="operation"><see cref="Operation"/> to be performed after the operations for each sweep step have completed.</param>
        public Sweep(IEnumerable<ISweepStep> sweepSteps, Action operation) : this(sweepSteps)
        {
            Operation = operation;
        }

        /// <summary>
        /// Performs the sweep on the configured sweep steps.
        /// </summary>
        public void Run()
        {
            var sweepStepsStack = new Stack<ISweepStep>(SweepSteps.Reverse());
            RecursiveSweep(sweepStepsStack);
        }

        private void RecursiveSweep(Stack<ISweepStep> sweepStepsStack)
        {
            if (sweepStepsStack.Count > 0)
            {
                ISweepStep step = sweepStepsStack.Pop(); ;
                while (step.MoveToNextPoint())
                {
                    step.ExecuteCurrentPoint();
                    RecursiveSweep(sweepStepsStack);
                }
                step.Reset();
                sweepStepsStack.Push(step);
            }
            else
                Operation?.Invoke();
        }

        public IEnumerator<ISweepStep> GetEnumerator()
        {
            return SweepSteps.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
