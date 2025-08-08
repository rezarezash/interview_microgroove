using Azure.Storage.Queues;
using System.Text;
using System.Text.Json;

namespace GetInitialFunctions.Services
{

    public interface IQueueService
    {
        Task<bool> PublishToQueueAsync<T>(string queueName, T message, CancellationToken cancellationToken)
            where T : class;
    }

    public sealed class AzureQueueService : IQueueService
    {
        private readonly QueueServiceClient _queueServiceClient;

        public AzureQueueService(QueueServiceClient queueServiceClient)
        {
            _queueServiceClient = queueServiceClient;
        }

        public async Task<bool> PublishToQueueAsync<T>(string queueName, T message,
            CancellationToken cancellationToken) where T : class
        {
            try
            {
                var queueClient = _queueServiceClient.GetQueueClient(queueName);
                await queueClient.CreateIfNotExistsAsync();

                var bytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));
                var seiralizedMessage = Convert.ToBase64String(bytes);
                var result = await queueClient.SendMessageAsync(seiralizedMessage, cancellationToken);

                return result?.Value != null;
            }
            catch (OperationCanceledException)
            {
                // Log exception here
                return false;
            }
            catch (Exception)
            {
                // Log exception here
                return false;
            }
        }
    }
}
