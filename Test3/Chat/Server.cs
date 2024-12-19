namespace Chat;

using System.Net;
using System.Net.Sockets;

/// <summary>
/// Represents server that can read and write messages to one clinet.
/// </summary>
/// <param name="port">Port to listen.</param>
public class ChatServer(int port) : IDisposable
{
    private readonly TcpListener listener = new (IPAddress.Any, port);
    private readonly CancellationTokenSource cancellationTokenSource = new ();

    /// <summary>
    /// Starts the server.
    /// </summary>
    public async void Start()
    {
        this.listener.Start();
        var client = await this.listener.AcceptTcpClientAsync(this.cancellationTokenSource.Token);
        var readTask = Task.Run(async () =>
        {
            while (!this.cancellationTokenSource.IsCancellationRequested)
            {
                var message = Console.ReadLine();
                if (message == "exist" || message == null)
                {
                    this.cancellationTokenSource.Cancel();
                    break;
                }

                await this.WriteMessage(message, client);
            }
        });
        var writeTask = Task.Run(async () =>
        {
            while (!this.cancellationTokenSource.IsCancellationRequested)
            {
                var message = await this.ReadMessage(client);
                Console.WriteLine(message);
            }
        });

        Task.WaitAll(readTask, writeTask);
    }

    /// <summary>
    /// Stops the server.
    /// </summary>
    public void Stop()
    {
        this.cancellationTokenSource.Cancel();
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        this.Stop();
        this.listener.Dispose();
    }

    private async Task WriteMessage(string message, TcpClient client)
    {
        var stream = client.GetStream();
        using var writer = new StreamWriter(stream) { AutoFlush = true };
        await writer.WriteLineAsync(message);
    }

    private async Task<string?> ReadMessage(TcpClient client)
    {
        var stream = client.GetStream();
        using var reader = new StreamReader(stream);
        return await reader.ReadLineAsync();
    }
}