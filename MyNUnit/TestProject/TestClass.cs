namespace TestProject;

using MyNUnit;

public class Tests
{
    static public int beforeClassCount;
    static public int afterClassCount;
    static public int beforeCount;
    static public int afterCount;
    static public Random random = new Random();

    [BeforeClass]
    public static void BeforeClass()
    {
        Interlocked.Add(ref beforeClassCount, 1);
    }

    [AfterClass]
    public static void AfterClass()
    {
        Interlocked.Add(ref afterClassCount, 1);
    }

    [Before]
    public void Before()
    {
        Interlocked.Add(ref beforeCount, 1);
    }

    [Test]
    public void Test()
    {
        var rand = random.Next();
        if (rand % 2 == 0)
        {
            return;
        }
        if (rand % 2 == 1)
        {
            return;
        }

        throw new Exception();
    }

    [Test]
    public static int StaticTest()
    {
        if (beforeClassCount == 1)
        {
            return 1;
        }
        else
        {
            throw new Exception();
        }
    }

    [Test(typeof(DivideByZeroException))]
    public void ThrowCorrectException()
    {
        throw new DivideByZeroException();
    }

    [Test(typeof(DivideByZeroException))]
    public void ThrowIncorrectException()
    {
        throw new NotImplementedException();
    }

    [Test(typeof(DivideByZeroException))]
    public void ThrowNoExceptions()
    {

    }

    [Test("Ignore")]
    public void IgnoreTest()
    {

    }

    [After]
    public void After()
    {
        Interlocked.Add(ref afterCount, 1);
    }
}
