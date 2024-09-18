namespace MatrixMultiplication;

public class NonParallelMultiplicationAlgo : IMultiplicationAlgo
{
    public int[,] Multiply(int[,] left, int[,] right)
    {
        var rows = left.GetLength(0);
        var columns = right.GetLength(1);
        var result = new int[rows, columns];
        for (int i = 0; i < rows; ++i)
        {
            for (int j = 0; j < columns; ++j)
            {
                for (int k = 0; k < rows; ++k)
                {
                    result[i, j] += left[i, k] * right[k, j];
                }
            }
        }

        return result;
    }
}
