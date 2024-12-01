// Copyright (c) 2024
//
// Use of this source code is governed by an MIT license
// that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

namespace SimpleFTP;

using System.Net;
using System.Net.Sockets;

/// <summary>
/// Represents client that provides simple file transportation protocol.
/// </summary>
public class FTPClient(IPEndPoint endPoint)
{
    private readonly IPEndPoint endPoint = endPoint;

    /// <summary>
    /// Lists all files and subdirectories of the specified directory on the server.
    /// </summary>
    /// <param name="path">Path to directory to perform list.</param>
    /// <returns>String containing server response.</returns>
    public async Task<string> List(string path) => await this.Send($"1 {path}\n");

    /// <summary>
    /// Get file from server.
    /// </summary>
    /// <param name="path">Path to the file.</param>
    /// <returns>String containing server response.</returns>
    public async Task<string> Get(string path) => await this.Send($"2 {path}\n");

    private async Task<string> Send(string request)
    {
        using var client = new TcpClient();
        await client.ConnectAsync(this.endPoint);
        var stream = client.GetStream();
        using var writer = new StreamWriter(stream) { AutoFlush = true };
        using var reader = new StreamReader(stream);
        await writer.WriteAsync(request);
        return await reader.ReadToEndAsync();
    }
}
