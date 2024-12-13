using Microsoft.Extensions.Logging;

namespace StarfighterAlliance.Core.Logging;

public class GodotLoggerProvider : ILoggerProvider
{
	public ILogger CreateLogger(string categoryName)
	{
		Type? loggerType = Type.GetType(categoryName) ?? typeof(object);

		return (ILogger)Activator.CreateInstance(typeof(GodotLogger<>).MakeGenericType(loggerType))!
			   ?? throw new InvalidOperationException(message: "Could not create instance of GodotLogger");
	}

	public void Dispose()
	{
	}
}