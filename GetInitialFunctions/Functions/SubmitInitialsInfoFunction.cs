using Azure.Storage.Queues;
using GetInitialFunctions.Dtos;
using GetInitialFunctions.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace GetInitialFunctions.Functions
{
    public class SubmitInitialsInfoFunction
    {
        private readonly ILogger<SubmitInitialsInfoFunction> _logger;
        private readonly IInitialsSerivice _initialsSerivice;
        private readonly IQueueService _queueService;

        public SubmitInitialsInfoFunction(ILogger<SubmitInitialsInfoFunction> logger,
            IInitialsSerivice initialsSerivice, IQueueService queueService)
        {
            _logger = logger;
            _initialsSerivice = initialsSerivice;
            _queueService = queueService;
        }

        [Function("SubmitInitialsInfo")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post")]
            HttpRequest req, CancellationToken cancellationToken)
        {
            _logger.LogInformation("GetInitialsInfo started..");
            using var reader = new StreamReader(req.Body);
            var requestBody = await reader.ReadToEndAsync();

            if (string.IsNullOrEmpty(requestBody))
            {
                _logger.LogWarning("Empty request body provided!");
                return new BadRequestObjectResult(new { error = "Request body is required" });
            }

            var initials = JsonSerializer.Deserialize<InitialsDto>(requestBody, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            if (string.IsNullOrEmpty(initials.FirstName))
            {
                return new BadRequestObjectResult(nameof(initials.FirstName));
            }

            if (string.IsNullOrEmpty(initials.LastName))
            {
                return new BadRequestObjectResult(nameof(initials.LastName));
            }

            var result = await _initialsSerivice.InsertInitialsAsync(initials, cancellationToken);
            await _queueService.PublishToQueueAsync("initialsqueue",
                    InitialsQueueMessageDto.Create(initials.FirstName, initials.LastName, result), cancellationToken);

            return new OkObjectResult(result == 1 ? "Success" : "Failed");
        }
    }
}
