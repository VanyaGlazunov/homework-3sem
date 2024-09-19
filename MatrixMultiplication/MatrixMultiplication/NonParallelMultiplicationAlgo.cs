// Copyright (c) 2024
//
// Use of this source code is governed by an MIT license
// that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

namespace MatrixMultiplication;

/// <summary>
/// Multiplies matricies sequentially.
/// </summary>
public class NonParallelMultiplicationAlgo : IMultiplicationAlgo
{
    public int[,] Multiply(int[,] left, int[,] right)
    {
        var rows = left.GetLength(0);
        var columns = right.GetLength(1);
        var sameDimension = left.GetLength(1);
        var result = new int[rows, columns];
        for (int i = 0; i < rows; ++i)
        {
            for (int j = 0; j < columns; ++j)
            {
                for (int k = 0; k < sameDimension; ++k)
                {
                    result[i, j] += left[i, k] * right[k, j];
                }
            }
        }

        return result;
    }
}
