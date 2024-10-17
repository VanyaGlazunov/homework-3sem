using System.Collections.Concurrent;

namespace MyThreadPool;

public class MyTask<TResult>(MyThreadPool threadPool, Func<TResult> func, CancellationToken cancellationToken) : IMyTask<TResult>
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
        var newFunc = () => func(this.Result);
        var newMyTask = new MyTask<TNewResult>(this.threadPool, newFunc, this.cancellationToken);
        if (!this.cancellationToken.IsCancellationRequested)
        {
            this.continueWithTasks.Enqueue(() => this.threadPool.AddTask(newMyTask));
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
                        this.cancellationToken.ThrowIfCancellationRequested();
                        this.Result = this.func();
                    }
                    catch (Exception e)
                    {
                        this.aggregateException = new ("Task failed", e);
                    }
                    finally
                    {
                        this.IsCompleted = true;
                        for (; !this.continueWithTasks.IsEmpty && !this.cancellationToken.IsCancellationRequested;)
                        {
                            if (this.continueWithTasks.TryDequeue(out Action? continueWithTask))
                            {
                                continueWithTask.Invoke();
                            }
                        }
                    }
                }
            }
        }
    }
}
