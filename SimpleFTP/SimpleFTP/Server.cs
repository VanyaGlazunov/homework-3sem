// Copyright (c) 2024
//
// Use of this source code is governed by an MIT license
// that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

namespace SimpleFTP;

using System.Net;
using System.Net.Sockets;
using System.Text;

/// <summary>
/// Represents server that provides simple file transportation protocol.
/// </summary>
/// <param name="port">Port that server will listen to.</param>
public class FTPServer(int port) : IDisposable
{
    private readonly TcpListener listener = new (IPAddress.Any, port);
    private readonly CancellationTokenSource cancellationTokenSource = new ();

    /// <summary>
    /// Starts listening for incoming server requests.
    /// </summary>
    public async void Start()
    {
        this.listener.Start();
        var tasks = new List<Task>();
        while (!this.cancellationTokenSource.IsCancellationRequested)
        {
            try
            {
                var client = await this.listener.AcceptTcpClientAsync(this.cancellationTokenSource.Token);
                tasks.Add(this.AddRequest(client));
            }
            catch (OperationCanceledException)
            {
                break;
            }
        }

        Task.WaitAll([.. tasks]);
        this.listener.Stop();
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

    private async Task AddRequest(TcpClient client)
    {
        using var stream = client.GetStream();
        using var reader = new StreamReader(stream);
        using var writer = new StreamWriter(stream) { AutoFlush = true };
        var data = await reader.ReadLineAsync();
        if (data != null)
        {
            if (data[0] == '1')
            {
                await this.ListRequest(writer, data[2..]);
            }

            if (data[0] == '2')
            {
                await this.GetRequest(writer, data[2..]);
            }
        }
    }

    private async Task ListRequest(StreamWriter writer, string path)
    {
        if (!Directory.Exists(path))
        {
            await writer.WriteAsync("-1 \n");
            return;
        }

        var list = Directory.GetFileSystemEntries(path);
        await writer.WriteAsync($"{list.Length}");
        foreach (var entry in list)
        {
            await writer.WriteAsync($" {entry} {Directory.Exists(entry)}");
        }

        await writer.WriteAsync("\n");
    }

    private async Task GetRequest(StreamWriter writer, string path)
    {
        if (!File.Exists(path))
        {
            await writer.WriteAsync("-1 \n");
            return;
        }

        var bytes = await File.ReadAllBytesAsync(path);
        await writer.WriteAsync($"{bytes.Length} {Encoding.UTF8.GetString(bytes)}\n");
    }
}
