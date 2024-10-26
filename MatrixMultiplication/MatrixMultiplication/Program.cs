// Copyright (c) 2024
//
// Use of this source code is governed by an MIT license
// that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.
namespace MatrixMultiplication;

using BenchmarkDotNet.Running;

public class Program
{
    public static void Main(string[] args)
    {
        if (args.Length == 1 && args[0] == "-b")
        {
            BenchmarkRunner.Run<MatrixMultiplicationBenchmark>();
        }
        else if (args.Length == 2)
        {
            var left = MatrixMultiplier.InputMatrix(args[0]);
            var right = MatrixMultiplier.InputMatrix(args[0]);
            var result = MatrixMultiplier.Multiply(left, right, new ParallelMultiplicationAlgo());
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
            Console.WriteLine("Specify: 'filpath to first matrix' 'filepath to second matrix' for multiplication or use '-b' for benchmark");
        }
    }
}
