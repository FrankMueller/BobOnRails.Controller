namespace BobOnRails.Controller
{
    /// <summary>
    /// A class representing a jerk (aka jolt, surge, or lurch) vector in 3D cartesian space.
    /// </summary>
    public sealed class JerkVector : ThreeDimensionalVector
    {
        /// <summary>
        /// Gets or sets the jerk along the x-coordinate [m/s^3].
        /// </summary>
        public override double X { get => base.X; set => base.X = value; }

        /// <summary>
        /// Gets or sets the jerk along the y-coordinate [m/s^3].
        /// </summary>
        public override double Y { get => base.Y; set => base.Y = value; }

        /// <summary>
        /// Gets or sets the jerk along the z-coordinate [m/s^3].
        /// </summary>
        public override double Z { get => base.Z; set => base.Z = value; }

        /// <summary>
        /// Initializes a new instance of the <see cref="JerkVector"/> class.
        /// </summary>
        /// <param name="x">The jerk along the x-coordinate [m/s^3].</param>
        /// <param name="y">The jerk along the y-coordinate [m/s^3].</param>
        /// <param name="z">The jerk along the z-coordinate [m/s^3].</param>
        public JerkVector(double x = 0.0, double y = 0.0, double z = 0.0)
            : base(x, y, z)
        { }

        /// <summary>
        /// Adds a <see cref="JerkVector"/> to a <see cref="JerkVector"/>.
        /// </summary>
        /// <param name="a">The first summand.</param>
        /// <param name="b">The second summand.</param>
        /// <returns>
        /// A <see cref="JerkVector"/> that is the sum of <paramref name="a"/>
        /// and <paramref name="b"/>.
        /// </returns>
        public static JerkVector operator +(JerkVector a, JerkVector b)
        {
            return new JerkVector(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        }

        /// <summary>
        /// Subtracts a <see cref="JerkVector"/> from a <see cref="JerkVector"/>.
        /// </summary>
        /// <param name="a">The minuend.</param>
        /// <param name="b">The subtrahend.</param>
        /// <returns>
        /// A <see cref="JerkVector"/> that is the difference of <paramref name="a"/>
        /// and <paramref name="b"/>.
        /// </returns>
        public static JerkVector operator -(JerkVector a, JerkVector b)
        {
            return new JerkVector(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
        }

        /// <summary>
        /// Scales the specified <see cref="JerkVector"/> by the specified factor.
        /// </summary>
        /// <param name="vector">The vector to scale.</param>
        /// <param name="scale">The scale factor.</param>
        /// <returns>
        /// A <see cref="JerkVector"/> that is the scaled vector of <paramref name="vector"/>.
        /// </returns>
        public static JerkVector operator *(JerkVector vector, double scale)
        {
            return new JerkVector(vector.X * scale, vector.Y * scale, vector.Z * scale);
        }

        /// <summary>
        /// Scales the specified <see cref="JerkVector"/> by the specified factor.
        /// </summary>
        /// <param name="scale">The scale factor.</param>
        /// <param name="vector">The vector to scale.</param>
        /// <returns>
        /// A <see cref="JerkVector"/> that is the scaled vector of <paramref name="vector"/>.
        /// </returns>
        public static JerkVector operator *(double scale, JerkVector vector)
        {
            return new JerkVector(vector.X * scale, vector.Y * scale, vector.Z * scale);
        }
    }
}
