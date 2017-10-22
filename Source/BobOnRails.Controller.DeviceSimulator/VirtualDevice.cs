using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using BobOnRails.Controller.Communication;

namespace BobOnRails.Controller.DeviceSimulator
{
    public class VirtualDevice : IDisposable
    {
        private TcpListener tcpListener;
        private Communicator<Request, Response> communicator;
        private List<AccelerometerMeasurement> genericMeasurements;
        private volatile int nextPointIndex;
        private volatile bool sendMeasurements;

        public bool IsSendingMeasurements
        {
            get { return sendMeasurements; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VirtualDevice"/> class.
        /// </summary>
        public VirtualDevice(IPEndPoint endPoint, Path motionPath)
        {
            genericMeasurements = motionPath.Select(p => new AccelerometerMeasurement(p.Time, p.Acceleration)).ToList();

            tcpListener = new TcpListener(endPoint);
            tcpListener.Start();
            tcpListener.BeginAcceptTcpClient(AcceptClient, null);
        }

        /// <inheritdoc cref="IDisposable.Dispose()" />
        public void Dispose()
        {
            sendMeasurements = false;

            if (tcpListener != null)
                tcpListener.Stop();

            if (communicator != null)
            {
                lock (communicator)
                    communicator.Dispose();
            }
        }

        private void AcceptClient(IAsyncResult result)
        {
            communicator = new Communicator<Request, Response>(tcpListener.EndAcceptTcpClient(result));
            communicator.DataReceived += OnRequestReceived;

            tcpListener.Stop();
        }

        private void OnRequestReceived(object sender, MessageReceivedEventArgs<Request> e)
        {
            HandleRequest(e.Message);
        }

        private void SendAvailableMeasurements()
        {
            while (sendMeasurements)
            {
                Thread.Sleep(10);

                AccelerometerMeasurement nextPointToSend = null;
                lock (genericMeasurements)
                {
                    if (genericMeasurements.Count > nextPointIndex)
                        nextPointToSend = genericMeasurements[nextPointIndex];
                    else
                        nextPointToSend = null;
                }

                while (sendMeasurements && nextPointToSend != null && nextPointToSend.TimeStamp < DateTime.Now)
                {
                    lock (communicator)
                        communicator.SendMessage(new Response(ResponseTypes.AccelerometerData, DataEncoder.EncodeAccelerometerData(nextPointToSend)));

                    nextPointIndex++;

                    lock (genericMeasurements)
                    {
                        if (genericMeasurements.Count > nextPointIndex)
                            nextPointToSend = genericMeasurements[nextPointIndex];
                        else
                            nextPointToSend = null;
                    }
                }
            }
        }

        private void HandleRequest(Request request)
        {
            switch (request.Type)
            {
                case RequestTypes.Echo:
                    lock (communicator)
                        communicator.SendMessage(new Response(ResponseTypes.Echo, request.Body));
                    break;

                case RequestTypes.Disconnect:
                    tcpListener.Start();
                    tcpListener.BeginAcceptTcpClient(AcceptClient, null);
                    break;

                case RequestTypes.StartAccelerometerDataStream:
                    if (!sendMeasurements)
                    {
                        var timeOffset = DateTime.Now - genericMeasurements[0].TimeStamp;
                        foreach (var measurement in genericMeasurements)
                            measurement.TimeStamp += timeOffset;

                        nextPointIndex = 0;

                        var sendMeasurementsThread = new Thread(new ThreadStart(SendAvailableMeasurements));
                        sendMeasurements = true;
                        sendMeasurementsThread.Start();
                    }
                    else
                    {
                        lock (communicator)
                            communicator.SendMessage(new Response(ResponseTypes.Echo, Encoding.Default.GetBytes("Sending mode is already active!")));
                    }

                    break;

                case RequestTypes.StopAccelerometerDataStream:
                    sendMeasurements = false;
                    break;

                default:
                    throw new NotSupportedException("Request data set is of an unknown type!");
            }
        }
    }
}
