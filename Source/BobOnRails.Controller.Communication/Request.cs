namespace BobOnRails.Controller.Communication
{
    /// <summary>
    /// Represents a request which can be send to the remote device.
    /// </summary>
    public class Request
    {
        /// <summary>
        /// Gets the type of the request.
        /// </summary>
        public RequestTypes Type { get; private set; }

        /// <summary>
        /// Gets the content of the request.
        /// </summary>
        public byte[] Body { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Request"/> class.
        /// </summary>
        /// <param name="type">The type of the request.</param>
        /// <param name="body">The body of the request.</param>
        public Request(RequestTypes type, byte[] body)
        {
            Type = type;

            if (body != null)
                Body = body;
            else
                Body = new byte[0];
        }
    }
}
