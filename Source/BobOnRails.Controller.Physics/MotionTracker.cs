using System;
using System.Collections.ObjectModel;
using System.Linq;
using BobOnRails.Controller.Physics.Integration;
using BobOnRails.Controller.Physics.Interpolation;

namespace BobOnRails.Controller.Physics
{
    /// <summary>
    /// A motion tracker which tracks the position of an object by integrating
    /// acceleration measures.
    /// </summary>
    public class MotionTracker
    {
        private Path path;
        private GaussLegendreIntegrator integrator;

        /// <summary>
        /// Gets the path the object moves along.
        /// </summary>
        public ReadOnlyCollection<PathPoint> Path { get; private set; }

        /// <summary>
        /// Gets the current (last) position on the <see cref="Path"/>.
        /// </summary>
        public PathPoint CurrentPosition
        {
            get { return path.Last(); }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MotionTracker"/> class.
        /// </summary>
        /// <param name="integrationOrder">
        /// The order of integration to use for the integration of the velocity and position.
        /// </param>
        public MotionTracker(int integrationOrder = 5)
            : this(new PathPoint(), integrationOrder)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="MotionTracker"/> class.
        /// </summary>
        /// <param name="initialPosition">The initial position state of the device.</param>
        /// <param name="integrationOrder">
        /// The order of integration to use for the integration of the velocity and position.
        /// </param>
        public MotionTracker(PathPoint initialPosition, int integrationOrder = 5)
        {
            path = new Path();
            path.Add(initialPosition);

            Path = path.AsReadOnly();

            integrator = new GaussLegendreIntegrator(integrationOrder);
        }

        /// <summary>
        /// Appends a new point to the <see cref="Path"/> computed from the
        /// specified acceleration.
        /// </summary>
        /// <param name="acceleration">The measured acceleration.</param>
        /// <param name="timeStamp">The time stamp of the measurement.</param>
        public void AppendMotion(AccelerationVector acceleration, DateTime timeStamp)
        {
            //Improve the jerk, velocity and position of the last point (since we now
            //know the next point acceleration we can compute an more exact jerk using
            //central difference method)
            var previousPoint = ImproveCurrentPosition(acceleration, timeStamp);

            var intervalLengthInSeconds = (timeStamp - previousPoint.Time).TotalSeconds;

            var jerk = MotionCalculator.GetJerk(intervalLengthInSeconds,
                    previousPoint.Acceleration, acceleration);

            var velocity = previousPoint.Velocity
                + MotionCalculator.GetVelocityChange(
                                    integrator, intervalLengthInSeconds,
                                    previousPoint.Jerk, jerk,
                                    previousPoint.Acceleration, acceleration);

            var position = previousPoint.Position
                + MotionCalculator.GetPositionChange(
                                    integrator, intervalLengthInSeconds,
                                    previousPoint.Acceleration, acceleration,
                                    previousPoint.Velocity, velocity);

            var newPosition = new PathPoint();
            newPosition.Time = timeStamp;
            newPosition.Jerk = jerk;
            newPosition.Acceleration = acceleration;
            newPosition.Velocity = velocity;
            newPosition.Position = position;

            path.Add(newPosition);
        }

        private PathPoint ImproveCurrentPosition(AccelerationVector nextPointAcceleration, 
            DateTime nextPointTimeStamp)
        {
            var currentPoint = Path.Last();
            if (Path.Count > 1)
            {
                var previousPoint = Path[Path.Count - 2];

                var intervalLengthInSeconds = (nextPointTimeStamp - currentPoint.Time).TotalSeconds;
                currentPoint.Jerk = MotionCalculator.GetJerk((nextPointTimeStamp - previousPoint.Time).TotalSeconds,
                                                              previousPoint.Acceleration, nextPointAcceleration);

                currentPoint.Velocity = previousPoint.Velocity + MotionCalculator.GetVelocityChange(
                    integrator, intervalLengthInSeconds,
                    previousPoint.Jerk, currentPoint.Jerk,
                    previousPoint.Acceleration, currentPoint.Acceleration);

                currentPoint.Position = previousPoint.Position + MotionCalculator.GetPositionChange(
                    integrator, intervalLengthInSeconds,
                    previousPoint.Acceleration, currentPoint.Acceleration,
                    previousPoint.Velocity, currentPoint.Velocity);
            }

            return currentPoint;
        }
    }
}
