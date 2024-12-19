namespace Chat;

using System.Net;
using System.Net.Sockets;

/// <summary>
/// Represents chat client.
/// </summary>
public class ChatClient(IPEndPoint iPEndPoint) : IDisposable
{
    private readonly TcpClient tcpClient = new (iPEndPoint);
    private readonly CancellationTokenSource cancellationTokenSource = new ();

    /// <summary>
    /// Connects client to a server and starts chat.
    /// </summary>
    /// <param name="reader">TextReader to read meassages.</param>
    /// <param name="writer">TextWriter to write messages from server.</param>
    public void Connect(TextReader reader, TextWriter writer)
    {
        this.tcpClient.Connect(iPEndPoint);
        var readTask = Task.Run(async () =>
        {
            while (!this.cancellationTokenSource.IsCancellationRequested)
            {
                var message = await reader.ReadLineAsync();
                if (message == "exist" || message == null)
                {
                    this.cancellationTokenSource.Cancel();
                    break;
                }

                await this.WriteMessage(message);
            }
        });
        var writeTask = Task.Run(async () =>
        {
            while (!this.cancellationTokenSource.IsCancellationRequested)
            {
                var message = await this.ReadMessage();
                await writer.WriteLineAsync(message);
            }
        });

        Task.WaitAll(readTask, writeTask);
    }

    /// <summary>
    /// Stops client.
    /// </summary>
    public void Stop()
    {
        this.cancellationTokenSource.Cancel();
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        this.Stop();
        this.tcpClient.Dispose();
    }

    private async Task WriteMessage(string message)
    {
        var stream = this.tcpClient.GetStream();
        using var writer = new StreamWriter(stream) { AutoFlush = true };
        await writer.WriteLineAsync(message);
    }

    private async Task<string?> ReadMessage()
    {
        var stream = this.tcpClient.GetStream();
        using var reader = new StreamReader(stream);
        return await reader.ReadLineAsync();
    }
}