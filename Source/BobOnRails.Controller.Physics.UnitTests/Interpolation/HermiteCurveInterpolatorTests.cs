using BobOnRails.Controller.Physics.Core.Interpolation;
using NUnit.Framework;

namespace BobOnRails.Controller.Physics.UnitTests.Interpolation
{
    [TestFixture]
    public class HermiteCurveInterpolatorTests
    {
        [TestCase(0, 1, 1, 1, 0.5, 0.5)]
        [TestCase(0, 1, 1, 0, 0.5, 0.625)]
        public void InterpolatesCurveProperly(double y0, double y1, double s0, double s1,
            double xInterpolation, double expectedResult)
        {
            var interpolator = new HermiteCurveInterpolator(y0, y1, s0, s1);

            var value = interpolator.InterpolateAt(xInterpolation);

            Assert.AreEqual(expectedResult, value, 1E-12, "The interpolated value is wrong!");
        }
    }
}
