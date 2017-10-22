using System;

namespace BobOnRails.Controller.Communication
{
    /// <summary>
    /// An <see cref="EventArgs"/> class holding a <see cref="Message"/>.
    /// </summary>
    public class MessageReceivedEventArgs<TMessage> : EventArgs
        where TMessage : IMessage
    {
        /// <summary>
        /// The received message.
        /// </summary>
        public TMessage Message { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageReceivedEventArgs{T}"/> class.
        /// </summary>
        /// <param name="message">The response.</param>
        public MessageReceivedEventArgs(TMessage message)
        {
            Message = message;
        }
    }
}
