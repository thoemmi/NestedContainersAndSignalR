using System.Threading.Tasks;

namespace Sample.Core
{
    public interface INotificationService
    {
        Task SendMessageAsync(string message);
    }
}
