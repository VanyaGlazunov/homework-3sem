// Copyright (c) 2024
//
// Use of this source code is governed by an MIT license
// that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

namespace MyNUnit;

public record TestResult
{
    required public string TestName { get; init; }

    public ResultType ResultType { get; init; }

    public long Time { get; init; }

    public string? Ingore { get; init; }

    public Exception? Exception { get; init; }
    public Type? Expected { get; init; }
}
