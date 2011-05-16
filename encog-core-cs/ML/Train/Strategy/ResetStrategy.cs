using Encog.Neural.Networks.Training;
using Encog.Util.Logging;

namespace Encog.ML.Train.Strategy
{
    /// <summary>
    /// The reset strategy will reset the weights if the neural network fails to fall
    /// below a specified error by a specified number of cycles. This can be useful
    /// to throw out initially "bad/hard" random initializations of the weight
    /// matrix.
    /// </summary>
    ///
    public class ResetStrategy : IStrategy
    {
        /// <summary>
        /// The number of cycles to reach the required minimum error.
        /// </summary>
        ///
        private readonly int cycles;

        /// <summary>
        /// The required minimum error.
        /// </summary>
        ///
        private readonly double required;

        /// <summary>
        /// How many bad cycles have there been so far.
        /// </summary>
        ///
        private int badCycleCount;

        private MLResettable method;

        /// <summary>
        /// The training algorithm that is using this strategy.
        /// </summary>
        ///
        private MLTrain train;

        /// <summary>
        /// Construct a reset strategy.  The error rate must fall
        /// below the required rate in the specified number of cycles,
        /// or the neural network will be reset to random weights and
        /// bias values.
        /// </summary>
        ///
        /// <param name="required_0">The required error rate.</param>
        /// <param name="cycles_1">The number of cycles to reach that rate.</param>
        public ResetStrategy(double required_0, int cycles_1)
        {
            required = required_0;
            cycles = cycles_1;
            badCycleCount = 0;
        }

        #region IStrategy Members

        /// <summary>
        /// Initialize this strategy.
        /// </summary>
        ///
        /// <param name="train_0">The training algorithm.</param>
        public virtual void Init(MLTrain train_0)
        {
            train = train_0;

            if (!(train_0.Method is MLMethod))
            {
                throw new TrainingError(
                    "To use the reset strategy the machine learning method must support MLResettable.");
            }

            method = (MLResettable) train.Method;
        }

        /// <summary>
        /// Called just after a training iteration.
        /// </summary>
        ///
        public virtual void PostIteration()
        {
        }

        /// <summary>
        /// Called just before a training iteration.
        /// </summary>
        ///
        public virtual void PreIteration()
        {
            if (train.Error > required)
            {
                badCycleCount++;
                if (badCycleCount > cycles)
                {
                    EncogLogging.Log(EncogLogging.LEVEL_DEBUG,
                                     "Failed to imrove network, resetting.");
                    method.Reset();
                    badCycleCount = 0;
                }
            }
            else
            {
                badCycleCount = 0;
            }
        }

        #endregion
    }
}