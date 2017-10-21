namespace BobOnRails.Controller
{
    /// <summary>
    /// A class representing an velocity vector in 3D cartesian space.
    /// </summary>
    public class VelocityVector : ThreeDimensionalVector
    {
        /// <summary>
        /// Gets or sets the velocity along the x-coordinate [m/s^2].
        /// </summary>
        public override double X { get => base.X; set => base.X = value; }

        /// <summary>
        /// Gets or sets the velocity along the y-coordinate [m/s^2].
        /// </summary>
        public override double Y { get => base.Y; set => base.Y = value; }

        /// <summary>
        /// Gets or sets the velocity along the z-coordinate [m/s^2].
        /// </summary>
        public override double Z { get => base.Z; set => base.Z = value; }

        /// <summary>
        /// Initializes a new instance of the <see cref="VelocityVector"/> class.
        /// </summary>
        /// <param name="x">The velocity along the x-coordinate [m/s].</param>
        /// <param name="y">The velocity along the y-coordinate [m/s].</param>
        /// <param name="z">The velocity along the z-coordinate [m/s].</param>
        public VelocityVector(double x = 0.0, double y = 0.0, double z = 0.0) 
            : base(x, y, z)
        { }

        /// <summary>
        /// Adds a <see cref="VelocityVector"/> to a <see cref="VelocityVector"/>.
        /// </summary>
        /// <param name="a">The first summand.</param>
        /// <param name="b">The second summand.</param>
        /// <returns>
        /// A <see cref="VelocityVector"/> that is the sum of <paramref name="a"/>
        /// and <paramref name="b"/>.
        /// </returns>
        public static VelocityVector operator +(VelocityVector a, VelocityVector b)
        {
            return new VelocityVector(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        }

        /// <summary>
        /// Subtracts a <see cref="VelocityVector"/> from a <see cref="VelocityVector"/>.
        /// </summary>
        /// <param name="a">The minuend.</param>
        /// <param name="b">The subtrahend.</param>
        /// <returns>
        /// A <see cref="VelocityVector"/> that is the difference of <paramref name="a"/>
        /// and <paramref name="b"/>.
        /// </returns>
        public static VelocityVector operator -(VelocityVector a, VelocityVector b)
        {
            return new VelocityVector(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
        }

        /// <summary>
        /// Scales the specified <see cref="VelocityVector"/> by the specified factor.
        /// </summary>
        /// <param name="vector">The vector to scale.</param>
        /// <param name="scale">The scale factor.</param>
        /// <returns>
        /// A <see cref="VelocityVector"/> that is the scaled vector of <paramref name="vector"/>.
        /// </returns>
        public static VelocityVector operator *(VelocityVector vector, double scale)
        {
            return new VelocityVector(vector.X * scale, vector.Y * scale, vector.Z * scale);
        }

        /// <summary>
        /// Scales the specified <see cref="VelocityVector"/> by the specified factor.
        /// </summary>
        /// <param name="scale">The scale factor.</param>
        /// <param name="vector">The vector to scale.</param>
        /// <returns>
        /// A <see cref="VelocityVector"/> that is the scaled vector of <paramref name="vector"/>.
        /// </returns>
        public static VelocityVector operator *(double scale, VelocityVector vector)
        {
            return new VelocityVector(vector.X * scale, vector.Y * scale, vector.Z * scale);
        }
    }
}
