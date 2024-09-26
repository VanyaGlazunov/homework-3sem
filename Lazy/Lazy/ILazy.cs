// <copyright file="ILazy.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

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