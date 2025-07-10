using System.Linq.Expressions;
using System.Collections.Concurrent;
using Microsoft.Extensions.DependencyInjection;

namespace OrquestR.Core;

public sealed class OrquestR : IOrquestR
{
	private readonly IServiceProvider _provider;

	private static readonly ConcurrentDictionary<(Type, Type), Delegate> _delegateCache = new();

	public OrquestR(IServiceProvider provider) => _provider = provider;

	public Task<Unit> Send(IRequest request, CancellationToken cancellationToken = default)
	{
		ArgumentNullException.ThrowIfNull(request);

		var requestType = request.GetType();
		var responseType = typeof(Unit);
		var handlerInterface = typeof(IRequestHandler<>).MakeGenericType(requestType);

		var handler = _provider.GetRequiredService(handlerInterface);
		var handlerKey = (requestType, responseType);

		var delegateFunc = (Func<object, object, CancellationToken, Task<Unit>>)_delegateCache.GetOrAdd(handlerKey, key =>
			CompileSingleDelegate(requestType));

		var behaviorType = typeof(IPipelineBehavior<,>).MakeGenericType(requestType, responseType);
		var behaviors = _provider.GetServices(behaviorType).Cast<object>().Reverse().ToList();

		RequestHandlerDelegate<Unit> next = () => delegateFunc(handler, request, cancellationToken);

		foreach (var behavior in behaviors)
		{
			var current = behavior;
			var nextCopy = next;
			next = () => ((dynamic)current).Handle((dynamic)request, nextCopy, cancellationToken);
		}

		return next();
	}

	public Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
	{
		ArgumentNullException.ThrowIfNull(request);

		var requestType = request.GetType();
		var responseType = typeof(TResponse);
		var handlerInterface = typeof(IRequestHandler<,>).MakeGenericType(requestType, responseType);

		var handler = _provider.GetRequiredService(handlerInterface);
		var handlerKey = (requestType, responseType);

		var delegateFunc = (Func<object, object, CancellationToken, Task<TResponse>>)_delegateCache.GetOrAdd(handlerKey, key =>
			CompileTypedDelegate<TResponse>(requestType));

		var behaviorType = typeof(IPipelineBehavior<,>).MakeGenericType(requestType, responseType);
		var behaviors = _provider.GetServices(behaviorType).Cast<object>().Reverse().ToList();

		RequestHandlerDelegate<TResponse> next = () => delegateFunc(handler, request, cancellationToken);

		foreach (var behavior in behaviors)
		{
			var current = behavior;
			var nextCopy = next;
			next = () => ((dynamic)current).Handle((dynamic)request, nextCopy, cancellationToken);
		}

		return next();
	}

	private static Delegate CompileSingleDelegate(Type requestType)
	{
		var handlerType = typeof(IRequestHandler<>).MakeGenericType(requestType);
		var handleMethod = handlerType.GetMethod("Handle")!;

		var handlerParam = Expression.Parameter(typeof(object), "handler");
		var requestParam = Expression.Parameter(typeof(object), "request");
		var cancellationParam = Expression.Parameter(typeof(CancellationToken), "cancellationToken");

		var castHandler = Expression.Convert(handlerParam, handlerType);
		var castRequest = Expression.Convert(requestParam, requestType);

		var call = Expression.Call(castHandler, handleMethod, castRequest, cancellationParam);

		var lambda = Expression.Lambda(typeof(Func<object, object, CancellationToken, Task<Unit>>), call, handlerParam, requestParam, cancellationParam);
		return lambda.Compile();
	}

	private static Delegate CompileTypedDelegate<TResponse>(Type requestType)
	{
		var handlerType = typeof(IRequestHandler<,>).MakeGenericType(requestType, typeof(TResponse));
		var handleMethod = handlerType.GetMethod("Handle")!;

		var handlerParam = Expression.Parameter(typeof(object), "handler");
		var requestParam = Expression.Parameter(typeof(object), "request");
		var cancellationParam = Expression.Parameter(typeof(CancellationToken), "cancellationToken");

		var castHandler = Expression.Convert(handlerParam, handlerType);
		var castRequest = Expression.Convert(requestParam, requestType);

		var call = Expression.Call(castHandler, handleMethod, castRequest, cancellationParam);

		var lambda = Expression.Lambda(typeof(Func<object, object, CancellationToken, Task<TResponse>>), call, handlerParam, requestParam, cancellationParam);
		return lambda.Compile();
	}
}
