using Domain.Configs;
using FluentValidation;
using System.Reflection;
using OrquestR.Extensions;
using Web.ExceptionHandler;
using Infrastructure.Context;
using Infrastructure.Services;
using Application.Common.Behaviors;
using Infrastructure.Respositories;
using Microsoft.EntityFrameworkCore;
using Application.Interfaces.Services;
using Application.Interfaces.Repositories;

namespace Application.Extensions;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddDependencies(this IServiceCollection services, IConfiguration configuration)
	{
		services.AddEndpointsApiExplorer();
		services.AddSwaggerGen();

		services.AddExceptionHandler<GlobalExceptionHandler>()
					   .AddProblemDetails();

		services.AddHttpContextAccessor();

		services.AddDbContext<ApplicationDBContext>(options =>
						options.UseSqlServer(configuration.GetConnectionString("SqlConnection")));

		services.AddTransient<IDateTimeService, SystemDateTimeService>()
				.AddTransient(typeof(IRepositoryAsync<,>), typeof(RepositoryAsync<,>))
				.AddTransient(typeof(IUnitOfWork<>), typeof(UnitOfWork<>))
				.AddTransient<IDateTimeService, SystemDateTimeService>();

		services
			.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly())
			.AddOrquestR(cfg =>
			{
				cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
				cfg.AddOpenBehavior(typeof(RequestResponseLoggingBehavior<,>));
				cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
			});

		services.AddLazyCache();

		services.AddOptions<BehaviourLoggingConfig>()
				.Bind(configuration.GetSection(BehaviourLoggingConfig.SectionName))
				.ValidateDataAnnotations();

		return services;
	}

	internal static IApplicationBuilder UseDependencies(this WebApplication app)
	{
		app.UseHttpsRedirection();
		app.UseExceptionHandler();

		if (app.Environment.IsDevelopment())
		{
			app.UseSwagger();
			app.UseSwaggerUI();
			app.Services.CreateScope().ServiceProvider.GetRequiredService<ApplicationDBContext>().Database.EnsureCreated();
		}
		return app;
	}
}