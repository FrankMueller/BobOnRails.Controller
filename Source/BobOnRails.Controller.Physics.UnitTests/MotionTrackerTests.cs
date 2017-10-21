using System;
using BobOnRails.Controller.Physics.Core;
using NUnit.Framework;

namespace BobOnRails.Controller.Physics.UnitTests
{
    [TestFixture]
    public class MotionTrackerTests
    {
        [TestCase(0.353, 50, 200, 1)]
        [TestCase(1, 50, 1000, 2)]
        [TestCase(1, 50, 100, 2)]
        public void TracksCirclePathProperly(double circleRadius, double timeForOneRotation,
            int timeStepsForOneRotation, int rotations)
        {
            var speed = Math.PI * circleRadius / timeForOneRotation;
            var sampleFrequency = timeStepsForOneRotation / timeForOneRotation;
            var path = MotionSimulator.GenerateConstantSpeedCirclePath(circleRadius, speed, sampleFrequency, rotations);

            //Initialize the motion tracker
            var tracker = new MotionTracker(path[0]);

            //Move along the circle
            var maxDeviation = 0.0;
            for (int i = 1; i < path.Count; ++i)
            {
                var pathPoint = path[i];
                var previousPathPoint = path[i - 1];
                tracker.AppendMotion(pathPoint.Acceleration, pathPoint.Time - previousPathPoint.Time);

                var trackedPoint = tracker.CurrentPosition.Position;
                var deviation = Math.Abs((pathPoint.Position - trackedPoint).Length);
                maxDeviation = Math.Max(maxDeviation, deviation);

                Assert.LessOrEqual(deviation, circleRadius * 0.01, $"Motion tracker lost position at step {i}/{timeStepsForOneRotation}");
            }

            TestContext.WriteLine($"Max. deviation was: {maxDeviation}");
        }

        [TestCase(2.0 / (2.0 * Math.PI), 2.0 * 1000 / 3600, 50, 5, 3)]
        public void TracksSinusSpeedCirclePathProperly(double circleRadius, double topSpeed,
            double sampleFrequency, double periodTime, double rotations)
        {
            var path = MotionSimulator.GenerateSinusSpeedCirclePath(circleRadius, topSpeed,
                sampleFrequency, periodTime, rotations);

            //Initialize the motion tracker
            var tracker = new MotionTracker(path[0]);

            //Move along the circle
            var maxDeviation = 0.0;
            for (int i = 1; i < path.Count; ++i)
            {
                var pathPoint = path[i];
                var previousPathPoint = path[i - 1];
                tracker.AppendMotion(pathPoint.Acceleration, pathPoint.Time - previousPathPoint.Time);

                var trackedPoint = tracker.CurrentPosition.Position;
                var deviation = Math.Abs((pathPoint.Position - trackedPoint).Length);
                maxDeviation = Math.Max(maxDeviation, deviation);

                Assert.LessOrEqual(deviation, circleRadius * 0.01, $"Motion tracker lost position at step {i}/{path.Count} (dt={pathPoint.Time})");
            }

            TestContext.WriteLine($"Max. deviation was: {maxDeviation}");
        }

        [TestCase(2.0 * 1000 / 3600, 50, 5, 5)]
        public void TracksSinusSpeedLinearPathProperly(double topSpeed, double sampleFrequency,
            double periodTime, double periodCount)
        {
            var path = MotionSimulator.GenerateSinusSpeedLinearPath(topSpeed, sampleFrequency,
                periodTime, periodCount);

            //Initialize the motion tracker
            var tracker = new MotionTracker(path[0]);

            //Move along the path
            var maxDeviation = 0.0;
            for (int i = 1; i < path.Count; ++i)
            {
                var pathPoint = path[i];
                var previousPathPoint = path[i - 1];
                tracker.AppendMotion(pathPoint.Acceleration, pathPoint.Time - previousPathPoint.Time);

                var trackedPoint = tracker.CurrentPosition.Position;
                var deviation = Math.Abs((pathPoint.Position - trackedPoint).Length);
                maxDeviation = Math.Max(maxDeviation, deviation);

                Assert.LessOrEqual(deviation, 0.01, $"Motion tracker lost position at step {i}/{path.Count} (dt={pathPoint.Time})");
            }

            TestContext.WriteLine($"Max. deviation was: {maxDeviation}");
        }
    }
}
