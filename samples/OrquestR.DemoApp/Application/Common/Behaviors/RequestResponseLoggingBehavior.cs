using OrquestR;
using Domain.Configs;
using Shared.Wrapper;
using System.Text.Json;
using Microsoft.Extensions.Options;
using Application.Interfaces.Services;

namespace Application.Common.Behaviors;

public class RequestResponseLoggingBehavior<TRequest, TResponse>(
    IDateTimeService dateTimeService,
    IHttpContextAccessor httpContextAccessor,
	ILogger<RequestResponseLoggingBehavior<TRequest, TResponse>> logger,
	IOptions<BehaviourLoggingConfig> behaviourLoggingConfig)
    : IPipelineBehavior<TRequest, TResponse>
      where TRequest : IRequest<TResponse>
      where TResponse : Result
{
    private readonly  IDateTimeService _dateTimeService = dateTimeService;
	private readonly ILogger<RequestResponseLoggingBehavior<TRequest, TResponse>> _logger = logger;
	private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    private readonly BehaviourLoggingConfig _behaviourLoggingConfig = behaviourLoggingConfig.Value;

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var httpContext = _httpContextAccessor.HttpContext;

        var traceId = Guid.NewGuid();
        var timestamp = _dateTimeService.Now;

        if (_behaviourLoggingConfig.Enable)
        {
            // Request Logging
            var requstestUri = $"RequestUri: " +
                $"{httpContext?.Request.Method} " +
                $"{httpContext?.Request.Scheme}://{httpContext?.Request.Host}{httpContext?.Request.Path}   " +
                $"Time: {timestamp}";
            var requstestUriJson = JsonSerializer.Serialize(requstestUri);
            _logger.LogInformation($"Executing request: traceId: {traceId}: requestUri: {requstestUriJson}");

            var requestJson = JsonSerializer.Serialize(request);
            _logger.LogInformation($"Handling request traceId: {traceId}: request: {requestJson}");
        }

        var result = await next();

        //Error Logging
        if (result.IsFailure)
            _logger.LogError($"Request failure " +
                $"{typeof(TRequest).Name}, " +
                $"traceId: {traceId}, " +
                $"timeStamp: {timestamp}, " +
                $"error: {result.Error}");

        if (_behaviourLoggingConfig.Enable)
        {
            // Response Logging
            var responseJson = JsonSerializer.Serialize(result);
            _logger.LogInformation($"Response for traceId: {traceId}: response: {responseJson}");
        }

        return result;
    }
}