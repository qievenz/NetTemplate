
namespace ReactTSWithNetCoreTemplate.Core.Services
{
    public interface IChannelProcessorService<T>
    {
        void CompletePublishing();
        Task PublishMessageAsync(T message);
        Task StartProcessingAsync(int workerCount, Func<T, Task> processMessageAsync, TimeSpan? delay = null, CancellationToken cancellationToken = default);
    }
}
