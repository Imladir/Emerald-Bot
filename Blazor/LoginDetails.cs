using System;
using System.Collections.Generic;
using System.Text;

namespace EmeraldBot.Blazor
{
    public class LoginDetails
    {
        public string Username { get; set; }
        public string Password { get; set; }

        public LoginDetails()
        {
            Username = "";
            Password = "";
        }
    }
}
