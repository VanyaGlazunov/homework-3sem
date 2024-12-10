// Copyright (c) 2024
//
// Use of this source code is governed by an MIT license
// that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

namespace MyNUnit;

/// <summary>
/// Attribute for methods indicating they should be invoked after every test in class.
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class AfterAttribute : Attribute
{
}
