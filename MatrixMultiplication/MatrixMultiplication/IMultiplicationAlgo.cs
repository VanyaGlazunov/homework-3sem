// Copyright (c) 2024
//
// Use of this source code is governed by an MIT license
// that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

namespace MatrixMultiplication;

/// <summary>
/// Represents an algorithm for matrix multiplication.
/// </summary>
public interface IMultiplicationAlgo
{
    /// <summary>
    /// Multiplies two matricies.
    /// </summary>
    /// <param name="first">first matrix to multiply.</param>
    /// <param name="second">second matrix to multiply.</param>
    /// <returns>Product of the two given matricies.</returns>
    public int[,] Multiply(int[,] first, int[,] second);
}
