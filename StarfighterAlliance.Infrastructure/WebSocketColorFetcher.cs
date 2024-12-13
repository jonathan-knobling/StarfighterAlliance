using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;
using StarfighterAlliance.Core.Spaceship.Color;

namespace StarfighterAlliance.Infrastructure;

/// <summary>
///     Fetches color data from a WebSocket server.
///     Implements <see cref="IColorFetcher" /> to provide colors fetched from a remote WebSocket server.
///     Handles communication, response parsing, and error logging.
/// </summary>
public class WebSocketColorFetcher : IColorFetcher, IDisposable
{
	private const int ServerResponseMaxByteSize = 256;
	private const string JsonRequest = "{\n \"messageType\": \"COLOR_REQUEST\" \n}";
	private readonly ClientWebSocket client = new();
	private readonly ILogger<WebSocketColorFetcher> logger;

	private readonly Uri uri;

	private bool disposed;

	/// <summary>
	///     Initializes a new instance of the <see cref="WebSocketColorFetcher" /> class.
	/// </summary>
	/// <param name="logger">Logger instance for logging errors and events.</param>
	/// <param name="uri">
	///     Optional URI to override the default WebSocket server address.
	///     If not provided, the default server URI <c>wss://softwaregrund.pro/jekt/ws/color</c> is used.
	/// </param>
	public WebSocketColorFetcher(ILogger<WebSocketColorFetcher> logger, Uri? uri = null)
	{
		this.logger = logger;
		this.uri = uri ?? new Uri("wss://softwaregrund.pro/jekt/ws/color");
	}

	/// <summary>
	///     Establishes a connection with the WebSocket server and requests a random color.
	/// </summary>
	/// <param name="cancellationToken">
	///     A cancellation token for terminating the operation early, if needed.
	/// </param>
	/// <returns>
	///     A string representing the name of the color received from the server.
	///     Possible values include RED, YELLOW, ORANGE, GREEN, VIOLET, WHITE, BLACK, TRANSPARENT.
	/// </returns>
	/// <exception cref="ObjectDisposedException">
	///     Thrown if the fetcher has already been disposed.
	/// </exception>
	/// <exception cref="WebSocketException">
	///     Thrown if there is an error during the WebSocket communication.
	/// </exception>
	public async Task<string> FetchColorAsync(CancellationToken cancellationToken = default)
	{
		ObjectDisposedException.ThrowIf(disposed, this);

		await client.ConnectAsync(uri, cancellationToken);
		await SendColorRequestAsync(cancellationToken);
		(byte[] buffer, WebSocketReceiveResult? result) = await ReceiveResultAsync(cancellationToken);
		await client.CloseAsync(WebSocketCloseStatus.NormalClosure, statusDescription: "Request completed.",
								cancellationToken);

		string responseMessage = Encoding.UTF8.GetString(buffer, 0, result.Count);

		if (!TryGetColor(responseMessage, out string colorName, out string errorMessage))
		{
			logger.LogError("Could not retreive color data from websocket connection: {error}", errorMessage);
		}

		return colorName;
	}

	public void Dispose()
	{
		if (disposed)
		{
			return;
		}

		client.Dispose();
		disposed = true;
	}

	/// <summary>
	///     Sends a predefined JSON request to the WebSocket server.
	/// </summary>
	/// <param name="cancellationToken">A cancellation token for terminating the operation early.</param>
	private async Task<(byte[] buffer, WebSocketReceiveResult result)> ReceiveResultAsync(
		CancellationToken cancellationToken)
	{
		var buffer = new byte[ServerResponseMaxByteSize];
		var responseSegment = new ArraySegment<byte>(buffer);
		WebSocketReceiveResult? result = await client.ReceiveAsync(responseSegment, cancellationToken);

		return (buffer, result);
	}

	/// <summary>
	///     Receives the response from the WebSocket server.
	/// </summary>
	/// <param name="cancellationToken">A cancellation token for terminating the operation early.</param>
	/// <returns>A tuple containing the response buffer and the WebSocket receive result.</returns>
	private async Task SendColorRequestAsync(CancellationToken cancellationToken)
	{
		byte[] requestBytes = Encoding.UTF8.GetBytes(JsonRequest);
		var requestSegment = new ArraySegment<byte>(requestBytes);
		await client.SendAsync(requestSegment, WebSocketMessageType.Text, true, cancellationToken);
	}

	/// <summary>
	///     Parses the WebSocket server's response to extract color data.
	/// </summary>
	/// <param name="responseMessage">The raw JSON response message from the server.</param>
	/// <param name="colorName">The name of the color extracted from the response.</param>
	/// <param name="errorMessage">An error message if the response parsing fails.</param>
	/// <returns>True if the response was successfully parsed; otherwise, false.</returns>
	private static bool TryGetColor(string responseMessage, out string colorName, out string errorMessage)
	{
		try
		{
			var response = JsonSerializer.Deserialize<Response>(responseMessage);

			if (response?.Code == ResponseCode.Ok)
			{
				colorName = response.ColorName!;
				errorMessage = "";

				return true;
			}

			colorName = "";
			errorMessage = $"{response?.Code}: {response?.ErrorMessage}";

			return false;
		}
		catch (Exception e)
		{
			colorName = "";
			errorMessage = e.Message;

			return false;
		}
	}

	#region Types

	/// <summary>
	///     Represents a response message from the WebSocket server.
	/// </summary>
	private class Response
	{
		[JsonPropertyName("code")] public ResponseCode Code { get; set; }
		[JsonPropertyName("color")] public string? ColorName { get; set; }
		[JsonPropertyName("error")] public string? ErrorMessage { get; set; }
	}

	/// <summary>
	///     Represents the status codes returned by the WebSocket server.
	/// </summary>
	private enum ResponseCode
	{
		Ok = 200,
		BadRequest = 400,
		InternalServerError = 500
	}

	#endregion
}