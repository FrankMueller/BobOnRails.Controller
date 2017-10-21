using System;
using System.Collections;
using System.Collections.Generic;

namespace BobOnRails.Controller
{
    /// <summary>
    /// A basic implementation for a vector in 3D cartesian space.
    /// </summary>
    public abstract class ThreeDimensionalVector : IEnumerable<double>
    {
        /// <summary>
        /// Gets or sets the coordinate of the vector with the specified index.
        /// </summary>
        /// <param name="i">The index of the coordinate.</param>
        /// <returns>The value of the coordinate.</returns>
        public double this[int i]
        {
            get
            {
                if (i == 0)
                    return X;
                else if (i == 1)
                    return Y;
                else if (i == 2)
                    return Z;
                else
                    throw new IndexOutOfRangeException();
            }
            set
            {
                if (i == 0)
                    X = value;
                else if (i == 1)
                    Y = value;
                else if (i == 2)
                    Z = value;
                else
                    throw new IndexOutOfRangeException();
            }

        }

        /// <summary>
        /// Gets the dimension of the vector.
        /// </summary>
        public static int Dimension
        {
            get { return 3; }
        }

        /// <summary>
        /// Gets or sets the x-coordinate of the vector.
        /// </summary>
        public virtual double X { get; set; }

        /// <summary>
        /// Gets or sets the y-coordinate of the vector.
        /// </summary>
        public virtual double Y { get; set; }

        /// <summary>
        /// Gets or sets the z-coordinate of the vector.
        /// </summary>
        public virtual double Z { get; set; }

        /// <summary>
        /// Gets the length of the vector.
        /// </summary>
        public double Length
        {
            get
            {
                return Math.Sqrt(X * X + Y * Y + Z * Z);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ThreeDimensionalVector"/> class.
        /// </summary>
        public ThreeDimensionalVector()
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ThreeDimensionalVector"/> class.
        /// </summary>
        /// <param name="x">The x-coordinate of the vector.</param>
        /// <param name="y">The y-coordinate of the vector.</param>
        /// <param name="z">The z-coordinate of the vector.</param>
        public ThreeDimensionalVector(double x, double y, double z) 
            : this()
        {
            X = x;
            Y = y;
            Z = z;
        }

        /// <inheritdoc cref="IEnumerable{T}.GetEnumerator()"/>
        public IEnumerator<double> GetEnumerator()
        {
            yield return X;
            yield return Y;
            yield return Z;
        }

        /// <inheritdoc cref="IEnumerable.GetEnumerator()"/>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"{X}, {Y}, {Z}";
        }
    }
}
