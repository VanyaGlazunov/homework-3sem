namespace Reflector.Tests;

public class Tests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void PrintStructureEmptyClassPrintsExpectedResult()
    {
        var a = typeof(A);
        Reflector.PrintStructure(a, "./");
        var path = "./A.cs";
        var actual = File.ReadAllText(path);
        var expected = File.ReadAllText("TestClasses/EmptyClass.cs");
        Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public void PrintStructureStaticClassPrintsExpectedResult()
    {
        var cls = typeof(B);
        Reflector.PrintStructure(cls, "./");
        var path = "./B.cs";
        var expected = "public static class B {\n    public static Int32 i;\n    public static String s;\n    public static System.Void M1 (){}\n    public static System.Int32 M2 (){}\n}\n";
        var actual = File.ReadAllText(path);
        Assert.That(actual, Is.EqualTo(expected));
    }
}