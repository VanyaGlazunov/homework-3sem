// Copyright (c) 2024
//
// Use of this source code is governed by an MIT license
// that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

namespace MyNUnit;

/// <summary>
/// Represents results of testing for some test method.
/// </summary>
public record TestResult
{
    /// <summary>
    /// Gets name of the test.
    /// </summary>
    required public string TestName { get; init; }

    /// <summary>
    /// Gets outcome of the test.
    /// </summary>
    public ResultType ResultType { get; init; }

    /// <summary>
    /// Gets elapsed time of testing.
    /// </summary>
    public long Time { get; init; }

    /// <summary>
    /// Gets reason for ignoring the test.
    /// </summary>
    public string? Ingore { get; init; }

    /// <summary>
    /// Gets exception thrown when running test method.
    /// </summary>
    public Exception? Exception { get; init; }

    /// <summary>
    /// Gets type of expected exception for test method.
    /// </summary>
    public Type? Expected { get; init; }
}
