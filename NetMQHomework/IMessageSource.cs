using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetMQHomework
{
    public interface IMessageSource
    {
        void SendMessage(string message);
        string ReceiveMessage();
    }
}
