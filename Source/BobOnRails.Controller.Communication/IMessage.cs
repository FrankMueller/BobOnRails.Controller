namespace BobOnRails.Controller.Communication
{
    /// <summary>
    /// An interface for messages send and received from and to
    /// the device.
    /// </summary>
    public interface IMessage
    {
        /// <summary>
        /// A code indicating the type of the message.
        /// </summary>
        byte TypeCode { get; set; }

        /// <summary>
        /// The message body.
        /// </summary>
        byte[] Body { get; set; }
    }
}