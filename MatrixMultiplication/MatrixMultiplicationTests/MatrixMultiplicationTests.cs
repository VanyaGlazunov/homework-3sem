namespace MatrixMultiplication.Tests;

public class MatrixMultiplicationTests
{
    public static IEnumerable<TestCaseData> MultiplierAlgo()
    {
        yield return new TestCaseData(new NonParallelMultiplicationAlgo());
        yield return new TestCaseData(new ParallelMultiplicationAlgo());
    }

    [TestCaseSource(nameof(MultiplierAlgo))]
    public void Multiply1x1Matricies(IMultiplicationAlgo multiplicationAlgo)
    {
        var left = MatrixMultiplier.InputMatrix("Samples/Test1/left.txt");
        var right = MatrixMultiplier.InputMatrix("Samples/Test1/right.txt");
        var expected = MatrixMultiplier.InputMatrix("Samples/Test1/ans.txt");
        var actual = MatrixMultiplier.Multiply(left, right, multiplicationAlgo);
        Assert.That(actual, Is.EqualTo(expected));
    }

    [TestCaseSource(nameof(MultiplierAlgo))]
    public void MultiplyMatrixByVector(IMultiplicationAlgo multiplicationAlgo)
    {
        var left = MatrixMultiplier.InputMatrix("Samples/Test2/left.txt");
        var right = MatrixMultiplier.InputMatrix("Samples/Test2/right.txt");
        var expected = MatrixMultiplier.InputMatrix("Samples/Test2/ans.txt");
        var actual = MatrixMultiplier.Multiply(left, right, multiplicationAlgo);
        Assert.That(actual, Is.EqualTo(expected));
    }

    [TestCaseSource(nameof(MultiplierAlgo))]
    public void Multiply5x5Matricies(IMultiplicationAlgo multiplicationAlgo)
    {
        var left = MatrixMultiplier.InputMatrix("Samples/Test3/left.txt");
        var right = MatrixMultiplier.InputMatrix("Samples/Test3/right.txt");
        var expected = MatrixMultiplier.InputMatrix("Samples/Test3/ans.txt");
        var actual = MatrixMultiplier.Multiply(left, right, multiplicationAlgo);
        Assert.That(actual, Is.EqualTo(expected));
    }


    [TestCaseSource(nameof(MultiplierAlgo))]
    public void MultiplyMatriciesWithIncorrectSizeThrowsException(IMultiplicationAlgo multiplicationAlgo)
    {
        var left = MatrixMultiplier.InputMatrix("Samples/Test4/left.txt");
        var right = MatrixMultiplier.InputMatrix("Samples/Test4/right.txt");
        Assert.Throws<ArgumentException>(() => MatrixMultiplier.Multiply(left, right, multiplicationAlgo));
    }
}