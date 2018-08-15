using Microsoft.AspNetCore.SignalR;
using System.Collections.Generic;
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
    }

    public class ChatHub : Hub<IChatClient>
    {
        private readonly static List<string> TypingNames = new List<string>();
        private readonly static List<Message> Messages = new List<Message>();

        public void PostMessage(string name, string message)
        {
            Messages.Add(new Message(name, message));
            Clients.All.MessagePosted(name, message);
        }

        public void StartedTyping(string name)
        {
            TypingNames.Add(name);
            Clients.All.StartedTyping(name);
        }

        public void StoppedTyping(string name)
        {
            TypingNames.Remove(name);
            Clients.All.StoppedTyping(name);
        }

        public override Task OnConnectedAsync()
        {
            foreach(var m in Messages)
            {
                Clients.Caller.MessagePosted(m.UserName, m.Text);
            }
            foreach(var name in TypingNames)
            {
                Clients.Caller.StartedTyping(name);
            }
            return base.OnConnectedAsync();
        }
    }
}