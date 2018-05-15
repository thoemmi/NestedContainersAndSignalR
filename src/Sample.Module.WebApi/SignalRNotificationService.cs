using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Sample.Core;

namespace Sample.Module.WebApi
{
    public class SignalRNotificationService : INotificationService
    {
        private readonly HubLifetimeManager<NotificationsHub> _hubLifetimeManager;

        public SignalRNotificationService(HubLifetimeManager<NotificationsHub> hubLifetimeManager)
        {
            _hubLifetimeManager = hubLifetimeManager;
        }

        public Task SendMessageAsync(string message)
        {
            return _hubLifetimeManager.SendAllAsync("message", new object[] {message});
        }
    }
}