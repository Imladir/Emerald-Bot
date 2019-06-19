﻿using System;
using System.Collections.Generic;
using System.Text;

namespace EmeraldBot.Blazor
{
    public static class Urls
    {
        public const string BotHub = "http://localhost:5050/emeraldBot";
        public const string Login = "Account/Login";
        public const string Logout = "Account/Logout";

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
