using System.Diagnostics;

namespace Test1;

public class Program()
{
    public static void Main(string[] args)
    {
        Console.WriteLine("Benchmarknig on current working directory");
        string directory = Directory.GetCurrentDirectory();

        var nonParallelCheckSum = new NonParallelCheckSum();
        var nonParallelTime = new Stopwatch();
        nonParallelTime.Start();
        var nonParallelHash = nonParallelCheckSum.GetCheckSum(directory);
        nonParallelTime.Stop();

        var parallelCheckSum = new ParallelCheckSum();
        var parallelTime = new Stopwatch();
        parallelTime.Start();
        var parallelHash = parallelCheckSum.GetCheckSum(directory);
        parallelTime.Stop();

        Console.WriteLine($"Parallel Time: {parallelTime.ElapsedMilliseconds}");
        Console.WriteLine($"Non-Parallel Time: {nonParallelTime.ElapsedMilliseconds}");
        Console.WriteLine($"Are Hashes Equal {nonParallelHash == parallelHash}");
    }
}