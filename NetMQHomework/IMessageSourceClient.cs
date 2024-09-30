using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetMQHomework
{
    public interface IMessageSourceClient
    {
        void Connect();
        void SendMessage(string message);
        string ReceiveMessage();
        void Disconnect();
    }
}
