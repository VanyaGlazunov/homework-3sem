// Copyright (c) 2024
//
// Use of this source code is governed by an MIT license
// that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

namespace MyThreadPool;

/// <summary>
/// Represents an asynchronous evaluation.
/// </summary>
/// <typeparam name="TResult">Type of the result of the evaluation.</typeparam>
public interface IMyTask<TResult>
{
    /// <summary>
    /// Gets a value indicating whether evaluation is complete.
    /// </summary>
    public bool IsCompleted { get; }

    /// <summary>
    /// Gets the result of the evaluation.
    /// </summary>
    public TResult? Result { get; }

    /// <summary>
    /// Creates a continuation that shares cancelation token and executes asynchronously when the target Task completes.
    /// </summary>
    /// <typeparam name="TNewResult">Type of the result of the continuation.</typeparam>
    /// <param name="func">Continaution to evaluate.</param>
    /// <returns>Task containing continuation.</returns>
    public IMyTask<TNewResult> ContinueWith<TNewResult>(Func<TResult?, TNewResult?> func);
}
