using MatrixMultiplication;
using BenchmarkDotNet.Running;

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