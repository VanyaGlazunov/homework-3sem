using System.Collections.Concurrent;

namespace MyThreadPool;

public class MyThreadPool
{
    private readonly ConcurrentQueue<Action> tasks = new ();
    private readonly int numberOfThreads;
    private readonly Thread[] threads;
    private readonly CancellationTokenSource cancellationTokenSource = new ();

    public MyThreadPool(int numberOfThreads)
    {
        this.numberOfThreads = numberOfThreads;
        this.threads = new Thread[this.numberOfThreads];
        for (int i = 0; i < this.numberOfThreads; ++i)
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
        for (int i = 0; i < this.numberOfThreads; ++i)
        {
            this.threads[i].Join();
        }
    }
}
