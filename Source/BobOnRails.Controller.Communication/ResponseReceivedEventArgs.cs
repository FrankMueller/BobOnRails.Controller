using System;

namespace BobOnRails.Controller.Communication
{
    /// <summary>
    /// An <see cref="EventArgs"/> class holding a <see cref="Response"/>.
    /// </summary>
    public class ResponseReceivedEventArgs : EventArgs
    {
        /// <summary>
        /// The response.
        /// </summary>
        public Response Response { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ResponseReceivedEventArgs"/> class.
        /// </summary>
        /// <param name="response">The response.</param>
        public ResponseReceivedEventArgs(Response response)
        {
            Response = response;
        }
    }
}
