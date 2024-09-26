// <copyright file="SingleThreadLazy.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Lazy;

/// <summary>
/// Represents single-thread lazy evaluation.
/// </summary>
/// <typeparam name="T">Type of the result of evaluation.</typeparam>
/// <param name="supplier">Funciton to evaluate.</param>
public class SingleThreadLazy<T>(Func<T> supplier) : ILazy<T>
{
    private readonly Func<T> supplier = supplier ?? throw new ArgumentException("Supplier cannot be null");
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
            }
        }

        return this.result;
    }
}
