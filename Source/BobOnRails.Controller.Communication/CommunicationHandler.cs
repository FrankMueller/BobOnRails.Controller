using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace BobOnRails.Controller.Communication
{
    /// <summary>
    /// A class handling the communication to the measurement device. This includes
    /// connecting, disconnecting and buffering of received data.
    /// </summary>
    public class CommunicationHandler : IDisposable
    {
        private Communicator<Response, Request> communicator;
        private SynchronizationContext syncronizationContext;
        private Queue<AccelerometerMeasurement> accelerometerMeasurementQueue;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommunicationHandler"/> class.
        /// </summary>
        public CommunicationHandler(string hostname, int port)
        {
            syncronizationContext = SynchronizationContext.Current;

            accelerometerMeasurementQueue = new Queue<AccelerometerMeasurement>();

            communicator = new Communicator<Response, Request>(new TcpClient(hostname, port));
            communicator.DataReceived += OnDataReceived;
        }

        /// <inheritdoc cref="IDisposable.Dispose()" />
        public void Dispose()
        {
            communicator.Dispose();
        }

        /// <summary>
        /// Starts the measurement mode on the device.
        /// </summary>
        public void StartMeasuring()
        {
            communicator.SendMessageAsync(new Request(RequestTypes.StartAccelerometerDataStream));
        }

        /// <summary>
        /// Stops the measurement mode on the device.
        /// </summary>
        public void StopMeasuring()
        {
            communicator.SendMessageAsync(new Request(RequestTypes.StopAccelerometerDataStream));
        }

        /// <summary>
        /// Gets all available accelerometer measurements and removes them from the 
        /// receive buffer.
        /// </summary>
        /// <returns>The available accelerometer measurements.</returns>
        public List<AccelerometerMeasurement> GrabAccelerometerMeasurements()
        {
            lock(accelerometerMeasurementQueue)
            {
                var measurements = new List<AccelerometerMeasurement>(accelerometerMeasurementQueue.Count);
                for (int i = 0; i < accelerometerMeasurementQueue.Count; i++)
                    measurements.Add(accelerometerMeasurementQueue.Dequeue());

                return measurements;
            }
        }

        private void OnDataReceived(object sender, MessageReceivedEventArgs<Response> e)
        {
            try
            {
                HandleResponse(e.Message);
            }
            catch ( Exception ex)
            {
                syncronizationContext.Post(delegate { throw ex; }, null);
            }
        }

        private void HandleResponse(Response response)
        {
            switch (response.Type)
            {
                case ResponseTypes.Echo:
                case ResponseTypes.Disconnect:
                    break;
                case ResponseTypes.AccelerometerData:
                    var data = DataEncoder.DecodeAccelerometerData(response.Body);
                    lock (accelerometerMeasurementQueue)
                        accelerometerMeasurementQueue.Enqueue(data);
                    break;
                default:
                    throw new NotSupportedException("Received data set is of an unknown type!");
            }
        }
    }
}
