using Azure.Storage.Queues.Models;
using GetInitialFunctions.Dtos;
using GetInitialFunctions.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace GetInitialFunctions.Functions;

public class GetInitialsSvgFunction
{
    private readonly ILogger<GetInitialsSvgFunction> _logger;
    private readonly IInitialsSerivice _initialsSerivice;
    private readonly ITagDiscoveryApiService _tagDiscoveryApiService;

    public GetInitialsSvgFunction(ILogger<GetInitialsSvgFunction> logger,
        IInitialsSerivice initialsSerivice,
        ITagDiscoveryApiService tagDiscoveryApiService)
    {
        _logger = logger;
        _initialsSerivice = initialsSerivice;
        _tagDiscoveryApiService = tagDiscoveryApiService;
    }

    [Function(nameof(GetInitialsSvgFunction))]
    public async Task Run([QueueTrigger("initialsqueue", Connection = "AzureWebJobsStorage")] QueueMessage message
        , CancellationToken cancellationToken)
    {
        _logger.LogInformation("C# Queue trigger function processed: {messageText}", message.MessageText);
        try
        {
            var initials = JsonSerializer.Deserialize<InitialsQueueMessageDto>(message.MessageText);
            if (initials == null)
            {
                _logger.LogWarning("Message deserialization resulted in null object");
                return;
            }
            if (string.IsNullOrEmpty(initials.FullName))
            {
                _logger.LogWarning("FullName is null or empty in the message");
                return ;
            }

            var svg = await _tagDiscoveryApiService.GetInitialsSvgAsync(initials.FullName, cancellationToken);

            if (string.IsNullOrEmpty(svg))
            {
                _logger.LogWarning("Received empty SVG from TagDiscoveryApiService for name {FullName}", initials.FullName);
            }

            var updateResult = await _initialsSerivice.UpdateInitialSvgAsync(initials.Id, svg, cancellationToken);
            if (updateResult)
            {
                _logger.LogInformation("Successfully updated SVG for ID {Id}", initials.Id);               
            }
            else
            {
                _logger.LogWarning("Failed to update SVG for ID {Id}", initials.Id);
            }
           
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing queue message");
            throw;
        }
    }
}