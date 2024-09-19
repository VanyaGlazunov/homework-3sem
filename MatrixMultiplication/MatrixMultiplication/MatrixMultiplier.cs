namespace MatrixMultiplication;

public static class MatrixMultiplier
{
    public static int[,] InputMatrix(string filepath) {
        var streamReader = new StreamReader(filepath);
        var dimensions = streamReader.ReadLine()!.Trim().Split();
        var rows = int.Parse(dimensions[0]);
        var columns = int.Parse(dimensions[1]);
        int[,] matrix = new int[rows, columns];
        for (int i = 0; i < rows; ++i)
        {
            var row = streamReader.ReadLine()!.Trim().Split().Select(x => int.Parse(x)).ToArray();
            for (int j = 0; j < columns; ++j)
            {
                matrix[i, j] = row[j];
            }
        }

        return matrix;
    }

    public static int[,] Multiply(int[,] left, int[,] right, IMultiplicationAlgo multiplicationAlgo) {
        if (left.GetLength(1) != right.GetLength(0))
        {
            throw new ArgumentException("Number of columns in the left matrix must be the same as the number of rows in the right matrix");
        }

        return multiplicationAlgo.Multiply(left, right);
    }
}
