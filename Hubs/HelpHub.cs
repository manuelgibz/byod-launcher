using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace ByodLauncher.Hubs
{
    public class HelpHub: Hub
    {
        public async Task SendMessage(string message, string username, string title)
        {
            //receiving
            await Clients.All.SendAsync("AddHelp", message, username, title);
        }
    }
}
