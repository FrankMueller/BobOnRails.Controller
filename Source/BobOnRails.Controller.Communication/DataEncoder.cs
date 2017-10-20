using System;
using System.Collections.Generic;

namespace BobOnRails.Controller.Communication
{
    /// <summary>
    /// A class providing static methods for the conversion of measurement data from 
    /// and into arrays of bytes.
    /// </summary>
    public static class DataEncoder
    {
        /// <summary>
        /// Encodes an <see cref="AccelerometerMeasurement"/> into a binary representation.
        /// </summary>
        /// <param name="measurement">The data to encode.</param>
        /// <returns>The binary representation of the measurement.</returns>
        public static byte[] EncodeAccelerometerData(AccelerometerMeasurement measurement)
        {
            var bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes(measurement.TimeStamp.Ticks));

            bytes.AddRange(BitConverter.GetBytes((float)measurement.Acceleration.X));
            bytes.AddRange(BitConverter.GetBytes((float)measurement.Acceleration.Y));
            bytes.AddRange(BitConverter.GetBytes((float)measurement.Acceleration.Z));

            return bytes.ToArray();
        }

        /// <summary>
        /// Decodes an <see cref="AccelerometerMeasurement"/> from the specified array of <see cref="byte"/>s.
        /// </summary>
        /// <param name="bytes">The binary representation of the </param>
        /// <param name="startIndex">The index in the <paramref name="bytes"/> were to start reading at.</param>
        /// <returns>The measurement.</returns>
        public static AccelerometerMeasurement DecodeAccelerometerData(byte[] bytes, int startIndex = 0)
        {
            var timeStamp = new DateTime(BitConverter.ToInt64(bytes, startIndex + 0));
            var vector = new AccelerationVector(
                (double)BitConverter.ToSingle(bytes, startIndex + 8),
                (double)BitConverter.ToSingle(bytes, startIndex + 10),
                (double)BitConverter.ToSingle(bytes, startIndex + 12));

            return new AccelerometerMeasurement(timeStamp, vector);
        }
    }
}
