using System;

namespace BobOnRails.Controller
{
    /// <summary>
    /// A class representing a position on the path of motion.
    /// </summary>
    public class PathPoint
    {
        /// <summary>
        /// Gets or sets the time stamp when this <see cref="PathPoint"/> was reached.
        /// </summary>
        public DateTime Time { get; set; }

        /// <summary>
        /// Gets or sets the position of this <see cref="PathPoint"/> in 3D space.
        /// </summary>
        public PositionVector Position { get; set; }

        /// <summary>
        /// Gets or sets the velocity at this <see cref="PathPoint"/>.
        /// </summary>
        public VelocityVector Velocity { get; set; }

        /// <summary>
        /// Gets or sets the acceleration at this <see cref="PathPoint"/>.
        /// </summary>
        public AccelerationVector Acceleration { get; set; }

        /// <summary>
        /// Gets or sets the jerk at this <see cref="PathPoint"/>.
        /// </summary>
        public JerkVector Jerk { get; set; }

        /// <summary>
        /// Initialize a new instance of the <see cref="PathPoint"/> class.
        /// </summary>
        public PathPoint()
            : this(DateTime.MinValue, new PositionVector(), new VelocityVector(), new AccelerationVector(), new JerkVector())
        { }

        /// <summary>
        /// Initialize a new instance of the <see cref="PathPoint"/> class.
        /// </summary>
        /// <param name="timeStep">The time stamp when this <see cref="PathPoint"/> was reached.</param>
        /// <param name="position">The position of this <see cref="PathPoint"/> in 3D space.</param>
        /// <param name="velocity">The velocity at this <see cref="PathPoint"/>.</param>
        /// <param name="acceleration">The acceleration at this <see cref="PathPoint"/>.</param>
        /// <param name="jerk">The jerk at this <see cref="PathPoint"/>.</param>
        public PathPoint(DateTime timeStep,
            PositionVector position, VelocityVector velocity,
            AccelerationVector acceleration, JerkVector jerk)
        {
            Time = timeStep;
            Position = position;
            Velocity = velocity;
            Acceleration = acceleration;
            Jerk = jerk;
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"R={Position.Length} v={Velocity.Length} a={Acceleration.Length}";
        }
    }
}
