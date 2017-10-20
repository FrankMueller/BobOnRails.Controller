namespace BobOnRails.Controller.Communication
{
    /// <summary>
    /// Represents a request which can be send to the remote device.
    /// </summary>
    public class Response
    {
        /// <summary>
        /// Gets the type of the response.
        /// </summary>
        public ResponseTypes Type { get; private set; }

        /// <summary>
        /// Gets the content of the response.
        /// </summary>
        public byte[] Body { get; internal set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Response"/> class.
        /// </summary>
        /// <param name="type">The type of the response.</param>
        public Response(ResponseTypes type)
        {
            Type = type;
        }
    }
}
