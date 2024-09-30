using ChatApp.Contracts;
using Infrastructure.Provider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static Infrastructure.Provider.MessageSource;

namespace Core
{
    public class ChatClient : ChatBase
    {
        private readonly User _user;
        private readonly IPEndPoint _serverEndpoint;
        private readonly IMessageSource _source;
        private IEnumerable<User> _users = [];

        public ChatClient(string username, IPEndPoint serverEndpoint, IMessageSource source)
        {
            _user = new User { Name = username };
            _serverEndpoint = serverEndpoint;
            _source = source;
        }

        public override async Task Start()
        {
            var join = new Message { Text = _user.Name, Command = Command.Join };
            await _source.Send(join, _serverEndpoint, CancelToken);

            Task.Run(Listener);

            while (!CancelToken.IsCancellationRequested)
            {
                string input = (await Console.In.ReadLineAsync()) ?? string.Empty;
                Message message;
                if (input.Trim().Equals("/exit", StringComparison.CurrentCultureIgnoreCase))
                {
                    message = new() { SenderId = _user.Id, Command = Command.Exit };
                }
                else
                {
                    message = new() { Text = input, SenderId = _user.Id, Command = Command.None };
                }

                await _source.Send(message, _serverEndpoint, CancelToken);
            }
        }

        protected override async Task Listener()
        {
            while (CancelToken.IsCancellationRequested)
            {
                try
                {
                    ReceiveResult result = await _source.Receive(CancelToken);
                    if(result.Message is null)
                    {
                        throw new Exception("Message is null");
                    }

                    if (result.Message.Command == Command.Join)
                    {
                        JoinHandler(result.Message);
                    }
                    else if (result.Message.Command == Command.Users)
                    {
                        UsersHandler(result.Message);
                    }
                    else if (result.Message.Command == Command.None)
                    {
                        MessageHandler(result.Message);
                    }
                }
                catch (Exception exc) 
                {
                    await Console.Out.WriteAsync(exc.Message);
                }
            }
        }

        private void JoinHandler(Message message)
        {
            _user.Id = message.RecipientId;
            Console.WriteLine("Join success");
        }

        private void UsersHandler(Message message)
        {
            _users = message.Users;
        }

        private void MessageHandler(Message message)
        {
            Console.WriteLine($"{_users.First(u => u.Id == message.SenderId)}: {message.Text}");
        }
    }
}
