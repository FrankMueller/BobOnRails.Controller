using BobOnRails.Controller.Physics.Core.Integration;
using BobOnRails.Controller.Physics.Core.Interpolation;

namespace BobOnRails.Controller.Physics.Core
{
    /// <summary>
    /// A static class providing methods compute velocity and position changes from 
    /// accelerations and time spans.
    /// </summary>
    public static class MotionCalculator
    {
        /// <summary>
        /// Computes the jerk as difference of the specified accelerations.
        /// </summary>
        /// <param name="intervalLengthInSeconds">
        /// The length of the interval between the measurement of 
        /// <paramref name="accelerationAtIntervalStart"/> and 
        /// <paramref name="accelerationAtIntervalEnd"/> (dt) [s].
        /// </param>
        /// <param name="accelerationAtIntervalStart">
        /// The acceleration vector at the beginning of the interval.
        /// </param>
        /// <param name="accelerationAtIntervalEnd">
        /// The acceleration vector at the end of the interval.
        /// </param>
        /// <returns>The jerk vector.</returns>
        public static JerkVector GetJerk(double intervalLengthInSeconds,
            AccelerationVector accelerationAtIntervalStart, AccelerationVector accelerationAtIntervalEnd)
        {
            var jerkVector = new JerkVector();

            for (int i = 0; i < ThreeDimensionalVector.Dimension; i++)
                jerkVector[i] = (accelerationAtIntervalEnd[i] - accelerationAtIntervalStart[i]) * intervalLengthInSeconds;

            return jerkVector;
        }

        /// <summary>
        /// Computes the change in velocity from the specified acceleration and time span.
        /// </summary>
        /// <param name="integrator">
        /// An instance of a <see cref="GaussLegendreIntegrator"/> to use to perform
        /// the integration of the velocity function.
        /// </param>
        /// <param name="intervalLengthInSeconds">
        /// The length of the interval to integrate (dt) [s].
        /// </param>
        /// <param name="jerkAtIntervalStart">
        /// The jerk vector at the beginning of the interval.
        /// </param>
        /// <param name="jerkAtIntervalEnd">
        /// The jerk vector at the end of the interval.
        /// </param>
        /// <param name="accelerationAtIntervalStart">
        /// The acceleration vector at the beginning of the interval.
        /// </param>
        /// <param name="accelerationAtIntervalEnd">
        /// The acceleration vector at the end of the interval.
        /// </param>
        /// <returns>The change in velocity dv = Integral(a(t) dt) [m].</returns>
        public static VelocityVector GetVelocityChange(
            GaussLegendreIntegrator integrator, double intervalLengthInSeconds,
            JerkVector jerkAtIntervalStart, JerkVector jerkAtIntervalEnd,
            AccelerationVector accelerationAtIntervalStart, AccelerationVector accelerationAtIntervalEnd)
        {
            var velocity = new VelocityVector(0.0, 0.0, 0.0);
            for (int i = 0; i < ThreeDimensionalVector.Dimension; i++)
            {
                var slope = (accelerationAtIntervalEnd[i] - accelerationAtIntervalStart[i]);
                var interpolator = new HermiteCurveInterpolator(
                    accelerationAtIntervalStart[i], accelerationAtIntervalEnd[i],
                    jerkAtIntervalStart[i], jerkAtIntervalEnd[i]);

                velocity[i] = integrator.Integrate(interpolator.InterpolateAt) * intervalLengthInSeconds;
            }

            return velocity;
        }

        /// <summary>
        /// Computes the change in position from the specified velocity and time span.
        /// </summary>
        /// <param name="integrator">
        /// An instance of a <see cref="GaussLegendreIntegrator"/> to use to perform
        /// the integration of the velocity function.
        /// </param>
        /// <param name="intervalLengthInSeconds">
        /// The length of the interval to integrate (dt) [s].
        /// </param>
        /// <param name="accelerationAtIntervalStart">
        /// The acceleration vector at the beginning of the interval.
        /// </param>
        /// <param name="accelerationAtIntervalEnd">
        /// The acceleration vector at the end of the interval.
        /// </param>
        /// <param name="velocityAtIntervalStart">
        /// The velocity vector at the beginning of the interval.
        /// </param>
        /// <param name="velocityAtIntervalEnd">
        /// The velocity vector at the end of the interval.
        /// </param>
        /// <returns>The change in position dp = Integral(v(t) dt) [m].</returns>
        public static PositionVector GetPositionChange(
            GaussLegendreIntegrator integrator, double intervalLengthInSeconds,
            AccelerationVector accelerationAtIntervalStart, AccelerationVector accelerationAtIntervalEnd,
            VelocityVector velocityAtIntervalStart, VelocityVector velocityAtIntervalEnd)
        {
            var position = new PositionVector(0.0, 0.0, 0.0);
            for (int i = 0; i < ThreeDimensionalVector.Dimension; i++)
            {
                var interpolator = new HermiteCurveInterpolator(
                    velocityAtIntervalStart[i], velocityAtIntervalEnd[i],
                    accelerationAtIntervalStart[i], accelerationAtIntervalEnd[i]);

                position[i] = integrator.Integrate(interpolator.InterpolateAt) * intervalLengthInSeconds;
            }

            return position;
        }
    }
}
