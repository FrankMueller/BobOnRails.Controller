using System;
using NUnit.Framework;

namespace BobOnRails.Controller.Communication.UnitTests
{
    [TestFixture]
    public class DataEncoderTests
    {
        [Test]
        public void CanDecodeEncodedAcceleration()
        {
            var testData = new AccelerometerMeasurement(DateTime.Now, new AccelerationVector(1, 2, 3));

            var encodedData = DataEncoder.EncodeAccelerometerData(testData);
            var decodedData = DataEncoder.DecodeAccelerometerData(encodedData);

            Assert.AreEqual(testData.TimeStamp, decodedData.TimeStamp);
            Assert.AreEqual(testData.Acceleration.X, decodedData.Acceleration.X, 1E-12);
            Assert.AreEqual(testData.Acceleration.Y, decodedData.Acceleration.Y, 1E-12);
            Assert.AreEqual(testData.Acceleration.Z, decodedData.Acceleration.Z, 1E-12);
        }
    }
}
