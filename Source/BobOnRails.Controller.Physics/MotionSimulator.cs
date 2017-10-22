using System;

namespace BobOnRails.Controller.Physics
{
    /// <summary>
    /// A static class providing methods to compute generic acceleration measurements.
    /// </summary>
    public static class MotionSimulator
    {
        /// <summary>
        /// Generates accelerations which would idealy occur during a motion
        /// on a circle like path with a constant speed.
        /// </summary>
        /// <param name="circleRadius">The radius of the circle [m].</param>
        /// <param name="speed">The speed along the circle [m/s].</param>
        /// <param name="sampleFrequency">
        /// The frequency of the artifical acceleration measurement [Hz]
        /// .</param>
        /// <param name="rotations">
        /// The number of rotations along the circle [-].
        /// </param>
        /// <returns>
        /// A <see cref="Path"/> containing the positions, speeds and accelerations
        /// which occur during the motion.
        /// </returns>
        public static Path GenerateConstantSpeedCirclePath(double circleRadius, double speed,
            double sampleFrequency, double rotations = 1.0)
        {
            var timeForOneRotation = 2.0 * Math.PI * circleRadius / speed;
            var timeStepsForOneRotation = timeForOneRotation * sampleFrequency;

            var startTime = DateTime.Now;
            var timeStep = 1.0 / sampleFrequency;
            var angleStep = 2.0 * Math.PI / timeStepsForOneRotation;
            var angleSpeed = 2.0 * Math.PI / timeForOneRotation;

            var path = new Path();
            for (int i = 0; i < timeStepsForOneRotation * rotations; i++)
            {
                var angle = angleStep * i;
                path.Add(new PathPoint(
                    startTime + TimeSpan.FromSeconds(timeStep * i),
                    GetPositionOnCircle(circleRadius, angle),
                    GetVelocityOnCircle(circleRadius, angle, angleSpeed),
                    GetAccelerationOnCircle(circleRadius, angle, angleSpeed),
                    new JerkVector(0.0, 0.0, 0.0)));
            }

            return path;
        }

        /// <summary>
        /// Generates accelerations which would idealy occur during a motion
        /// on a circle like path with a sinus like speed distribution.
        /// </summary>
        /// <param name="circleRadius">The radius of the circle [m].</param>
        /// <param name="topSpeed">The maximal speed which shall occur [m/s].</param>
        /// <param name="sampleFrequency">
        /// The frequency of the artifical acceleration measurement [Hz].
        /// </param>
        /// <param name="periodTime">
        /// The time for one period of the sinus like speed function [s].
        /// </param>
        /// <param name="rotations">
        /// The number of rotations along the circle [-].
        /// </param>
        /// <returns>
        /// A <see cref="Path"/> containing the positions, speeds and accelerations
        /// which occur during the motion.
        /// </returns>
        public static Path GenerateSinusSpeedCirclePath(double circleRadius, double topSpeed,
            double sampleFrequency, double periodTime, double rotations = 1.0)
        {
            var startTime = DateTime.Now;
            var timeStep = 1.0 / sampleFrequency;

            var time = 0.0;
            var angle = 0.0;
            var lastAngleSpeed = 0.0;
            var path = new Path();
            do
            {
                var speed = 0.5 * topSpeed * (Math.Sin(2.0 * Math.PI / periodTime * time - Math.PI * 0.5) + 1.0);

                var angleSpeed = speed / circleRadius;
                var angleIncrement = (lastAngleSpeed + angleSpeed) * 0.5 * timeStep;
                lastAngleSpeed = angleSpeed;

                angle += angleIncrement;

                path.Add(new PathPoint(
                    startTime + TimeSpan.FromSeconds(time),
                    GetPositionOnCircle(circleRadius, angle),
                    GetVelocityOnCircle(circleRadius, angle, angleSpeed),
                    GetAccelerationOnCircle(circleRadius, angle, angleSpeed), 
                    new JerkVector(0.0, 0.0, 0.0)));

                time += timeStep;
            }
            while (angle < 2.0 * Math.PI * rotations);

            return path;
        }

        /// <summary>
        /// Generates accelerations which would idealy occur during a motion
        /// on a line with a sinus like speed distribution.
        /// </summary>
        /// <param name="topSpeed">The maximal speed which shall occur [m/s].</param>
        /// <param name="sampleFrequency">
        /// The frequency of the artifical acceleration measurement [Hz].
        /// </param>
        /// <param name="periodTime">
        /// The time for one period of the sinus like speed function [s].
        /// </param>
        /// <param name="periodCount">
        /// The number of periods of the sinus like speed function to build [-].
        /// </param>
        /// <returns>
        /// A <see cref="Path"/> containing the positions, speeds and accelerations
        /// which occur during the motion.
        /// </returns>
        public static Path GenerateSinusSpeedLinearPath(double topSpeed, double sampleFrequency,
            double periodTime, double periodCount = 1.0)
        {
            var startTime = DateTime.Now;
            var timeStep = 1.0 / sampleFrequency;

            var time = 0.0;
            var path = new Path();
            do
            {
                var periodFactor = 2.0 * Math.PI / periodTime;
                var speedFactor = 0.5 * topSpeed;

                var length = speedFactor * (-Math.Sin(periodFactor * time) / periodFactor + time);
                var speed = speedFactor * (-Math.Cos(periodFactor * time) + 1.0);
                var acceleration = speedFactor * Math.Sin(periodFactor * time) * periodFactor;

                path.Add(new PathPoint(
                    startTime + TimeSpan.FromSeconds(time),
                    new PositionVector(length, 0, 0),
                    new VelocityVector(speed, 0, 0),
                    new AccelerationVector(acceleration, 0, 0),
                    new JerkVector(0.0, 0.0, 0.0)));

                time += timeStep;
            }
            while (time < periodTime * periodCount);

            return path;
        }

        private static PositionVector GetPositionOnCircle(double circleRadius, double angle)
        {
            return new PositionVector(
                    Math.Cos(angle) * circleRadius,
                    Math.Sin(angle) * circleRadius,
                    0);
        }

        private static VelocityVector GetVelocityOnCircle(double circleRadius, double angle, double angleSpeed)
        {
            return new VelocityVector(
                -Math.Sin(angle) * circleRadius * angleSpeed,
                +Math.Cos(angle) * circleRadius * angleSpeed,
                0);
        }

        private static AccelerationVector GetAccelerationOnCircle(double circleRadius, double angle, double angleSpeed)
        {
            return new AccelerationVector(
                -Math.Cos(angle) * circleRadius * angleSpeed * angleSpeed,
                -Math.Sin(angle) * circleRadius * angleSpeed * angleSpeed,
                0);
        }
    }
}
