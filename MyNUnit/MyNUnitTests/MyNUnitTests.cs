namespace MyNUnit.Tests;

using System.Reflection;

public class Tests
{
    private Assembly? assembly;
    private Dictionary<string, List<TestResult>>? testResults;

    [OneTimeSetUp]
    public async Task Setup()
    {
        assembly = Assembly.LoadFrom("../../../../TestProject/bin/Debug/net8.0/TestProject.dll");
        testResults = await TestRunner.RunTests(assembly);
    }

    [NUnit.Framework.Test]
    public void AfterClassBeforeClassShouldBeExecutedOnceClassTest()
    {
        var expected = 1;
        var type = assembly!.GetType("TestProject.Tests");
        Assert.Multiple(() =>
        {
            var actual = type!.GetField("afterClassCount")!.GetValue(null) ?? -1;
            Assert.That((int)actual, Is.EqualTo(expected));
            actual = type!.GetField("beforeClassCount")!.GetValue(null) ?? -1;
            Assert.That((int)actual, Is.EqualTo(expected));
        });
    }

    [NUnit.Framework.Test]
    public void AfterBeforeShouldBeExecutedForEveryTest()
    {
        var expected = 6;
        var type = assembly!.GetType("TestProject.Tests");
        Assert.Multiple(() =>
        {
            var actual = type!.GetField("afterCount")!.GetValue(null) ?? -1;
            Assert.That((int)actual, Is.EqualTo(expected));
            actual = type!.GetField("beforeCount")!.GetValue(null) ?? -1;
            Assert.That((int)actual, Is.EqualTo(expected));
        });
    }

    [Test]
    public void CorrectTestsShouldPass()
    {
        var testNames = new string[4] { "ThrowIncorrectException", "Test", "StaticTest", "TestWithReturnType" };
        var actuals = testResults!["Tests"].Where(t => testNames.Contains(t.TestName));
        Assert.Multiple(() =>
        {
            foreach (var actual in actuals)
            {
                Assert.That(actual.ResultType, Is.EqualTo(ResultType.Passed));
            }
        });
    }

    [NUnit.Framework.Test]
    public void TestWithIgnoreShouldPerformExpectedResults()
    {
        var testName = "IgnoreTest";
        var expected = "Ignore";
        var actual = testResults!["Tests"].First(t => t.TestName == testName);
        Assert.Multiple(() =>
        {
            Assert.That(actual.ResultType, Is.EqualTo(ResultType.Ignored));
            Assert.That(actual.Ingore, Is.EqualTo(expected));
        });
    }


    [NUnit.Framework.Test]
    public void TestWithIncorrectExceptionShouldFail()
    {
        var testName = "ThrowIncorrectException";
        var actual = testResults!["Tests"].First(t => t.TestName == testName);
        Assert.That(actual.ResultType, Is.EqualTo(ResultType.Failed));
    }

    [NUnit.Framework.Test]
    public void TestWithExpectedNoExceptionThrownShouldFail()
    {
        var testName = "ThrowNoExceptions";
        var actual = testResults!["Tests"].First(t => t.TestName == testName);
        Assert.Multiple(() =>
        {
            Assert.That(actual.ResultType, Is.EqualTo(ResultType.Failed));
            Assert.That(actual.Expected, Is.EqualTo(typeof(DivideByZeroException)));
        });
    }
}