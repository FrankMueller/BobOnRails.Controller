using System;
using System.Collections.Generic;

namespace BobOnRails.Controller.Communication
{
    public static class DataEncoder
    {
        public static byte[] EncodeAccelerometerData(DateTime time, AccelerationVector acceleration)
        {
            var bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes(time.Ticks));

            bytes.AddRange(BitConverter.GetBytes(acceleration.X));
            bytes.AddRange(BitConverter.GetBytes(acceleration.Y));
            bytes.AddRange(BitConverter.GetBytes(acceleration.Z));

            return bytes.ToArray();
        }

        public static AccelerationVector DecodeAccelerometerData()
        {
            throw new NotImplementedException();
        }
    }
}
