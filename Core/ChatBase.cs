using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public abstract class ChatBase
    {
        protected CancellationTokenSource CancelTokenSource { get; set; } = new CancellationTokenSource();
        protected CancellationToken CancelToken => CancelTokenSource.Token;

        protected abstract Task Listener();

        public abstract Task Start();
    }
}
