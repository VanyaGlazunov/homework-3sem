// Copyright (c) 2024
//
// Use of this source code is governed by an MIT license
// that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

namespace MatrixMultiplication;

using BenchmarkDotNet.Attributes;

/// <summary>
/// Class for benchmarking matrix multiplication via BenchmarkDotNet package.
/// </summary>
public class MatrixMultiplicationBenchmark
{
    private int[,] left = new int[1, 1];
    private int[,] right = new int[1, 1];
    private NonParallelMultiplicationAlgo nonParallelAlgo = new ();
    private ParallelMultiplicationAlgo parallelAlgo = new ();

    private Dictionary<int, string> samples = new ()
    {
        { 100, "Samples/Benchmark1/" },
        { 200, "Samples/Benchmark2/" },
        { 400, "Samples/Benchmark3/" },
        { 800, "Samples/Benchmark4/" },
        { 1600, "Samples/Benchmark5/" },
    };

    [Params(100, 200, 400, 800, 1600)]
    public int N { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var filepath = this.samples[this.N];
        this.left = MatrixMultiplier.InputMatrix(filepath + "left.txt");
        this.right = MatrixMultiplier.InputMatrix(filepath + "right.txt");
    }

    [Benchmark(Description = "Parallel")]
    public int[,] BenchmarkParallelMultliplication() => MatrixMultiplier.Multiply(this.left, this.right, this.parallelAlgo);

    [Benchmark(Description = "Non-Parallel")]
    public int[,] BenchmarkNonParallelMultiplication() => MatrixMultiplier.Multiply(this.left, this.right, this.nonParallelAlgo);
}
