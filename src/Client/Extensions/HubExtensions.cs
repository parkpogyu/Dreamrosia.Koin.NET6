using Dreamrosia.Koin.Shared.Constants.Application;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;

namespace Dreamrosia.Koin.Client.Extensions
{
    public static class HubExtensions
    {
        public static HubConnection TryInitialize(this HubConnection hubConnection, NavigationManager navigationManager)
        {
            if (hubConnection == null)
            {
                hubConnection = new HubConnectionBuilder()
                                  .WithUrl(navigationManager.ToAbsoluteUri(ApplicationConstants.SignalR.HubUrl))
                                  .Build();
            }
            return hubConnection;
        }

        public static HubConnection TryInitializeToSynchronize(this HubConnection hubConnection, NavigationManager navigationManager, string userId)
        {
            return new HubConnectionBuilder()
                      .WithUrl(navigationManager.ToAbsoluteUri($"{ApplicationConstants.SynchronizeSignalR.HubUrl}?guid={userId}"))
                      .Build();
        }
    }
}