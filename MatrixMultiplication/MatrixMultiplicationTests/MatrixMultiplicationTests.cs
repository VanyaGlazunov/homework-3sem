namespace MatrixMultiplication.Tests;

public class Tests
{
    public static IEnumerable<TestCaseData> MultiplierAlgo()
    {
        yield return new TestCaseData(new NonParallelMultiplicationAlgo());
        yield return new TestCaseData(new ParallelMultiplicationAlgo());
    }

    [TestCaseSource(nameof(MultiplierAlgo))]
    public void Multiply1x1Matricies(IMultiplicationAlgo multiplicationAlgo)
    {
        int[,] left = MatrixMultiplier.InputMatrix("Samples/Test1/left.txt");
        int[,] right = MatrixMultiplier.InputMatrix("Samples/Test1/right.txt");
        int[,] ans = MatrixMultiplier.InputMatrix("Samples/Test1/ans.txt");
        Assert.That(MatrixMultiplier.Multiply(left, right, multiplicationAlgo), Is.EqualTo(ans));
    }

    [TestCaseSource(nameof(MultiplierAlgo))]
    public void MultiplyMatrixByVector(IMultiplicationAlgo multiplicationAlgo)
    {
        int[,] left = MatrixMultiplier.InputMatrix("Samples/Test2/left.txt");
        int[,] right = MatrixMultiplier.InputMatrix("Samples/Test2/right.txt");
        int[,] ans = MatrixMultiplier.InputMatrix("Samples/Test2/ans.txt");
        Assert.That(MatrixMultiplier.Multiply(left, right, multiplicationAlgo), Is.EqualTo(ans));
    }

    [TestCaseSource(nameof(MultiplierAlgo))]
    public void Multiply5x5Matricies(IMultiplicationAlgo multiplicationAlgo)
    {
        int[,] left = MatrixMultiplier.InputMatrix("Samples/Test3/left.txt");
        int[,] right = MatrixMultiplier.InputMatrix("Samples/Test3/right.txt");
        int[,] ans = MatrixMultiplier.InputMatrix("Samples/Test3/ans.txt");
        Assert.That(MatrixMultiplier.Multiply(left, right, multiplicationAlgo), Is.EqualTo(ans));
    }


    [TestCaseSource(nameof(MultiplierAlgo))]
    public void MultiplyMatriciesWithIncorrectSizeThrowsException(IMultiplicationAlgo multiplicationAlgo)
    {
        int[,] left = MatrixMultiplier.InputMatrix("Samples/Test4/left.txt");
        int[,] right = MatrixMultiplier.InputMatrix("Samples/Test4/right.txt");
        Assert.Throws<ArgumentException>(() => MatrixMultiplier.Multiply(left, right, multiplicationAlgo));
    }

}