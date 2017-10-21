namespace BobOnRails.Controller.Physics.Interpolation
{
    /// <summary>
    /// A class providing methods to smoothly interpolate a function y = f(x)
    /// with x [0..1] using an
    /// <a href="https://en.wikipedia.org/wiki/Cubic_Hermite_spline">hermite curve</a>
    /// approach.
    /// </summary>
    public class HermiteCurveInterpolator
    {
        /// <summary>
        /// Gets or sets the function value (y = f(x)) at x=0.
        /// </summary>
        public double Y0 { get; set; }

        /// <summary>
        /// Gets or sets the function value (y = f(x)) at x=1.
        /// </summary>
        public double Y1 { get; set; }

        /// <summary>
        /// Gets or sets the slope of the function (s = f(x)/dx) at x=0.
        /// </summary>
        public double S0 { get; set; }

        /// <summary>
        /// Gets or sets the slope of the function (s = f(x)/dx) at x=1.
        /// </summary>
        public double S1 { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="HermiteCurveInterpolator"/> class.
        /// </summary>
        /// <param name="y0">The function value (y = f(x)) at x=0.</param>
        /// <param name="y1">The function value (y = f(x)) at x=1.</param>
        /// <param name="s0">The slope of the function (s = f(x)/dx) at x=0.</param>
        /// <param name="s1">The slope of the function (s = f(x)/dx) at x=1.</param>
        public HermiteCurveInterpolator(double y0, double y1, double s0, double s1)
        {
            Y0 = y0;
            Y1 = y1;
            S0 = s0;
            S1 = s1;
        }

        /// <summary>
        /// Interpolates a function value at the specified argument <paramref name="x"/>.
        /// </summary>
        /// <param name="x">The value to interpolate at [0..1].</param>
        /// <returns>The interpolated value.</returns>
        public double InterpolateAt(double x)
        {
            var x2 = x * x;
            var x3 = x2 * x;

            return (+2 * x3 - 3 * x2 + 1) * Y0
                 + (+1 * x3 - 2 * x2 + x) * S0
                 + (-2 * x3 + 3 * x2 + 0) * Y1
                 + (+1 * x3 - 1 * x2 + 0) * S1;
        }
    }
}
