using System;

namespace BobOnRails.Controller.Communication
{
    /// <summary>
    /// Represents a request which can be send to the remote device.
    /// </summary>
    public class Request : IMessage
    {
        /// <inheritdoc cref="IMessage.TypeCode"/>
        public byte TypeCode
        {
            get { return (byte)Type; }
            set
            {
                if (!Enum.IsDefined(typeof(RequestTypes), value))
                    throw new ArgumentException();
                else
                    Type = (RequestTypes)value;
            }
        }

        /// <summary>
        /// Gets the type of the request.
        /// </summary>
        public RequestTypes Type { get; set; }

        /// <summary>
        /// Gets the content of the request.
        /// </summary>
        public byte[] Body { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Request"/> class.
        /// </summary>
        public Request()
            : this(RequestTypes.Echo)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Request"/> class.
        /// </summary>
        /// <param name="type">The type of the request.</param>
        /// <param name="body">The body of the request.</param>
        public Request(RequestTypes type, byte[] body = null)
        {
            Type = type;

            if (body != null)
                Body = body;
            else
                Body = new byte[0];
        }
    }
}
