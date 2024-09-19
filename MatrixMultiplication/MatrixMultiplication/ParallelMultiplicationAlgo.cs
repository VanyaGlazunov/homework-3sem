namespace MatrixMultiplication;

public class ParallelMultiplicationAlgo : IMultiplicationAlgo
{
    public int[,] Multiply(int[,] left, int[,] right)
    {
        var rows = left.GetLength(0);
        var columns = right.GetLength(1);
        var sameDimension = left.GetLength(1);
        var result = new int[rows, columns];
        var threads = new Thread[Environment.ProcessorCount];
        var chunkSize = (rows / threads.Length) + 1;
        for (var i = 0; i < threads.Length; ++i)
        {
            var localI = i;
            threads[i] = new Thread(() =>
            {
                for (var j = localI * chunkSize; j < (localI + 1) * chunkSize && j < rows; j++)
                {
                    for (var k = 0; k < columns; ++k)
                    {
                        for (var l = 0; l < sameDimension; ++l)
                        {
                            result[j, k] += left[j, l] * right[l, k];
                        }
                    }
                }
            });
        }

        foreach (var thread in threads)
        {
            thread.Start();
        }

        foreach (var thread in threads)
        {
            thread.Join();
        }

        return result;
    }
}
