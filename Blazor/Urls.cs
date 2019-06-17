using System;
using System.Collections.Generic;
using System.Text;

namespace EmeraldBot.Blazor
{
    public static class Urls
    {
        public const string Login = "api/login";

        public const string ListServers = "api/servers";
        public const string ListServerChannels = "api/servers/{id}/channels";
        public const string ListPC = "api/servers/characters/{id}";
        public const string GetPC = "api/characters/{id}";

        // Handle messages
        public const string Message = "api/messages/{id}";
        public const string ListMessages = "api/messages";
        public const string SendMessage = "api/messages/send";
        public const string EditMessage = "api/messages/edit";
        public const string DeleteMessage = "api/messages/delete";
    }
}
