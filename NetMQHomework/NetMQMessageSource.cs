using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetMQ;
using NetMQ.Sockets;

namespace NetMQHomework
{
    public class NetMQMessageSource : IMessageSource, IMessageSourceClient
    {
        private readonly string _ipAddress;
        private readonly int _port;
        private PairSocket _socket;

        public NetMQMessageSource(string ipAddress, int port)
        {
            _ipAddress = ipAddress;
            _port = port;
        }

        public void SendMessage(string message)
        {
            _socket.SendFrame(message);
        }

        public string ReceiveMessage()
        {
            return _socket.ReceiveFrameString();
        }

        public void Connect()
        {
            _socket = new PairSocket();
            _socket.Connect($"tcp://{_ipAddress}:{_port}");
        }

        public void Disconnect()
        {
            _socket.Dispose();
        }
    }
}
