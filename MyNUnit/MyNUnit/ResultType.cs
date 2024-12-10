// Copyright (c) 2024
//
// Use of this source code is governed by an MIT license
// that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

namespace MyNUnit;

/// <summary>
/// Represents possible outcomes for test methods.
/// </summary>
public enum ResultType
{
    /// <summary>
    /// Test was ignored.
    /// </summary>
    Ignored,

    /// <summary>
    /// Test passed successfully.
    /// </summary>
    Passed,

    /// <summary>
    /// Test failed.
    /// </summary>
    Failed,
}
