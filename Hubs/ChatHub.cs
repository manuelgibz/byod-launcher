using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace ByodLauncher.Hubs
{
    public class ChatHub: Hub
    {
        public async Task SendTextMessage(string message, string username)
        {
            //save params in objectList here


            //receiving
            await Clients.All.SendAsync("ReceiveTextMessage", message, username);
        }
    }
}
