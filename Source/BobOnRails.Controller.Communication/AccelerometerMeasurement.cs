using System;

namespace BobOnRails.Controller.Communication
{
    /// <summary>
    /// Represents an accelerometer measurement data set.
    /// </summary>
    public class AccelerometerMeasurement
    {
        /// <summary>
        /// Gets the time stamp of the measurement.
        /// </summary>
        public DateTime TimeStamp { get; private set; }

        /// <summary>
        /// Gets the acceleration vector.
        /// </summary>
        public AccelerationVector Acceleration { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AccelerometerMeasurement"/> class.
        /// </summary>
        /// <param name="timeStamp">The time stamp of the measurement.</param>
        /// <param name="acceleration">The acceleration vector.</param>
        public AccelerometerMeasurement(DateTime timeStamp, AccelerationVector acceleration)
        {
            TimeStamp = timeStamp;
            Acceleration = acceleration;
        }
    }
}
