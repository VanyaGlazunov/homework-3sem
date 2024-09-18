namespace MatrixMultiplication;

public static class MatrixMultiplier
{
    public static int[,] Multiply(int[,] left, int[,] right, IMultiplicationAlgo multiplicationAlgo) {
        if (left.GetLength(1) != right.GetLength(0))
        {
            throw new ArgumentException("Number of columns in the left matrix must be the same as the number of rows in the right matrix");
        }

        return multiplicationAlgo.Multiply(left, right);
    }
}
