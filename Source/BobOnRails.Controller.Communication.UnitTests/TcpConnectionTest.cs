using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using NUnit.Framework;

namespace BobOnRails.Controller.Communication.UnitTests
{
    [TestFixture]
    public class TcpConnectionTest
    {
        [TestCase("ping", "pong")]
        [Parallelizable(ParallelScope.None)]
        public void SendsAndReceivesData(string request, string response)
        {
            string receivedRequest = null, receivedResponse = null;

            var listener = new TcpListener(IPAddress.Parse("127.0.0.1"), 5006);
            listener.Start();

            //Setup a server which response to a request of a client
            var server = new Communicator(new TcpClient("127.0.0.1", 5006));
            server.DataReceived += (sender, args) =>
            {
                receivedRequest = Encoding.Unicode.GetString(args.Response.Body);
                server.SendRequestAsync(new Request(RequestTypes.Text, Encoding.Unicode.GetBytes(response)));
            };

            var client = new Communicator(listener.AcceptTcpClient());
            client.DataReceived += (sender, args) =>
            {
                receivedResponse = Encoding.Unicode.GetString(args.Response.Body);
            };

            client.SendRequestAsync(new Request(RequestTypes.Text, Encoding.Unicode.GetBytes(request)));

            Thread.Sleep(50);

            Assert.IsNotNull(receivedRequest, "The server didn't get the request.");
            Assert.IsNotNull(receivedResponse, "The client received no response.");
            Assert.AreEqual(request, receivedRequest, $"The request received by the server was corrupted!");
            Assert.AreEqual(response, receivedResponse, $"The response received by the client was corrupted!");
        }
    }
}
