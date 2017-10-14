using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Threading;

namespace BobOnRails.Controller.Communication
{
    public class Communicator
    {
        private static int responseHeaderSize = Marshal.SizeOf(typeof(byte)) + Marshal.SizeOf(typeof(int));

        private byte[] receiveBuffer;
        private int receiveChunkSize;
        private int incomingResponseBodySize;
        private Response incomingResponse;
        private SynchronizationContext syncronizationContext;

        /// <summary>
        /// Gets the underlying <see cref="TcpClient"/>.
        /// </summary>
        public TcpClient TcpClient { get; private set; }

        /// <summary>
        /// Occurs if data was received.
        /// </summary>
        public event EventHandler<ResponseReceivedEventArgs> DataReceived;

        /// <summary>
        /// Raises the <see cref="DataReceived"/> event.
        /// </summary>
        /// <param name="e">The <see cref="EventArgs"/> to send.</param>
        protected virtual void OnDataReceived(ResponseReceivedEventArgs e)
        {
            if (syncronizationContext != null)
                syncronizationContext.Post(delegate { DataReceived?.Invoke(this, e); }, null);
            else
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
            syncronizationContext = SynchronizationContext.Current;

            StartReceive();
        }

        /// <summary>
        /// Sends the specified data asyncroniously.
        /// </summary>
        /// <param name="bytes">The data to send.</param>
        public void SendRequestAsync(Request request)
        {
            var headerBytes = new List<byte>();
            headerBytes.Add((byte)request.Type);
            headerBytes.AddRange(BitConverter.GetBytes(request.Body.Length));
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
                else if (receiveBuffer.Length >= responseHeaderSize)
                {
                    incomingResponse = new Response((ResponseTypes)receiveBuffer[0]);
                    incomingResponseBodySize = BitConverter.ToInt32(receiveBuffer, 1);

                    var remainingByteCount = receiveBuffer.Length - responseHeaderSize;
                    var remainingByteBuffer = new byte[remainingByteCount];
                    Array.Copy(receiveBuffer, responseHeaderSize, remainingByteBuffer, 0, remainingByteCount);
                    receiveBuffer = remainingByteBuffer;
                    break;
                }

            }
            while (true);

            StartReceive();
        }
    }
}