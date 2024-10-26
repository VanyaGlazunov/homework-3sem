using System.Collections.Concurrent;

namespace MyThreadPool;

public class MyThreadPool
{
    private readonly ConcurrentQueue<Action> tasks = new ();
    private readonly Thread[] threads;
    private readonly CancellationTokenSource cancellationTokenSource = new ();

    public int ThreadCount { get; private set; }

    public MyThreadPool(int numberOfThreads)
    {
        this.ThreadCount = numberOfThreads;
        this.threads = new Thread[this.ThreadCount];
        for (int i = 0; i < this.ThreadCount; ++i)
        {
            this.threads[i] = new (() =>
            {
                var cancellationToken = this.cancellationTokenSource.Token;
                while (!this.cancellationTokenSource.IsCancellationRequested)
                {
                    if (this.tasks.TryDequeue(out Action? task))
                    {
                        task.Invoke();
                    }
                }
            });
        }

        foreach (var thread in this.threads)
        {
            thread.Start();
        }
    }

    public IMyTask<T> Submit<T>(Func<T> task)
    {
        var newMyTask = new MyTask<T>(this, task, this.cancellationTokenSource.Token);
        this.tasks.Enqueue(() => newMyTask.Start());
        return newMyTask;
    }

    public void AddTask<T>(MyTask<T> task)
    {
        this.tasks.Enqueue(() => task.Start());
    }

    public void Shutdown()
    {
        this.cancellationTokenSource.Cancel();
        for (int i = 0; i < this.ThreadCount; ++i)
        {
            this.threads[i].Join();
        }
    }
}
