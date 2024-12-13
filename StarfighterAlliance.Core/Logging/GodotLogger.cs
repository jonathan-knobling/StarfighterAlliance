using Godot;
using Microsoft.Extensions.Logging;

namespace StarfighterAlliance.Core.Logging;

public class GodotLogger<T> : ILogger<T>
{
	public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception,
							Func<TState, Exception?, string> formatter)
	{
		string message = formatter(state, exception);

		switch (logLevel)
		{
			case LogLevel.Trace:
			case LogLevel.Debug:
				GD.Print(message);

				break;
			case LogLevel.Information:
				GD.Print(message);

				break;
			case LogLevel.Warning:
				GD.PrintErr("WARNING: " + message);

				break;
			case LogLevel.Error:
			case LogLevel.Critical:
				GD.PrintErr("ERROR: " + message);

				break;
			case LogLevel.None:
				break;
			default:
				GD.Print(message);

				break;
		}
	}

	public bool IsEnabled(LogLevel logLevel)
	{
		return true;
	}

	public IDisposable BeginScope<TState>(TState state) where TState : notnull
	{
		return new NullDisposable();
	}

	private class NullDisposable : IDisposable
	{
		public void Dispose()
		{
		}
	}
}