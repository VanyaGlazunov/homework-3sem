// <copyright file="SingleThreadLazy.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Lazy;

/// <summary>
/// Implements single-thread lazy evaluation.
/// </summary>
/// <typeparam name="T">Type of the result of evaluation.</typeparam>
/// <param name="supplier">Funciton to evaluate.</param>
public class SingleThreadLazy<T>(Func<T> supplier) : ILazy<T>
{
    private Func<T>? supplier = supplier;
    private T? result;
    private bool isReady;
    private Exception? exception;

    /// <inheritdoc/>
    public T? Get()
    {
        if (!this.isReady)
        {
            try
            {
                if (this.supplier is null)
                {
                    throw new InvalidOperationException("Supplier is null!");
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
            }
        }

        return this.result;
    }
}
