using BenchmarkDotNet.Attributes;

namespace MatrixMultiplication;

public class MatrixMultiplicationBenchmark
{
    private int[,]? left;
    private int[,]? right;
    private NonParallelMultiplicationAlgo nonParallelAlgo = new ();
    private ParallelMultiplicationAlgo parallelAlgo = new ();

    private Dictionary<int, string> samples = new ()
    {
        { 100, "Samples/Benchmark1/"},
        { 200, "Samples/Benchmark2/"},
        { 400, "Samples/Benchmark3/"},
        { 800, "Samples/Benchmark4/"},
        { 1600, "Samples/Benchmark5/"},
    };

    [Params(100, 200)]
    public int N;

    [GlobalSetup]
    public void Setup()
    {
        var filepath = samples[N];
        left = MatrixMultiplier.InputMatrix(filepath + "left.txt");
        right = MatrixMultiplier.InputMatrix(filepath + "right.txt");
    }

    [Benchmark(Description = "Parallel")]
    public int[,] BenchmarkParallelMultliplication() => MatrixMultiplier.Multiply(left, right, parallelAlgo);

    [Benchmark(Description = "Non-Parallel")]
    public int[,] BenchmarkNonParallelMultiplication() => MatrixMultiplier.Multiply(left, right, nonParallelAlgo);
}
