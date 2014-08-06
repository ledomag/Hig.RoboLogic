namespace Hig.GameEngine
{
    using System;

    public class Counter
    {
        protected Action _action;

        public float Value { get; protected set; }

        /// <summary>
        /// Gets or sets the interval (msec).
        /// </summary>
        public float Interval { get; set; }

        /// <summary>
        /// Creates the timer.
        /// </summary>
        /// <param name="action">An action of the timer in the interval.</param>
        /// <param name="interval">The interval (msec).</param>
        public Counter(Action action, float interval)
        {
            _action = action;
            Interval = interval;
        }

        public void Update(uint milliseconds)
        {
            if (Interval > 0)
            {
                Value += milliseconds;

                while (Value >= Interval)
                {
                    _action();
                    Value -= Interval;
                }
            }
        }

        public void Reset()
        {
            Value = 0;
        }
    }
}
