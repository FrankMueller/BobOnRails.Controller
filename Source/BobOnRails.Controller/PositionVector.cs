namespace BobOnRails.Controller
{
    /// <summary>
    /// A class representing a position in 3D cartesian space.
    /// </summary>
    public class PositionVector : ThreeDimensionalVector
    {
        /// <summary>
        /// Gets or sets the x-coordinate of the position [m].
        /// </summary>
        public override double X { get => base.X; set => base.X = value; }

        /// <summary>
        /// Gets or sets the y-coordinate of the position [m].
        /// </summary>
        public override double Y { get => base.Y; set => base.Y = value; }

        /// <summary>
        /// Gets or sets the z-coordinate of the position [m].
        /// </summary>
        public override double Z { get => base.Z; set => base.Z = value; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PositionVector"/> class.
        /// </summary>
        /// <param name="x">The x-coordinate of the position [m].</param>
        /// <param name="y">The y-coordinate of the position [m].</param>
        /// <param name="z">The z-coordinate of the position [m].</param>
        public PositionVector(double x = 0.0, double y = 0.0, double z = 0.0)
            : base(x, y, z)
        { }

        /// <summary>
        /// Adds a <see cref="PositionVector"/> to a <see cref="PositionVector"/>.
        /// </summary>
        /// <param name="a">The first summand.</param>
        /// <param name="b">The second summand.</param>
        /// <returns>
        /// A <see cref="PositionVector"/> that is the sum of <paramref name="a"/>
        /// and <paramref name="b"/>.
        /// </returns>
        public static PositionVector operator +(PositionVector a, PositionVector b)
        {
            return new PositionVector(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        }

        /// <summary>
        /// Subtracts a <see cref="PositionVector"/> from a <see cref="PositionVector"/>.
        /// </summary>
        /// <param name="a">The minuend.</param>
        /// <param name="b">The subtrahend.</param>
        /// <returns>
        /// A <see cref="PositionVector"/> that is the difference of <paramref name="a"/>
        /// and <paramref name="b"/>.
        /// </returns>
        public static PositionVector operator -(PositionVector a, PositionVector b)
        {
            return new PositionVector(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
        }

        /// <summary>
        /// Scales the specified <see cref="PositionVector"/> by the specified factor.
        /// </summary>
        /// <param name="position">The vector to scale.</param>
        /// <param name="scale">The scale factor.</param>
        /// <returns>
        /// A <see cref="PositionVector"/> that is the scaled vector of <paramref name="position"/>.
        /// </returns>
        public static PositionVector operator *(PositionVector position, double scale)
        {
            return new PositionVector(position.X * scale, position.Y * scale, position.Z * scale);
        }

        /// <summary>
        /// Scales the specified <see cref="PositionVector"/> by the specified factor.
        /// </summary>
        /// <param name="scale">The scale factor.</param>
        /// <param name="position">The vector to scale.</param>
        /// <returns>
        /// A <see cref="PositionVector"/> that is the scaled vector of <paramref name="position"/>.
        /// </returns>
        public static PositionVector operator *(double scale, PositionVector position)
        {
            return new PositionVector(position.X * scale, position.Y * scale, position.Z * scale);
        }
    }
}
