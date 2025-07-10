namespace OrquestR;

public interface IPipelineBehavior<in TRequest, TResponse>
		where TRequest : IBaseRequest
{
	Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken);
}