// Copyright (c) 2024
//
// Use of this source code is governed by an MIT license
// that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

using BenchmarkDotNet.Running;
using MatrixMultiplication;

if (args.Length == 1 && args[0] == "-b")
{
    BenchmarkRunner.Run<MatrixMultiplicationBenchmark>();
}
else if (args.Length == 2)
{
    int[,] left = MatrixMultiplier.InputMatrix(args[0]);
    int[,] right = MatrixMultiplier.InputMatrix(args[0]);
    int[,] result = MatrixMultiplier.Multiply(left, right, new ParallelMultiplicationAlgo());
    for (int i = 0; i < result.GetLength(0); ++i)
    {
        for (int j = 0; j < result.GetLength(1); ++j)
        {
            Console.WriteLine($"{result[i, j]} ");
        }

        Console.WriteLine();
    }
}
else
{
    foreach (var i in args) Console.WriteLine(i);
    Console.WriteLine("Specify: 'filpath to first matrix' 'filepath to second matrix' for multiplication or use '-b' for benchmark");
}
