using ChatApp.Contracts;
using System.Net;

namespace Infrastructure.Provider
{
    public partial class MessageSource
    {
        public record ReceiveResult(IPEndPoint EndPoint, Message Message);
    }
}
