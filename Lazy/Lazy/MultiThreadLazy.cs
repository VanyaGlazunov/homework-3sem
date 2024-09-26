// <copyright file="MultiThreadLazy.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Lazy;

/// <summary>
/// Represents Multi-thread lazy evaluation.
/// </summary>
/// <typeparam name="T">Type of the result of evalution.</typeparam>
/// <param name="supplier">Function to evaluate.</param>
public class MultiThreadLazy<T>(Func<T> supplier) : ILazy<T>
{
    private readonly Func<T> supplier = supplier ?? throw new ArgumentException("Supplier cannot be null");
    private readonly Semaphore semaphore = new (1, 1);
    private T? result;
    private Exception? exception;

    private bool isReady;

    /// <inheritdoc/>
    public T? Get()
    {
        if (this.exception != null)
        {
            throw this.exception;
        }

        if (!this.isReady)
        {
            this.semaphore.WaitOne();
            if (!this.isReady)
            {
                try
                {
                    this.result = this.supplier();
                }
                catch (Exception e)
                {
                    this.exception = e;
                    throw;
                }
                finally
                {
                    this.isReady = true;
                    this.semaphore.Release();
                }
            }
            else
            {
                this.semaphore.Release();
            }
        }

        return this.result;
    }
}
