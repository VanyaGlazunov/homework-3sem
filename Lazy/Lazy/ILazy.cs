// Copyright (c) 2024
//
// Use of this source code is governed by an MIT license
// that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

namespace Lazy;

/// <summary>
/// Provides support for lazy evalution.
/// </summary>
/// <typeparam name="T">Type of the result of evalution.</typeparam>
public interface ILazy<T>
{
    /// <summary>
    /// Gets evaluated result. Evaluation happens just once.
    /// </summary>
    /// <returns>Result of the lazy evaluation.</returns>
    public T? Get();
}
