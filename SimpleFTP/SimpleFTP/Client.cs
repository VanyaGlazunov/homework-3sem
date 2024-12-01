using System.Formats.Asn1;
using System.Net;
using System.Net.Sockets;

namespace SimpleFTP;

/// <summary>
/// Represents client that provides simple file transportation protocol.
/// </summary>
public class FTPClient : IDisposable
{
    private readonly TcpClient client;
    private readonly StreamWriter writer;
    private readonly StreamReader reader;

    /// <summary>
    /// Initializes a new instance of the <see cref="FTPClient"/> class and binds it to the specified local endpoint.
    /// </summary>
    /// <param name="localEP">Host and port of the server.</param>
    public FTPClient(IPEndPoint localEP)
    {
        this.client = new (localEP);
        this.writer = new (this.client.GetStream()) { AutoFlush = true };
        this.reader = new (this.client.GetStream());
    }

    /// <summary>
    /// Lists all files and subdirectories of the specified directory on the server.
    /// </summary>
    /// <param name="path">Path to directory to perform list.</param>
    /// <returns>String containing server response.</returns>
    public async Task<string> List(string path)
    {
        await this.writer.WriteLineAsync($"1 {path}");
        return await this.reader.ReadToEndAsync();
    }

    /// <summary>
    /// Get file from server.
    /// </summary>
    /// <param name="path">Path to the file.</param>
    /// <returns>String containing server response.</returns>
    public async Task<string> Get(string path)
    {
        await this.writer.WriteLineAsync($"2 {path}");
        return await this.reader.ReadToEndAsync();
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        this.client.Dispose();
        this.writer.Dispose();
        this.reader.Dispose();
    }
}
