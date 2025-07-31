// Copyright (c) 2024
//
// Use of this source code is governed by an MIT license
// that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

namespace MyNUnit;

/// <summary>
/// Attribute for methods that are tests.
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class TestAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TestAttribute"/> class.
    /// </summary>
    public TestAttribute()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TestAttribute"/> class with specified reason for ignoring test.
    /// </summary>
    /// <param name="ignore">Reason for ignoring the test.</param>
    public TestAttribute(string ignore)
    {
        this.Ingore = ignore;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TestAttribute"/> class
    /// with specified type of exception expected to be thrown during test.
    /// </summary>
    /// <param name="exceptionType">Type of expected excetption.</param>
    public TestAttribute(Type exceptionType)
    {
        this.Expected = exceptionType;
    }

    /// <summary>
    /// Gets or sets reason for ignoring the test.
    /// </summary>
    public string? Ingore { get; set; }

    /// <summary>
    /// Gets or sets type of exception expected to be thrown during test.
    /// </summary>
    public Type? Expected { get; set; }
}
