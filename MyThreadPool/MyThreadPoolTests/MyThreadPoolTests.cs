using NUnit.Framework.Constraints;

namespace MyThreadPool.Tests;

public class Tests
{
    private MyThreadPool threadPool;

    [SetUp]
    public void Setup()
    {
        threadPool = new (Environment.ProcessorCount);
    }

    [Test]
    [TestCase(1)]
    [TestCase(10)]
    [TestCase(20)]
    public void CheckIfThereIsExactlyNThreadsInPool(int n)
    {
        HashSet<Thread?> threads = [];
        threadPool = new (n);
        List<IMyTask<Thread>> tasks = [];
        for (var i = 0; i < 2 * n; ++i)
        {
            tasks.Add(threadPool.Submit(() => {
                Thread.Sleep(40);
                return Thread.CurrentThread;
            }));
        }
        Thread.Sleep(2000);
        threadPool.Shutdown();
        foreach (var task in tasks)
        {
            threads.Add(task.Result);
        }
        Assert.That(threads, Has.Count.EqualTo(n));
    }

    public void SubmitSimpleTaskReturnsExpectedResult() {
        var expected = 1;
        var task = threadPool.Submit(() => expected);
        Assert.That(task.Result, Is.EqualTo(expected));
    }

    public void SubmitMultipleTasksReturnsExpectedResult()
    {
        List<IMyTask<int>> tasks = [];
        for (var i = 0; i < threadPool.ThreadCount; ++i)
        {
            var localI = i;
            var task = threadPool.Submit(() => {
                Thread.Sleep(100);
                return localI;
            });
            tasks.Add(task);
        }
        Thread.Sleep(101);
        for (var i = 0; i < threadPool.ThreadCount; ++i)
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
        for (var i = 0; i < threadPool.ThreadCount; ++i)
        {
            var localI = i;
            var continueTask = task.ContinueWith(x => localI * x);
            tasks.Add(continueTask);
        }
        Thread.Sleep(101);
        for (var i = 0; i < threadPool.ThreadCount; ++i)
        {
            Assert.That(tasks[i].Result, Is.EqualTo(2 * i));
        }

    }

    [Test]
    public void SubmitTaskWithChainedContinueTasksReturnsExpectedResult()
    {
        var task = threadPool.Submit(() => "1").ContinueWith(x => int.Parse(x!)).ContinueWith(x => 2 * x + 1);
        Thread.Sleep(50);
        Assert.That(task.Result, Is.EqualTo(3));
    }

    [Test]
    public void ResultFuncThrowsExceptionShouldThrowAgregateException()
    {
        var task = threadPool.Submit<int>(() => throw new DivideByZeroException());
        Assert.Throws<AggregateException>(() => {var result = task.Result;});
    }

    [Test]
    public void TasksAfterShutdownAreCompleteOrThrowException()
    {
        List<IMyTask<int>> tasks = new ();
        var expected = 1;
        for (var i = 0; i < 2 * threadPool.ThreadCount; ++i)
        {
            threadPool.Submit(() => 
            {
                Thread.Sleep(100);
                return 1;
            });
        }
        Thread.Sleep(100);
        threadPool.Shutdown();
        foreach (var task in tasks)
        {
            try
            {
                var actual = task.Result;
                Assert.That(actual, Is.EqualTo(expected));
            }
            catch (OperationCanceledException)
            {
                Assert.Pass();
            }
        }
    }
}