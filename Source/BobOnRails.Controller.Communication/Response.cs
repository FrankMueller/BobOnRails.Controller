using System;

namespace BobOnRails.Controller.Communication
{
    /// <summary>
    /// Represents a request which can be send to the remote device.
    /// </summary>
    public class Response : IMessage
    {
        /// <inheritdoc cref="IMessage.TypeCode"/>
        public byte TypeCode
        {
            get { return (byte)Type; }
            set
            {
                if (!Enum.IsDefined(typeof(ResponseTypes), value))
                    throw new ArgumentException();
                else
                    Type = (ResponseTypes)value;
            }
        }

        /// <summary>
        /// Gets the type of the response.
        /// </summary>
        public ResponseTypes Type { get; set; }

        /// <summary>
        /// Gets the content of the response.
        /// </summary>
        public byte[] Body { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Response"/> class.
        /// </summary>
        public Response()
            : this(ResponseTypes.Echo)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Response"/> class.
        /// </summary>
        /// <param name="type">The type of the response.</param>
        /// <param name="body">The body of the response.</param>
        public Response(ResponseTypes type = ResponseTypes.Echo, byte[] body = null)
        {
            Type = type;

            if (body != null)
                Body = body;
            else
                Body = new byte[0];
        }
    }
}
