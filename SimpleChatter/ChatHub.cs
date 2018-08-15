﻿using Microsoft.AspNetCore.SignalR;
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

    public class ChatHub : Hub
    {
        private static List<string> TypingNames = new List<string>();
        private static List<Message> Messages = new List<Message>();

        public void PostMessage(string name, string message)
        {
            Messages.Add(new Message(name, message));
            Clients.All.SendAsync("MessagePosted", name, message);
        }

        public void StartedTyping(string name)
        {
            TypingNames.Add(name);
            Clients.All.SendAsync("StartedTyping",name);
        }

        public void StoppedTyping(string name)
        {
            TypingNames.Remove(name);
            Clients.All.SendAsync("StoppedTyping",name);
        }

        public override Task OnConnectedAsync()
        {
            foreach(var m in Messages)
            {
                Clients.Caller.SendAsync("MessagePosted",m.UserName, m.Text);
            }
            foreach(var name in TypingNames)
            {
                Clients.Caller.SendAsync("StartedTyping",name);
            }
            return base.OnConnectedAsync();
        }
    }
}