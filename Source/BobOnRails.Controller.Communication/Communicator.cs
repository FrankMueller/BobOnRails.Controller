using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Runtime.InteropServices;

namespace BobOnRails.Controller.Communication
{
    /// <summary>
    /// A class providing the basic functionality for asyncronious communication
    /// to the measurement device using the TCP protocol.
    /// </summary>
    public class Communicator<TIncomingMessages, TOutgoingMessages> : IDisposable
        where TIncomingMessages : IMessage, new()
        where TOutgoingMessages : IMessage, new()
    {
        private static int messageHeaderSize = Marshal.SizeOf(typeof(byte)) + Marshal.SizeOf(typeof(int));

        private byte[] receiveBuffer;
        private int receiveChunkSize;
        private int incomingMessageBodySize;
        private TIncomingMessages incomingMessage;

        /// <summary>
        /// Gets the underlying <see cref="TcpClient"/>.
        /// </summary>
        public TcpClient TcpClient { get; private set; }

        /// <summary>
        /// Occurs when data was received (this might occur on a different thread).
        /// </summary>
        public event EventHandler<MessageReceivedEventArgs<TIncomingMessages>> DataReceived;

        /// <summary>
        /// Raises the <see cref="DataReceived"/> event.
        /// </summary>
        /// <param name="e">The <see cref="EventArgs"/> to send.</param>
        protected virtual void OnDataReceived(MessageReceivedEventArgs<TIncomingMessages> e)
        {
            DataReceived?.Invoke(this, e);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Communicator{T1,T2}"/> class.
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
        /// Sends the specified data syncroniously.
        /// </summary>
        /// <param name="message">The message to send.</param>
        public void SendMessage(TOutgoingMessages message)
        {
            var headerBytes = GetHeaderBytes(message);
            TcpClient.Client.Send(headerBytes.ToArray(), headerBytes.Count, SocketFlags.None);

            if (message.Body.Length > 0)
                TcpClient.Client.Send(message.Body, message.Body.Length, SocketFlags.None);
        }

        /// <summary>
        /// Sends the specified data asyncroniously.
        /// </summary>
        /// <param name="message">The message to send.</param>
        public void SendMessageAsync(TOutgoingMessages message)
        {
            var headerBytes = GetHeaderBytes(message);
            TcpClient.Client.BeginSend(headerBytes.ToArray(), 0, headerBytes.Count, SocketFlags.None, EndSendCallback, null);

            if (message.Body.Length > 0)
                TcpClient.Client.BeginSend(message.Body, 0, message.Body.Length, SocketFlags.None, EndSendCallback, null);
        }

        private void EndSendCallback(IAsyncResult result)
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
                if (incomingMessage != null)
                {
                    if (receiveBuffer.Length >= incomingMessageBodySize)
                    {
                        var data = new byte[incomingMessageBodySize];
                        Array.Copy(receiveBuffer, data, data.Length);

                        incomingMessage.Body = data;
                        OnDataReceived(new MessageReceivedEventArgs<TIncomingMessages>(incomingMessage));

                        var remainingByteCount = receiveBuffer.Length - data.Length;
                        if (remainingByteCount > 0)
                        {
                            var remainingByteBuffer = new byte[remainingByteCount];
                            Array.Copy(receiveBuffer, data.Length, remainingByteBuffer, 0, remainingByteCount);
                            receiveBuffer = remainingByteBuffer;
                        }
                        else
                            receiveBuffer = new byte[0];

                        incomingMessage = default(TIncomingMessages);
                        break;
                    }
                }
                else if (receiveBuffer.Length >= messageHeaderSize)
                {
                    incomingMessage = new TIncomingMessages();
                    incomingMessage.TypeCode = receiveBuffer[0];
                    incomingMessageBodySize = BitConverter.ToInt32(receiveBuffer, 1);

                    var remainingByteCount = receiveBuffer.Length - messageHeaderSize;
                    var remainingByteBuffer = new byte[remainingByteCount];
                    Array.Copy(receiveBuffer, messageHeaderSize, remainingByteBuffer, 0, remainingByteCount);
                    receiveBuffer = remainingByteBuffer;

                    if (incomingMessageBodySize == 0)
                    {
                        OnDataReceived(new MessageReceivedEventArgs<TIncomingMessages>(incomingMessage));
                        incomingMessage = default(TIncomingMessages);
                    }

                    break;
                }

            }
            while (true);

            StartReceive();
        }

        private static List<byte> GetHeaderBytes(TOutgoingMessages message)
        {
            var headerBytes = new List<byte>(messageHeaderSize);
            headerBytes.Add(message.TypeCode);
            headerBytes.AddRange(BitConverter.GetBytes(message.Body.Length));

            return headerBytes;
        }
    }
}