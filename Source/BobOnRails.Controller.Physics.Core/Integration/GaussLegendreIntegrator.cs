using System;

namespace BobOnRails.Controller.Physics.Core.Integration
{
    /// <summary>
    /// A class providing methods to integrate a mathematical function using
    /// <a href="https://en.wikipedia.org/wiki/Gaussian_quadrature">Gass-Legendre</a> integration.
    /// </summary>
    public class GaussLegendreIntegrator
    {
        /// <summary>
        /// Gets the base points used for the integration.
        /// </summary>
        public GaussLegendreIntegratorBasePoint[] BasePoints { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="GaussLegendreIntegrator"/> class.
        /// </summary>
        /// <param name="order">The order of the integration.</param>
        public GaussLegendreIntegrator(int order)
        {
            BasePoints = GetLegendreControlPoints(order);
        }

        /// <summary>
        /// Integrates the specified function y = f(x) within the specified interval
        /// x in [<paramref name="from"/>..<paramref name="to"/>].
        /// </summary>
        /// <param name="function">The function to integrate.</param>
        /// <param name="from">The begin of the domain to integrate.</param>
        /// <param name="to">The end of the domain to integrate.</param>
        /// <returns>The integral value.</returns>
        public double Integrate(Func<double, double> function, double from = 0.0, double to = 1.0)
        {
            var slope = 0.5 * (to - from);
            var offset = 0.5 * (from + to);

            var integral = 0.0;
            for (int i = 0; i < BasePoints.Length; ++i)
                integral += BasePoints[i].Weight * function(slope * BasePoints[i].Root + offset);

            return slope * integral;
        }

        private static double[,] GetLegendreCoefficients(int order)
        {
            var coefficients = new double[order + 1, order + 1];
            coefficients[0, 0] = coefficients[1, 1] = 1;

            for (int n = 2; n <= order; ++n)
            {
                coefficients[n, 0] = -(n - 1) * coefficients[n - 2, 0] / n;

                for (int i = 1; i <= n; ++i)
                    coefficients[n, i] = ((2 * n - 1) * coefficients[n - 1, i - 1] - (n - 1) * coefficients[n - 2, i]) / n;
            }

            return coefficients;
        }

        private static double GetLegendrePolynomValue(double[,] coefficients, int order, double x)
        {
            double s = coefficients[order, order];
            for (int i = order; i > 0; i--)
                s = s * x + coefficients[order, i - 1];

            return s;
        }

        private static double GetLegendreDifference(double[,] coefficients, int order, double x)
        {
            return order * (x * GetLegendrePolynomValue(coefficients, order, x)
                          - GetLegendrePolynomValue(coefficients, order - 1, x)) / (x * x - 1);
        }

        private static GaussLegendreIntegratorBasePoint[] GetLegendreControlPoints(int order)
        {
            var controlPoints = new GaussLegendreIntegratorBasePoint[order];

            var lengendreCoefficients = GetLegendreCoefficients(order);
            double x, x1;
            for (int i = 1; i <= order; i++)
            {
                x = Math.Cos(Math.PI * (i - 0.25) / (order + 0.5));
                do
                {
                    x1 = x;
                    x -= GetLegendrePolynomValue(lengendreCoefficients, order, x) 
                         / GetLegendreDifference(lengendreCoefficients, order, x);
                }
                while (Math.Abs(x - x1) >1E-12);

                x1 = GetLegendreDifference(lengendreCoefficients, order, x);
                var weight = 2 / ((1 - x * x) * x1 * x1);

                controlPoints[i-1] = new GaussLegendreIntegratorBasePoint(x, weight);
            }

            return controlPoints;
        }
    }
}
