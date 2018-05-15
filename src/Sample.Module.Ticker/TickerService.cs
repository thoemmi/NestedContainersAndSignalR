using System;
using System.Threading;
using Microsoft.Extensions.Logging;
using Sample.Core;

namespace Sample.Module.Ticker
{
    public class TickerService : IApplicationStartable, IDisposable
    {
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private readonly INotificationService _notificationService;
        private readonly ILogger<TickerService> _logger;

        private Thread _thread;

        public TickerService(INotificationService notificationService, ILogger<TickerService> logger)
        {
            _notificationService = notificationService;
            _logger = logger;
        }


        public void Start()
        {
            _thread = new Thread(Tick)
            {
                Name = "Ticker",
                IsBackground = true
            };
            _thread.Start();
        }

        public void Dispose()
        {
            _cancellationTokenSource.Cancel();
            _thread.Join();
        }

        private void Tick()
        {
            while (!_cancellationTokenSource.Token.WaitHandle.WaitOne(TimeSpan.FromSeconds(1)))
            {
                var message = $"Tick at {DateTime.Now}";
                _logger.LogDebug(message);
                _notificationService.SendMessageAsync(message).Wait();
            }
        }
    }
}