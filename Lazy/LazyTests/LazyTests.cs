// <copyright file="LazyTests.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Lazy.Tests;

public class Tests
{
    private static int supplierRunsCounter = 1;

    private static int expectedValue = 100;

    private static IEnumerable<TestCaseData> Lazies()
    {
        yield return new TestCaseData(new SingleThreadLazy<int>(() => expectedValue));
        yield return new TestCaseData(new MultiThreadLazy<int>(() => expectedValue));
    }

    private static IEnumerable<TestCaseData> LaziesCountRuns()
    {
        yield return new TestCaseData(new SingleThreadLazy<int>(() => supplierRunsCounter++));
        yield return new TestCaseData(new MultiThreadLazy<int>(() => supplierRunsCounter++));
    }

    private static IEnumerable<TestCaseData> LaziesThrowException()
    {
        Func<int> supplier = () =>
        {
            return 1 / (Environment.ProcessId - Environment.ProcessId);
        };

        yield return new TestCaseData(new SingleThreadLazy<int>(supplier));
        yield return new TestCaseData(new MultiThreadLazy<int>(supplier));
    }

    [TestCaseSource(nameof(Lazies))]
    public void GetShouldReturnExpectedValue(ILazy<int> lazy)
    {
        Assert.That(lazy.Get(), Is.EqualTo(expectedValue));
    }

    [TestCaseSource(nameof(LaziesCountRuns))]
    public void GetSeveralTimesSupplierShouldBeCalledOnce(ILazy<int> lazy)
    {
        var firstRun = lazy.Get();
        var secondRun = lazy.Get();
        var thirdRun = lazy.Get();
        Assert.Multiple(() =>
        {
            Assert.That(firstRun, Is.EqualTo(secondRun));
            Assert.That(secondRun, Is.EqualTo(thirdRun));
        });
    }

    [TestCaseSource(nameof(LaziesThrowException))]
    public void GetSupplierThrowsExceptionShouldRethrowThatException(ILazy<int> lazy)
    {
        Assert.Throws<DivideByZeroException>(() => lazy.Get());
    }

    [Test]
    public void GetMultiThreadShouldReturnCorrectValue()
    {
        var expectedResult = 10;
        var initial = 0;
        MultiThreadLazy<int> lazy = new (() => initial += 10);
        var numberOfThreads = 10;
        var threads = new Thread[numberOfThreads];
        var results = new int[numberOfThreads];
        var manualResetEvent = new ManualResetEvent(false);
        for (int i = 0; i < threads.Length; ++i)
        {
            var localI = i;
            threads[i] = new (() => {
                manualResetEvent.WaitOne();
                results[localI] = lazy.Get();
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

        Assert.That(results.All(x => x == expectedResult), Is.True);
    }
}