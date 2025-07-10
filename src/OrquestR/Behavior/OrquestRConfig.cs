using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace OrquestR.Behavior;

public sealed class OrquestRConfig
{
	internal List<Assembly> Assemblies { get; } = new();
	internal List<OpenBehavior> OpenBehaviors { get; } = new();

	public void RegisterServicesFromAssembly(Assembly assembly)
	{
		if (!Assemblies.Contains(assembly))
			Assemblies.Add(assembly);
	}

	public void AddOpenBehavior(Type openBehaviorType, ServiceLifetime lifetime = ServiceLifetime.Transient)
	{
		OpenBehaviors.Add(new OpenBehavior(openBehaviorType, lifetime));
	}
}

