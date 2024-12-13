using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace StarfighterAlliance.Core.Logging;

public static class LoggingExtensions
{
	public static IServiceCollection AddGodotLogger(this IServiceCollection services)
	{
		services.AddLogging(builder =>
		{
			builder.ClearProviders();
			builder.AddProvider(new GodotLoggerProvider());
		});

		return services;
	}
}