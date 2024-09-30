using ChatApp.Contracts;
using ChatApp.Contracts.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Provider
{
    public partial class MessageSource : IMessageSource
    {
        private readonly UdpClient _udpClient;

        public MessageSource(UdpClient udpClient) 
        {
            _udpClient = udpClient;
        }

        public async Task<ReceiveResult> Receive(CancellationToken cancellationToken)
        {
            var data = await _udpClient.ReceiveAsync(cancellationToken);
            return new(data.RemoteEndPoint, data.Buffer.ToMessage());
        }

        public async Task Send(Message message, IPEndPoint endPoint, CancellationToken cancellationToken)
        {
            await _udpClient.SendAsync(message.ToBytes(), endPoint, cancellationToken);
        }
    }
}
