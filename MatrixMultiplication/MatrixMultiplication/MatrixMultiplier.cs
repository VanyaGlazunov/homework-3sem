// Copyright (c) 2024
//
// Use of this source code is governed by an MIT license
// that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

namespace MatrixMultiplication;

/// <summary>
/// Implements methods for matrix multiplication.
/// </summary>
public static class MatrixMultiplier
{
    /// <summary>
    /// Reads matrix from file into two-dimensional array.
    /// </summary>
    /// <param name="filepath">filepath to matrix.</param>
    /// <returns>two-dimensional array representing the given matrix.</returns>
    public static int[,] InputMatrix(string filepath)
    {
        using var streamReader = new StreamReader(filepath);
        var input = streamReader.ReadLine() ?? throw new ArgumentException("Wrong input file structure!");
        var dimensions = input.Trim().Split();
        var rows = int.Parse(dimensions[0]);
        var columns = int.Parse(dimensions[1]);
        var matrix = new int[rows, columns];
        for (var i = 0; i < rows; ++i)
        {
            input = streamReader.ReadLine() ?? throw new ArgumentException("Wrong input file structure!");
            var row = input.Trim().Split().Select(x => int.Parse(x)).ToArray();
            for (var j = 0; j < columns; ++j)
            {
                matrix[i, j] = row[j];
            }
        }

        return matrix;
    }

    /// <summary>
    /// Multiplies two matricies.
    /// </summary>
    /// <param name="first">first matrix to multiply.</param>
    /// <param name="second">second matrix to multiply.</param>
    /// <param name="multiplicationAlgo">Class representing multiplication algorithm.</param>
    /// <returns>Product of the two given matricies.</returns>
    /// <exception cref="ArgumentException">Thrown when two given matricies have incorrect sizes.</exception>
    public static int[,] Multiply(int[,] first, int[,] second, IMultiplicationAlgo multiplicationAlgo)
    {
        if (first.GetLength(1) != second.GetLength(0))
        {
            throw new ArgumentException("Number of columns in the first matrix must be the same as the number of rows in the second matrix");
        }

        return multiplicationAlgo.Multiply(first, second);
    }
}
