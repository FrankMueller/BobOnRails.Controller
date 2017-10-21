using System;
using BobOnRails.Controller.Physics.Integration;
using NUnit.Framework;

namespace BobOnRails.Controller.Physics.UnitTests.Integration
{
    [TestFixture]
    public class GaussLegendreIntegratorTests
    {
        [TestCase(2, new double[] { 0.5773502691896257, -0.5773502691896257 }, new double[] { 1.0000000000000000, 1.0000000000000000 })]
        [TestCase(3, new double[] { 0.7745966692414834, 0.0000000000000000, -0.7745966692414834 }, new double[] { 0.5555555555555556, 0.8888888888888888, 0.5555555555555556 })]
        [TestCase(4, new double[] { 0.8611363115940526, 0.3399810435848563, -0.3399810435848563, -0.8611363115940526 }, new double[] { 0.3478548451374538, 0.6521451548625461, 0.6521451548625461, 0.3478548451374538 })]
        [TestCase(5, new double[] { 0.9061798459386640, 0.5384693101056831, 0.0000000000000000, -0.5384693101056831, -0.9061798459386640 }, new double[] { 0.2369268850561891, 0.4786286704993665, 0.5688888888888889, 0.4786286704993665, 0.2369268850561891 })]
        public void ComputesCorrectBasePoints(int order, double[] expectedRoots, double[] expectedWeights)
        {
            var integrator = new GaussLegendreIntegrator(order);

            Assert.AreEqual(integrator.BasePoints.Length, order, "The number of base points doesn't match the requested order of integration!");
            for (int i = 0; i < order; i++)
            {
                var basePoint = integrator.BasePoints[i];

                Assert.AreEqual(expectedRoots[i], basePoint.Root, 1E-12, $"The {i}th root is wrong!");
                Assert.AreEqual(expectedWeights[i], basePoint.Weight, 1E-12, $"The {i}th root is wrong!");
            }
        }

        [Test]
        public void IntegratesSinusProperly()
        {
            var integrator = new GaussLegendreIntegrator(15);

            var domainStart = 0.0;
            var domainEnd = 2.0 * Math.PI;
            var integral = integrator.Integrate((x) => Math.Sin(x), 0, domainEnd);

            Assert.AreEqual(Math.Cos(domainStart) - Math.Cos(domainEnd), integral, 1E-8, "The integration resulted in an improper deviation!");
        }
    }
}
