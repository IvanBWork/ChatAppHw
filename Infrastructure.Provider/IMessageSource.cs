using ChatApp.Contracts;
using System.Net;
using static Infrastructure.Provider.MessageSource;

namespace Infrastructure.Provider
{
    public interface IMessageSource
    {
        Task<ReceiveResult> Receive(CancellationToken cancellationToken);
        Task Send(Message message, IPEndPoint endPoint, CancellationToken cancellationToken);
    }
}
