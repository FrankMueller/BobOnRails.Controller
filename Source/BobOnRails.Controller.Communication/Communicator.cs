using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Threading;

namespace BobOnRails.Controller.Communication
{
    /// <summary>
    /// A class providing the basic functionality for asyncronious communication
    /// to the measurement device using the TCP protocol.
    /// </summary>
    public class Communicator : IDisposable
    {
        private static int messageHeaderSize = Marshal.SizeOf(typeof(byte)) + Marshal.SizeOf(typeof(int));

        private byte[] receiveBuffer;
        private int receiveChunkSize;
        private int incomingResponseBodySize;
        private Response incomingResponse;

        /// <summary>
        /// Gets the underlying <see cref="TcpClient"/>.
        /// </summary>
        public TcpClient TcpClient { get; private set; }

        /// <summary>
        /// Occurs when data was received (this might occur on a different thread).
        /// </summary>
        public event EventHandler<ResponseReceivedEventArgs> DataReceived;

        /// <summary>
        /// Raises the <see cref="DataReceived"/> event.
        /// </summary>
        /// <param name="e">The <see cref="EventArgs"/> to send.</param>
        protected virtual void OnDataReceived(ResponseReceivedEventArgs e)
        {
            DataReceived?.Invoke(this, e);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Communicator"/> class.
        /// </summary>
        /// <param name="tcpClient">The underlying connection.</param>
        /// <param name="receiveChunkSize">The size of a data chunk.</param>
        public Communicator(TcpClient tcpClient, int receiveChunkSize = 1024)
        {
            TcpClient = tcpClient;

            this.receiveBuffer = new byte[0];
            this.receiveChunkSize = receiveChunkSize;

            StartReceive();
        }

        /// <inheritdoc cref="IDisposable.Dispose()" />
        public void Dispose()
        {
            TcpClient.Close();
        }

        /// <summary>
        /// Disconnects from the device.
        /// </summary>
        public void Disconnect()
        {
            SendRequest(new Request(RequestTypes.Disconnect));
        }

        /// <summary>
        /// Sends the specified data syncroniously.
        /// </summary>
        /// <param name="request">The request to send.</param>
        public void SendRequest(Request request)
        {
            var headerBytes = GetHeaderBytes(request);
            TcpClient.Client.Send(headerBytes.ToArray(), headerBytes.Count, SocketFlags.None);

            if (request.Body.Length > 0)
                TcpClient.Client.Send(request.Body, request.Body.Length, SocketFlags.None);
        }

        /// <summary>
        /// Sends the specified data asyncroniously.
        /// </summary>
        /// <param name="request">The request to send.</param>
        public void SendRequestAsync(Request request)
        {
            var headerBytes = GetHeaderBytes(request);
            TcpClient.Client.BeginSend(headerBytes.ToArray(), 0, headerBytes.Count, SocketFlags.None, EndSend, null);

            if (request.Body.Length > 0)
                TcpClient.Client.BeginSend(request.Body, 0, request.Body.Length, SocketFlags.None, EndSend, null);
        }

        private void EndSend(IAsyncResult result)
        {
            TcpClient.Client.EndSend(result);
        }

        private void StartReceive()
        {
            var buffer = new byte[receiveChunkSize];
            TcpClient.Client.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, DataReceivedCallback, buffer);
        }

        private void DataReceivedCallback(IAsyncResult result)
        {
            var bytesRead = TcpClient.Client.EndReceive(result);
            var bytesReceived = result.AsyncState as byte[];

            var currentBufferSize = receiveBuffer.Length;
            Array.Resize(ref receiveBuffer, receiveBuffer.Length + bytesRead);
            Array.Copy(bytesReceived, 0, receiveBuffer, currentBufferSize, bytesRead);

            do
            {
                if (incomingResponse != null)
                {
                    if (receiveBuffer.Length >= incomingResponseBodySize)
                    {
                        var data = new byte[incomingResponseBodySize];
                        Array.Copy(receiveBuffer, data, data.Length);

                        incomingResponse.Body = data;
                        OnDataReceived(new ResponseReceivedEventArgs(incomingResponse));

                        var remainingByteCount = receiveBuffer.Length - data.Length;
                        if (remainingByteCount > 0)
                        {
                            var remainingByteBuffer = new byte[remainingByteCount];
                            Array.Copy(receiveBuffer, data.Length, remainingByteBuffer, 0, remainingByteCount);
                            receiveBuffer = remainingByteBuffer;
                        }
                        else
                            receiveBuffer = new byte[0];

                        incomingResponse = null;
                        break;
                    }
                }
                else if (receiveBuffer.Length >= messageHeaderSize)
                {
                    incomingResponse = new Response((ResponseTypes)receiveBuffer[0]);
                    incomingResponseBodySize = BitConverter.ToInt32(receiveBuffer, 1);

                    var remainingByteCount = receiveBuffer.Length - messageHeaderSize;
                    var remainingByteBuffer = new byte[remainingByteCount];
                    Array.Copy(receiveBuffer, messageHeaderSize, remainingByteBuffer, 0, remainingByteCount);
                    receiveBuffer = remainingByteBuffer;
                    break;
                }

            }
            while (true);

            StartReceive();
        }

        private static List<byte> GetHeaderBytes(Request request)
        {
            var headerBytes = new List<byte>(messageHeaderSize);
            headerBytes.Add((byte)request.Type);
            headerBytes.AddRange(BitConverter.GetBytes(request.Body.Length));

            return headerBytes;
        }
    }
}