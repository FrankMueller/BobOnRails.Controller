namespace BobOnRails.Controller
{
    /// <summary>
    /// A class representing an acceleration vector in 3D cartesian space.
    /// </summary>
    public sealed class AccelerationVector : ThreeDimensionalVector
    {
        /// <summary>
        /// Gets or sets the acceleration along the x-coordinate [m/s^2].
        /// </summary>
        public override double X { get => base.X; set => base.X = value; }

        /// <summary>
        /// Gets or sets the acceleration along the y-coordinate [m/s^2].
        /// </summary>
        public override double Y { get => base.Y; set => base.Y = value; }

        /// <summary>
        /// Gets or sets the acceleration along the z-coordinate [m/s^2].
        /// </summary>
        public override double Z { get => base.Z; set => base.Z = value; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AccelerationVector"/> class.
        /// </summary>
        /// <param name="x">The acceleration along the x-coordinate [m/s^2].</param>
        /// <param name="y">The acceleration along the y-coordinate [m/s^2].</param>
        /// <param name="z">The acceleration along the z-coordinate [m/s^2].</param>
        public AccelerationVector(double x, double y, double z)
            : base(x, y, z)
        { }

        /// <summary>
        /// Adds a <see cref="AccelerationVector"/> to a <see cref="AccelerationVector"/>.
        /// </summary>
        /// <param name="a">The first summand.</param>
        /// <param name="b">The second summand.</param>
        /// <returns>
        /// A <see cref="AccelerationVector"/> that is the sum of <paramref name="a"/>
        /// and <paramref name="b"/>.
        /// </returns>
        public static AccelerationVector operator +(AccelerationVector a, AccelerationVector b)
        {
            return new AccelerationVector(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        }

        /// <summary>
        /// Subtracts a <see cref="AccelerationVector"/> from a <see cref="AccelerationVector"/>.
        /// </summary>
        /// <param name="a">The minuend.</param>
        /// <param name="b">The subtrahend.</param>
        /// <returns>
        /// A <see cref="AccelerationVector"/> that is the difference of <paramref name="a"/>
        /// and <paramref name="b"/>.
        /// </returns>
        public static AccelerationVector operator -(AccelerationVector a, AccelerationVector b)
        {
            return new AccelerationVector(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
        }

        /// <summary>
        /// Scales the specified <see cref="AccelerationVector"/> by the specified factor.
        /// </summary>
        /// <param name="vector">The vector to scale.</param>
        /// <param name="scale">The scale factor.</param>
        /// <returns>
        /// A <see cref="AccelerationVector"/> that is the scaled vector of <paramref name="vector"/>.
        /// </returns>
        public static AccelerationVector operator *(AccelerationVector vector, double scale)
        {
            return new AccelerationVector(vector.X * scale, vector.Y * scale, vector.Z * scale);
        }

        /// <summary>
        /// Scales the specified <see cref="AccelerationVector"/> by the specified factor.
        /// </summary>
        /// <param name="scale">The scale factor.</param>
        /// <param name="vector">The vector to scale.</param>
        /// <returns>
        /// A <see cref="AccelerationVector"/> that is the scaled vector of <paramref name="vector"/>.
        /// </returns>
        public static AccelerationVector operator *(double scale, AccelerationVector vector)
        {
            return new AccelerationVector(vector.X * scale, vector.Y * scale, vector.Z * scale);
        }
    }
}
