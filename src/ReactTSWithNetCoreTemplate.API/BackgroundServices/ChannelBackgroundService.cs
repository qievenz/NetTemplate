using ReactTSWithNetCoreTemplate.Core.Services;
using Serilog;

namespace ReactTSWithNetCoreTemplate.API.BackgroundServices
{
    public class ChannelBackgroundService<T> : BackgroundService
    {
        private readonly IChannelProcessorService<T> _channelDataService;

        public ChannelBackgroundService(IChannelProcessorService<T> channelDataService)
        {
            _channelDataService = channelDataService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _channelDataService.StartProcessingAsync(10000, ProcessMessage, cancellationToken: stoppingToken);
            _channelDataService.CompletePublishing();
        }

        private Task<T> ProcessMessage(T message)
        {
            Log.Information(message.ToString());

            return Task.FromResult(message);
        }
    }
}
