﻿// <copyright file="MyThreadPool.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace MyThreadPool;

using System.Collections.Concurrent;

/// <summary>
/// Provides a pool of threads that can be used to execute tasks asynchronously.
/// </summary>
public class MyThreadPool
{
    private readonly ConcurrentQueue<Action> tasks = new ();
    private readonly Thread[] threads;
    private readonly CancellationTokenSource cancellationTokenSource = new ();

    /// <summary>
    /// Initializes a new instance of the <see cref="MyThreadPool"/> class with specified number of running threads.
    /// </summary>
    /// <param name="numberOfThreads">Number of threads to be created inside MyThreadPool class.</param>
    public MyThreadPool(int numberOfThreads)
    {
        if (numberOfThreads < 1)
        {
            throw new ArgumentException("number of threads must be postive number!", nameof(numberOfThreads));
        }

        this.ThreadCount = numberOfThreads;
        this.threads = new Thread[this.ThreadCount];
        for (var i = 0; i < this.ThreadCount; ++i)
        {
            this.threads[i] = new (() =>
            {
                var cancellationToken = this.cancellationTokenSource.Token;
                while (!this.cancellationTokenSource.IsCancellationRequested)
                {
                    if (this.tasks.TryDequeue(out Action? task))
                    {
                        task();
                    }
                }
            });
        }

        foreach (var thread in this.threads)
        {
            thread.Start();
        }
    }

    /// <summary>
    /// Gets the number of threads that are currently running.
    /// </summary>
    public int ThreadCount { get; private set; }

    /// <summary>
    /// Shutdowns treadpool. All tasks that were started will be completed, the rest will throw exception when getting result.
    /// </summary>
    public void Shutdown()
    {
        this.cancellationTokenSource.Cancel();
        for (var i = 0; i < this.ThreadCount; ++i)
        {
            this.threads[i].Join();
        }
    }

    /// <summary>
    /// Queues the specified function to evaluate on the <see cref="MyThreadPool"/> and returns a <see cref="IMyTask<typeparamref name="T"/>"/> for that function.
    /// </summary>
    /// <typeparam name="T">Type of the result of the evaluation.</typeparam>
    /// <param name="task">Functions to evaluate on <see cref="MyThreadPool"/>.</param>
    /// <returns><see cref="IMyTask"/> for the given function.</returns>
    public IMyTask<T> Submit<T>(Func<T> task)
    {
        if (this.cancellationTokenSource.IsCancellationRequested)
        {
            throw new OperationCanceledException("Threadpool was shut down!");
        }

        var newMyTask = new MyTask<T>(this, task, this.cancellationTokenSource.Token);
        this.tasks.Enqueue(() => newMyTask.Start());
        return newMyTask;
    }

    private void QueryContinuation(Action continuation)
    {
        if (this.cancellationTokenSource.IsCancellationRequested)
        {
            throw new OperationCanceledException("Threadpool was shut down!");
        }

        this.tasks.Enqueue(continuation);
    }

    private class MyTask<TResult>(MyThreadPool threadPool, Func<TResult> func, CancellationToken cancellationToken) : IMyTask<TResult>
    {
        private readonly Func<TResult> func = func;
        private readonly object lockObject = new ();
        private readonly ConcurrentQueue<Action> continueWithTasks = new ();
        private readonly MyThreadPool threadPool = threadPool;
        private readonly CancellationToken cancellationToken = cancellationToken;
        private AggregateException? aggregateException;
        private TResult? result;

        public TResult? Result
        {
            get
            {
                if (!this.IsCompleted)
                {
                    this.cancellationToken.ThrowIfCancellationRequested();
                    this.Start();
                }

                if (this.aggregateException != null)
                {
                    throw this.aggregateException;
                }

                return this.result;
            }
            private set => this.result = value;
        }

        public bool IsCompleted { get; private set; }

        public IMyTask<TNewResult> ContinueWith<TNewResult>(Func<TResult?, TNewResult> func)
        {
            this.cancellationToken.ThrowIfCancellationRequested();
            var newFunc = () => func(this.Result);
            var newMyTask = new MyTask<TNewResult>(this.threadPool, newFunc, this.cancellationToken);
            if (this.IsCompleted)
            {
                this.threadPool.QueryContinuation(newMyTask.Start);
            }
            else
            {
                this.continueWithTasks.Enqueue(() => this.threadPool.QueryContinuation(newMyTask.Start));
            }

            return newMyTask;
        }

        public void Start()
        {
            if (!this.IsCompleted)
            {
                lock (this.lockObject)
                {
                    if (!this.IsCompleted)
                    {
                        try
                        {
                            this.Result = this.func();
                        }
                        catch (Exception e)
                        {
                            this.aggregateException = new ("Task failed", e);
                        }
                        finally
                        {
                            this.IsCompleted = true;
                            while (!this.continueWithTasks.IsEmpty && !this.cancellationToken.IsCancellationRequested)
                            {
                                if (this.continueWithTasks.TryDequeue(out Action? continueWithTask))
                                {
                                    continueWithTask();
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}