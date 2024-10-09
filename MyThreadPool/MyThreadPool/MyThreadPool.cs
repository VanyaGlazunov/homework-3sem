using System.Collections.Concurrent;

namespace MyThreadPool;

public class MyThreadPool
{
    private int numberOfThreads;
    private ConcurrentQueue<Action> tasks = new ();
    private Thread[] threads;
    private CancellationTokenSource cancellationTokenSource = new ();

    public MyThreadPool(int numberOfThreads)
    {
        this.numberOfThreads = numberOfThreads;
        this.threads = new Thread[this.numberOfThreads];
        for (int i = 0; i < this.numberOfThreads; ++i)
        {
            this.threads[i] = new (() => {
                var cancellationToken = this.cancellationTokenSource.Token;
                while (!cancellationToken.IsCancellationRequested && this.tasks.TryDequeue(out Action? task))
                {
                    task.Invoke();
                }
            });
        }
    }

    IMyTask<T> Submit<T>(Func<T> task) {
        throw new NotImplementedException();
    }
}
