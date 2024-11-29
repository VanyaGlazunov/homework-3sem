// Copyright (c) 2024
//
// Use of this source code is governed by an MIT license
// that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

namespace MyThreadPool.Tests;

public class Tests
{
    private static readonly int numberOfThreads = 10;
    private MyThreadPool threadPool = new(numberOfThreads);

    [SetUp]
    public void Setup()
    {
        threadPool = new(numberOfThreads);
    }

    [Test]
    [TestCase(1)]
    [TestCase(10)]
    [TestCase(20)]
    public void CheckIfThereIsExactlyNThreadsInPool(int n)
    {
        HashSet<Thread?> threads = [];
        threadPool = new(n);
        List<IMyTask<Thread>> tasks = [];
        ManualResetEvent manualResetEvent = new(false);
        for (var i = 0; i < 2 * n; ++i)
        {
            tasks.Add(threadPool.Submit(() =>
            {
                manualResetEvent.WaitOne();
                return Thread.CurrentThread;
            }));
        }
        manualResetEvent.Set();
        Thread.Sleep(100);
        threadPool.Shutdown();
        foreach (var task in tasks)
        {
            threads.Add(task.Result);
        }
        Assert.That(threads, Has.Count.EqualTo(n));
    }

    [Test]
    public void SubmitSimpleTaskReturnsExpectedResult()
    {
        var expected = 1;
        var task = threadPool.Submit(() => expected);
        Assert.That(task.Result, Is.EqualTo(expected));
    }

    [Test]
    public void SubmitMultipleTasksReturnsExpectedResult()
    {
        List<IMyTask<int>> tasks = [];
        for (var i = 0; i < threadPool.ThreadsCount; ++i)
        {
            var localI = i;
            var task = threadPool.Submit(() =>
            {
                return localI;
            });
            tasks.Add(task);
        }
        Thread.Sleep(100);
        for (var i = 0; i < threadPool.ThreadsCount; ++i)
        {
            Assert.That(tasks[i].Result, Is.EqualTo(i));
        }
    }

    [Test]
    public void SubmitTaskWithOneContinueTasksReturnsExpectedResult()
    {
        var task = threadPool.Submit(() => "1").ContinueWith(x => int.Parse(x!));
        Assert.That(task.Result, Is.EqualTo(1));
    }

    [Test]
    public void SubmitTaskWithMultipleContinueTasksReturnsExpectedResult()
    {
        List<IMyTask<int>> tasks = [];
        var task = threadPool.Submit(() => 2);
        for (var i = 0; i < threadPool.ThreadsCount; ++i)
        {
            var localI = i;
            var continueTask = task.ContinueWith(x => localI * x);
            tasks.Add(continueTask);
        }
        Thread.Sleep(100);
        for (var i = 0; i < threadPool.ThreadsCount; ++i)
        {
            Assert.That(tasks[i].Result, Is.EqualTo(2 * i));
        }

    }

    [Test]
    public void SubmitTaskWithChainedContinueTasksReturnsExpectedResult()
    {
        var task = threadPool.Submit(() => "1").ContinueWith(x => int.Parse(x!)).ContinueWith(x => 2 * x + 1);
        Thread.Sleep(50);
        var expected = 3;
        Assert.That(task.Result, Is.EqualTo(expected));
    }

    [Test]
    public void ResultFuncThrowsExceptionShouldThrowAgregateException()
    {
        var task = threadPool.Submit<int>(() => throw new DivideByZeroException());
        Assert.Throws<AggregateException>(() => { var result = task.Result; });
    }

    [Test]
    public void TasksSubmitedBerforeShutdownAreCompletedAfterShutdown()
    {
        List<IMyTask<int>> tasks = new();
        for (var i = 0; i < 2 * threadPool.ThreadsCount; ++i)
        {
            threadPool.Submit(() =>
            {
                Thread.Sleep(100);
                return 1;
            });
        }
        threadPool.Shutdown();
        foreach (var task in tasks)
        {
            Assert.That(task.IsCompleted, Is.True);
        }
    }

    [Test]
    public void SubmitAndContinueWithFromMultipleThreadsPerformsExpectedResults()
    {
        var expected = 10;
        var threadsCount = 6;
        var actual = new IMyTask<int>[threadsCount];
        var threads = new Thread[threadsCount];
        var manualResetEvent = new ManualResetEvent(false);

        for (var i = 0; i < threadsCount; ++i)
        {
            var localI = i;
            threads[i] = new(() =>
            {
                manualResetEvent.WaitOne();

                actual[localI] = threadPool.Submit(() => 5).ContinueWith(r => 2 * r);
            });
        }

        foreach (var thread in threads)
        {
            thread.Start();
        }

        manualResetEvent.Set();

        foreach (var thread in threads)
        {
            thread.Join();
        }

        Assert.That(actual.All(r => r.Result == expected), Is.True);
    }

    [Test]
    public async Task SubmitAndShutdownFromMultipleThreadsPerformsExpectedResults()
    {
        var manualResetEvent = new ManualResetEvent(false);
        var expected = 50 * 99;
        var actual = 0;

        var submitTask = Task.Run(() =>
        {
            manualResetEvent.WaitOne();
            return threadPool.Submit(() => Enumerable.Range(1, 100).Sum());
        });

        var firstShutdown = Task.Run(() =>
        {
            manualResetEvent.WaitOne();
            threadPool.Shutdown();
        });

        var secondShutdown = Task.Run(() =>
        {
            manualResetEvent.WaitOne();
            threadPool.Shutdown();
        });

        manualResetEvent.Set();

        try
        {
            actual = (await submitTask).Result;
        }
        catch (OperationCanceledException)
        {
            Assert.Pass();
        }

        Assert.That(actual, Is.EqualTo(expected));
    }
}