using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleChatter
{
    public class Message
    {
        public readonly string UserName;
        public readonly string Text;

        public Message(string userName, string text)
        {
            UserName = userName;
            Text = text;
        }
    }

    public interface IChatClient
    {
        Task MessagePosted(string name, string message);
        Task StartedTyping(string name);
        Task StoppedTyping(string name);
        Task NameChanged(string oldName, string newName);
        Task Joined(string name);
        Task Left(string name);
        Task UserNameGiven(string name);
    }

    public class ChatHub : Hub<IChatClient>
    {
        private readonly static List<string> TypingConnectionIds = new List<string>();
        private readonly static List<Message> Messages = new List<Message>();
        private readonly static Dictionary<string, string> NameByConnectionId = new Dictionary<string, string>();

        public bool PostMessage(string message)
        {
            if (!NameByConnectionId.ContainsKey(Context.ConnectionId))
            {
                return false;
            }
            if (string.IsNullOrWhiteSpace(message))
            {
                return false;
            }
            var name = NameByConnectionId[Context.ConnectionId];
            Messages.Add(new Message(name, message));
            Clients.All.MessagePosted(name, message);
            return true;
        }

        public void StartedTyping()
        {
            if (!NameByConnectionId.ContainsKey(Context.ConnectionId))
            {
                return;
            }
            var name = NameByConnectionId[Context.ConnectionId];
            TypingConnectionIds.Add(Context.ConnectionId);
            Clients.Others.StartedTyping(name);
        }

        public void StoppedTyping()
        {
            if (!NameByConnectionId.ContainsKey(Context.ConnectionId))
            {
                return;
            }
            var name = NameByConnectionId[Context.ConnectionId];
            TypingConnectionIds.Remove(Context.ConnectionId);
            Clients.Others.StoppedTyping(name);
        }

        public bool ChangeName(string newName)
        {
            if (!NameByConnectionId.ContainsKey(Context.ConnectionId))
            {
                return false;
            }
            if (string.IsNullOrWhiteSpace(newName))
            {
                return false;
            }
            var nameAvailable = !NameByConnectionId.Any(pair => pair.Value == newName);
            if (!nameAvailable)
            {
                return false;
            }
            var oldName = NameByConnectionId[Context.ConnectionId];
            NameByConnectionId[Context.ConnectionId] = newName;
            Clients.Others.NameChanged(oldName, newName);
            return true;
        }

        public override Task OnConnectedAsync()
        {
            var userName = "User" + Guid.NewGuid().ToString().Replace("-", "").Substring(0, 4).ToUpper();
            NameByConnectionId[Context.ConnectionId] = userName;
            Clients.Others.Joined(userName);
            Clients.Caller.UserNameGiven(userName);
            foreach(var m in Messages)
            {
                Clients.Caller.MessagePosted(m.UserName, m.Text);
            }
            foreach(var id in TypingConnectionIds)
            {
                Clients.Caller.StartedTyping(NameByConnectionId[id]);
            }
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            var name = NameByConnectionId[Context.ConnectionId];
            if (TypingConnectionIds.Remove(Context.ConnectionId))
            {
                Clients.Others.StoppedTyping(NameByConnectionId[Context.ConnectionId]);
            }
            NameByConnectionId.Remove(Context.ConnectionId);
            Clients.Others.Left(name);
            return base.OnDisconnectedAsync(exception);
        }
    }
}