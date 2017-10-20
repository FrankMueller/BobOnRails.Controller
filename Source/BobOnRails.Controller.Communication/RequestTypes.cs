namespace BobOnRails.Controller.Communication
{
    /// <summary>
    /// The available types of requests (<see cref="Request"/>).
    /// </summary>
    public enum RequestTypes : byte
    {
        /// <summary>
        /// Request to reply with the same data that was send (esp. for test purposes).
        /// </summary>
        Echo = 0,
        /// <summary>
        /// Request to perform the disconnect logic.
        /// </summary>
        Disconnect = 1,
        /// <summary>
        /// Request to start sending accelerometer measurement data.
        /// </summary>
        StartAccelerometerDataStream = 2,
        /// <summary>
        /// Request to stop sending accelerometer measurement data.
        /// </summary>
        StopAccelerometerDataStream = 3
    }
}
