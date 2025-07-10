using Microsoft.Extensions.DependencyInjection;

namespace OrquestR.Behavior;

public sealed class OpenBehavior
{
	public OpenBehavior(Type openBehaviorType, ServiceLifetime serviceLifetime = ServiceLifetime.Transient)
	{
		ValidatePipelineBehaviorType(openBehaviorType);
		OpenBehaviorType = openBehaviorType;
		ServiceLifetime = serviceLifetime;
	}

	public Type OpenBehaviorType { get; }
	public ServiceLifetime ServiceLifetime { get; }

	private static void ValidatePipelineBehaviorType(Type openBehaviorType)
	{
		if (openBehaviorType == null)
			throw new ArgumentNullException(nameof(openBehaviorType), "Open behavior type cannot be null.");

		bool isValid = openBehaviorType.IsGenericTypeDefinition &&
					   openBehaviorType.GetInterfaces()
						   .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IPipelineBehavior<,>));

		if (!isValid)
			throw new InvalidOperationException($"The type '{openBehaviorType.Name}' must be an open generic implementing IPipelineBehavior<,>.");
	}
}