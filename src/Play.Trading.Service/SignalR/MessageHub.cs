using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Play.Trading.Service.StateMachines;

namespace Play.Trading.Service.SignalR
{
    public class MessageHub : Hub
    {
        [Authorize]
        public async Task SendStatusAsync(PurchaseState status)
        {
            if (Clients is not null)
            {
                await Clients.User(Context.UserIdentifier)
                   .SendAsync("ReceivePurchaseStatus", status);
            }

        }
    }
}