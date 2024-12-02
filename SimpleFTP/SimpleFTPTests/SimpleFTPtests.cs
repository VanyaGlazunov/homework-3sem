// Copyright (c) 2024
//
// Use of this source code is governed by an MIT license
// that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

namespace SimpleFTP.Test;

using System.Diagnostics;
using System.Net;

public class Tests
{
    private static readonly int port = 9999;
    private static readonly IPEndPoint EP = new (IPAddress.Loopback, port);
    private FTPServer server = new (port);
    private FTPClient client = new (EP);
    private readonly string testFilesPath = "TestFiles/";
    
    [OneTimeSetUp]
    public void Setup()
    {
        server.Start();
    }

    [OneTimeTearDown]
    public void TearDown()
    {
        server.Stop();
    }

    [Test]
    public void ListDirectoryExistsShouldReturnsExpectedResults()
    {
        var list = Directory.GetFileSystemEntries(testFilesPath);
        var expected = $"{list.Length}";
        foreach (var entry in list)
        {
            expected += $" {entry} {Directory.Exists(entry)}";
        }

        expected += "\n";

        var actual = client.List(testFilesPath);

        Assert.That(actual.Result, Is.EqualTo(expected));
    }

    [Test]
    public void GetFileExistsReturnsExpectedResult()
    {
        var expected = "2 42\n";
        var actual = client.Get(Path.Combine(testFilesPath, "42.txt"));

        Assert.That(actual.Result, Is.EqualTo(expected));
    }

    [Test]
    public void ListDirectoryNotExistReturnsMinusOne()
    {
        var NotADirectory = Path.Combine(testFilesPath, "NotADirectory");
        var actual = client.List(NotADirectory);
        var expected = "-1 \n";
        Assert.That(actual.Result, Is.EqualTo(expected));
    }

    [Test]
    public void GetFileNotExistReturnsMinusOne()
    {
        var NotAFile = Path.Combine(testFilesPath, "NotAFile");
        var actual = client.Get(NotAFile);
        var expected = "-1 \n";
        Assert.That(actual.Result, Is.EqualTo(expected));
    }

    [Test]
    public void GetEmptyFileReturnsZero()
    {
        var emptyFile = Path.Combine(testFilesPath, "empty.txt");
        var actual = client.Get(emptyFile);
        var expected = "0 \n";
        Assert.That(actual.Result, Is.EqualTo(expected));
    }

    [Test]
    public void ListGetFromMultipleClientsShouldReturnExpectedResults()
    {
        var manualResetEvent = new ManualResetEvent(false);
        var clientsCount = 10;
        var getResults = new string[clientsCount];
        var listResults = new string[clientsCount];
        var getpath = Path.Combine(testFilesPath, "42.txt");
        var listpath = Path.Combine(testFilesPath, "subdir1");
        var getExpected = "2 42\n";
        var listExpected = $"1 {Path.Combine(testFilesPath, "subdir1", "sub.txt")} False\n";
        var tasks = new Task[clientsCount];

        var stopWatch = new Stopwatch();
        var delay = 200;

        for (var i = 0; i < clientsCount; ++i)
        {
            var localI = i;
            tasks[i] = Task.Run(async () => {
                manualResetEvent.WaitOne();
                await Task.Delay(delay);
                var client = new FTPClient(EP);
                getResults[localI] = await client.Get(getpath);
                listResults[localI] = await client.List(listpath);
            });
        }

        stopWatch.Start();
        manualResetEvent.Set();

        Task.WaitAll(tasks);
        stopWatch.Stop();

        Assert.That(stopWatch.ElapsedMilliseconds, Is.AtMost(2 * delay));

        Assert.Multiple(() => {
            Assert.That(listResults.All(x => x == listExpected), Is.True);
            Assert.That(getResults.All(x => x == getExpected), Is.True);
        });
    }
}