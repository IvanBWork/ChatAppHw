using ChatApp.Contracts;
using Infrastructure.Persistence.Contexts;
using Infrastructure.Provider;
using Microsoft.EntityFrameworkCore;
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
    public class ChatServer : ChatBase
    {
        private readonly IMessageSource _source;
        private readonly ChatContext _context;
        private HashSet<User> _users = [];

        public ChatServer(IMessageSource source, ChatContext context)
        {
            _source = source;
            _context = context;
        }

        public override async Task Start()
        {
            await Task.CompletedTask;

            Console.WriteLine("The UDP client is waiting for messages...");

            Task.Run(Listener);
        }

        protected override async Task Listener()
        {
            while (CancelToken.IsCancellationRequested)
            {
                try
                {
                    ReceiveResult result = (await _source.Receive(CancelToken)) ??
                        throw new Exception("Message is null");

                    switch (result.Message!.Command)
                    {
                        case Command.None:
                            await MessageHandler(result);
                            break;
                        case Command.Join:
                            await JoinHandler(result);
                            break;
                        case Command.Exit:
                            await ExitHandler(result);
                            break;
                        case Command.Users:
                            break;
                        case Command.Confirm:
                            break;
                    }
                }
                catch (Exception exc)
                {
                    await Console.Out.WriteAsync(exc.Message);
                }
            }
        }

        private async Task MessageHandler(ReceiveResult result)
        {
            if (result.Message!.RecipientId < 0)
            {
                await SendAllAsync(result.Message);
            }
            else
            {
                await _source.Send(result.Message,
                _users.First(u => u.Id == result.Message.SenderId).EndPoint!,
                CancelToken);

                var recipientEndpoint = _users.FirstOrDefault(u => u.Id == result.Message.SenderId)?.EndPoint;
                if (recipientEndpoint is not null)
                {
                    await _source.Send(
                        result.Message,
                        recipientEndpoint,
                        CancelToken);
                }
            }
        }

        private async Task JoinHandler(ReceiveResult result)
        {
            User? user = _users.FirstOrDefault(u => u.Name == result.Message!.Text);
            if (user is null)
            {
                user = new User() { Name = result.Message!.Text };
                _users.Add(user);
            }
            user.EndPoint = result.EndPoint;

            await _source.Send(new Message() { Command = Command.Join, RecipientId = user.Id },
                user.EndPoint!, CancelToken);

            await SendAllAsync(new Message() { Command = Command.Confirm, Text = $"{user.Name} joined to chat" });
            await SendAllAsync(new Message() { Command = Command.Users, RecipientId = user.Id, Users = _users });

            var unreaded = await _context.Messages.Where(x => x.RecepientId == user.Id).ToListAsync();

            foreach (var message in unreaded)
            {
                await _source.Send(
                    Message.FromDomain(message),
                    user.EndPoint!,
                    CancelToken);
            }
        }

        private async Task SendAllAsync(Message message)
        {
            foreach (User user in _users)
            {
                await _source.Send(message,
                user.EndPoint!,
                CancelToken);
            }
        }

        private async Task ExitHandler(ReceiveResult result)
        {
            var user = User.FromDomain(await _context.Users.FirstAsync(x => x.Id == result.Message!.SenderId));
            user.LastOnline = DateTime.Now;
            await _context.SaveChangesAsync();

            _users.Remove(_users.First(u => u.Id == result.Message!.SenderId));
        }
    }
}
