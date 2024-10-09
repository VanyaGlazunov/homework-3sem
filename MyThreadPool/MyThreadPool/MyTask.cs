namespace MyThreadPool;

public class MyTask<TResult>(Func<TResult> func, CancellationToken cancellationToken) : IMyTask<TResult>
{
    private readonly Func<TResult> func = func;
    private readonly object lockObject = new ();
    private AggregateException? aggregateException;

    public TResult? Result
    {
        get
        {
            if (aggregateException != null)
            {
                throw aggregateException;
            }

            if (!IsCompleted)
            {
                Start();
            }

            return Result;
        }
        set => Result = value;
    }

    public bool IsCompleted { get; private set; }

    public IMyTask<TNewResult> ContinueWith<TNewResult>(Func<TResult, TNewResult> func)
    {
        throw new NotImplementedException();
    }

    public void Start()
    {
        if (!IsCompleted)
        {
            lock (lockObject)
            {
                if (!IsCompleted)
                {
                    try
                    {
                        Result = func();
                    }
                    catch (Exception e)
                    {
                        aggregateException = new ("Task failed", e);
                    }

                    IsCompleted = true;
                }
            }
        }
    }
}
