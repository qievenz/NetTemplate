using ReactTSWithNetCoreTemplate.Core.Services;
using Serilog;
using System.Threading.Channels;

namespace ReactTSWithNetCoreTemplate.Application.Services
{
    public class ChannelProcessorService<T> : IChannelProcessorService<T>
    {
        private readonly Channel<T> _channel;

        public ChannelProcessorService()
        {
            _channel = Channel.CreateUnbounded<T>(new UnboundedChannelOptions
            {
                SingleReader = false,
                SingleWriter = false
            });
        }

        /// <summary>
        /// Publishes a message to the channel asynchronously.
        /// </summary>
        /// <param name="message">The message to publish.</param>
        public async Task PublishMessageAsync(T message)
        {
            await _channel.Writer.WriteAsync(message);
        }

        /// <summary>
        /// Signals that no more messages will be published, allowing the channel to complete its reading.
        /// </summary>
        public void CompletePublishing()
        {
            _channel.Writer.Complete();
        }

        /// <summary>
        /// Starts processing messages by launching the configured number of worker tasks.
        /// </summary>
        /// <param name="cancellationToken">A token to cancel the processing.</param>
        public async Task StartProcessingAsync(int workerCount, Func<T, Task> processMessageAsync, TimeSpan? delay = null, CancellationToken cancellationToken = default)
        {
            if (workerCount <= 0)
                throw new ArgumentException("Worker count must be greater than 0.", nameof(workerCount));

            Task[] tasks = new Task[workerCount];

            for (int i = 0; i < workerCount; i++)
            {
                tasks[i] = Task.Run(async () =>
                {
                    await foreach (var message in _channel.Reader.ReadAllAsync(cancellationToken))
                    {
                        try
                        {
                            await processMessageAsync(message);
                        }
                        catch (Exception ex)
                        {
                            Log.Error($"Error processing message: {ex.Message}");
                        }
                    }
                }, cancellationToken);
            }

            if (delay != null)
                await Task.Delay(delay.Value);

            await Task.WhenAll(tasks);
        }
    }
}
