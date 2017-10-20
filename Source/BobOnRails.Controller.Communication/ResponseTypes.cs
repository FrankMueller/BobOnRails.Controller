namespace BobOnRails.Controller.Communication
{
    /// <summary>
    /// The available types of responses (<see cref="Response"/>).
    /// </summary>
    public enum ResponseTypes : byte
    {
        /// <summary>
        /// Response to a <see cref="RequestTypes.Echo"/> request (esp. for test purposes).
        /// </summary>
        Echo = 0,
        /// <summary>
        /// Response telling that the diconnection logic was started on the device.
        /// </summary>
        Disconnect = 1,
        /// <summary>
        /// Response containing an accelerometer measurement data set.
        /// </summary>
        AccelerometerData = 2
    }
}
