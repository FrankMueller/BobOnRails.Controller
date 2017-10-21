namespace BobOnRails.Controller.Physics.Core.Integration
{
    /// <summary>
    /// Represents a base point for the <see cref="GaussLegendreIntegrator"/>.
    /// </summary>
    public struct GaussLegendreIntegratorBasePoint
    {
        /// <summary>
        /// The location of the function to evaluate at.
        /// </summary>
        public double Root { get; set; }

        /// <summary>
        /// The weight of the evaluated function value.
        /// </summary>
        public double Weight { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="GaussLegendreIntegratorBasePoint"/> class.
        /// </summary>
        /// <param name="root">The location of the function to evaluate at.</param>
        /// <param name="weight">The weight of the evaluated function value.</param>
        public GaussLegendreIntegratorBasePoint(double root, double weight)
            : this()
        {
            Root = root;
            Weight = weight;
        }
    }
}
