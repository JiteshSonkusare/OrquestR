using OrquestR.Behavior;
using Microsoft.Extensions.DependencyInjection;

namespace OrquestR.Extensions;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddOrquestR(this IServiceCollection services, Action<OrquestRConfig> configure)
	{
		var config = new OrquestRConfig();
		configure(config);

		var assemblies = config.Assemblies.Distinct().ToArray();

		// 1. Register all handler types
		var handlerTypes = assemblies
			.SelectMany(a => a.GetTypes())
			.Where(t => !t.IsAbstract && !t.IsInterface && t.GetInterfaces().Any(i =>
				(i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IRequestHandler<,>)) ||
				(i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IRequestHandler<>))))
			.ToList();

		foreach (var type in handlerTypes)
		{
			var interfaces = type.GetInterfaces().Where(i =>
				(i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IRequestHandler<,>)) ||
				(i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IRequestHandler<>)));

			foreach (var @interface in interfaces)
			{
				services.AddTransient(@interface, type);
			}
		}

		// 2. Register open generic pipeline behaviors
		foreach (var openBehavior in config.OpenBehaviors)
		{
			services.Add(new ServiceDescriptor(
				serviceType: typeof(IPipelineBehavior<,>),
				implementationType: openBehavior.OpenBehaviorType,
				lifetime: openBehavior.ServiceLifetime));
		}

		// 3. Register OrquestR sender
		services.AddScoped<IOrquestR, Core.OrquestR>();
		// Register and Map ISender to IOrquestR
		services.AddScoped<ISender>(sp => sp.GetRequiredService<IOrquestR>());

		return services;
	}
}
