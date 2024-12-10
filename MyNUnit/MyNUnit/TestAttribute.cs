// Copyright (c) 2024
//
// Use of this source code is governed by an MIT license
// that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

namespace MyNUnit;

[AttributeUsage(AttributeTargets.Method)]
public class TestAttribute : Attribute
{
    public string? Ingore;
    public Type? Expected;

    public TestAttribute()
    {
    }

    public TestAttribute(string ignore, Type exceptionType)
    {
        this.Ingore = ignore;
        this.Expected = exceptionType;
    }

    public TestAttribute(string ignore)
    {
        this.Ingore = ignore;
    }

    public TestAttribute(Type exceptionType)
    {
        this.Expected = exceptionType;
    }
}
