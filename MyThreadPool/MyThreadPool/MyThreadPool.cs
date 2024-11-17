// Copyright (c) 2024
//
// Use of this source code is governed by an MIT license
// that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

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
    private readonly ManualResetEvent shutdownEvent = new (true);
    private readonly AutoResetEvent wakeUp = new (false);

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
            this.threads[i] = new (this.Worker);
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
    /// Shuts down treadpool. All tasks that were submitted before shutdown will be completed, the rest will throw exception when getting result.
    /// </summary>
    public void Shutdown()
    {
        if (this.cancellationTokenSource.IsCancellationRequested)
        {
            return;
        }

        this.shutdownEvent.Reset();
        if (!this.cancellationTokenSource.IsCancellationRequested)
        {
            this.cancellationTokenSource.Cancel();
            this.shutdownEvent.Set();
            for (var i = 0; i < this.ThreadCount; ++i)
            {
                this.threads[i].Interrupt();
                this.threads[i].Join();
            }
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
        this.shutdownEvent.WaitOne();
        if (this.cancellationTokenSource.IsCancellationRequested)
        {
            throw new OperationCanceledException("Threadpool was shut down!");
        }

        var newMyTask = new MyTask<T>(this, task, this.cancellationTokenSource.Token, this.shutdownEvent);
        this.tasks.Enqueue(newMyTask.Start);
        this.wakeUp.Set();

        return newMyTask;
    }

    private void Worker()
    {
        while (true)
        {
            try
            {
                this.wakeUp.WaitOne();
            }
            catch (ThreadInterruptedException)
            {
                break;
            }

            if (this.tasks.TryDequeue(out var task))
            {
                this.wakeUp.Set();
                task();
            }
        }
    }

    private class MyTask<TResult>(MyThreadPool threadPool, Func<TResult> func, CancellationToken cancellationToken, ManualResetEvent shutdownEvent) : IMyTask<TResult>
    {
        private readonly Func<TResult> func = func;
        private readonly object lockObject = new ();
        private readonly ManualResetEvent shutdownEvent = shutdownEvent;
        private readonly Queue<Action> continueWithTasks = new ();
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
                    if (this.cancellationToken.IsCancellationRequested)
                    {
                        throw new OperationCanceledException("Thread pool was shut down!");
                    }

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

        public IMyTask<TNewResult> ContinueWith<TNewResult>(Func<TResult?, TNewResult?> func)
        {
            this.shutdownEvent.WaitOne();
            if (this.cancellationToken.IsCancellationRequested)
            {
                throw new OperationCanceledException("Threadpool was shut down!");
            }

            var newFunc = () => func(this.Result);
            if (this.IsCompleted)
            {
                return this.threadPool.Submit(newFunc);
            }

            var newMyTask = new MyTask<TNewResult>(this.threadPool, newFunc, this.cancellationToken, this.shutdownEvent) as IMyTask<TNewResult>;
            this.continueWithTasks.Enqueue(() => this.threadPool.Submit(() => newMyTask.Result));

            return newMyTask;
        }

        public void Start()
        {
            if (this.IsCompleted)
            {
                return;
            }

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
                        while (this.continueWithTasks.Count > 0)
                        {
                            if (this.continueWithTasks.TryDequeue(out var continueWithTask))
                            {
                                try
                                {
                                    continueWithTask();
                                }
                                catch (OperationCanceledException)
                                {
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
