// Copyright (c) 2024
//
// Use of this source code is governed by an MIT license
// that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

namespace Lazy;

/// <summary>
/// Implements Multi-thread lazy evaluation.
/// </summary>
/// <typeparam name="T">Type of the result of evalution.</typeparam>
/// <param name="supplier">Function to evaluate.</param>
public class MultiThreadLazy<T>(Func<T> supplier) : ILazy<T>
{
    private readonly Semaphore semaphore = new (1, 1);
    private Func<T>? supplier = supplier;
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
                    if (this.supplier is null)
                    {
                        throw new InvalidOperationException("Supplier is null");
                    }

                    this.result = this.supplier();
                }
                catch (Exception e)
                {
                    this.exception = e;
                    throw;
                }
                finally
                {
                    this.supplier = null;
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
