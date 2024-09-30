using Core;
using Infrastructure.Persistence.Contexts;
using Infrastructure.Provider;
using Microsoft.Identity.Client;
using System.Net;
using System.Net.Sockets;

namespace ChatApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            IPEndPoint serverEndPoint = new (IPAddress.Parse("127.0.0.1"), 12345);
            IMessageSource source;

            if (args.Length == 0)
            {
                //server
                source = new MessageSource(new UdpClient(serverEndPoint));
                var serv = new ChatServer(source, new ChatContext());
                serv.Start();
            }
            else
            {
                //client
                source = new MessageSource(new UdpClient());
                var chat = new ChatClient(args[0], serverEndPoint, source);
                chat.Start();
            }
        }
    }
}
