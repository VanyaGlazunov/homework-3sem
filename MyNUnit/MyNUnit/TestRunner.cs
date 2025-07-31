// Copyright (c) 2024
//
// Use of this source code is governed by an MIT license
// that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

namespace MyNUnit;

using System.Collections.Concurrent;
using System.Diagnostics;
using System.Reflection;

/// <summary>
/// Provides methods to invoke test methods in .NET assemblies.
/// </summary>
public static class TestRunner
{
    /// <summary>
    /// Runs all tests in specified assembly.
    /// </summary>
    /// <param name="assembly">Assembly to test.</param>
    /// <returns>Dictionary in which keys are test class names and values are results of tests methods.</returns>
    public static Dictionary<string, List<TestResult>> RunTests(Assembly assembly)
    {
        var types = assembly.GetTypes();
        var beforeClassMethods = types.SelectMany(t => t.GetMethods()).
        Where(m => m.GetCustomAttributes(typeof(BeforeClassAttribute), false).Length > 0);
        var afterClassMethods = types.SelectMany(t => t.GetMethods()).
        Where(m => m.GetCustomAttributes(typeof(AfterClassAttribute), false).Length > 0);

        var report = new Dictionary<string, List<TestResult>>();
        foreach (var type in types)
        {
            Parallel.ForEach(beforeClassMethods, method =>
            {
                method.Invoke(null, null);
            });
            var results = RunTestsInClass(type);
            Parallel.ForEach(afterClassMethods, method =>
            {
                method.Invoke(null, null);
            });

            report[type.Name] = results;
        }

        return report;
    }

    /// <summary>
    /// Runs tests in specified class.
    /// </summary>
    /// <param name="testType">Class to run tests in.</param>
    /// <returns>Results of all tests in class.</returns>
    public static List<TestResult> RunTestsInClass(Type testType)
    {
        var methods = testType.GetMethods();
        var tests = methods.Where(m => m.GetCustomAttributes(typeof(TestAttribute), false).Length > 0);
        var before = methods.Where(m => m.GetCustomAttributes(typeof(BeforeAttribute), false).Length > 0);
        var after = methods.Where(m => m.GetCustomAttributes(typeof(AfterAttribute), false).Length > 0);
        var results = new ConcurrentBag<TestResult>();
        Parallel.ForEach(tests, test =>
        {
            var testClass = Activator.CreateInstance(testType);
            foreach (var method in before)
            {
                method.Invoke(testClass, null);
            }

            var attribute = test.GetCustomAttribute(typeof(TestAttribute)) as TestAttribute;
            if (attribute != null)
            {
                results.Add(RunTest(test, testClass, attribute));
            }

            foreach (var method in after)
            {
                method.Invoke(testClass, null);
            }
        });

        return [.. results];
    }

/// <summary>
/// Runs specified test method in specified class.
/// </summary>
/// <param name="test">Test method to run.</param>
/// <param name="testClass">Class containing test method.</param>
/// <param name="attribute">Test attribute of specified test method.</param>
/// <returns>Results of the test.</returns>
    public static TestResult RunTest(MethodInfo test, object? testClass, TestAttribute attribute)
    {
        if (attribute.Ingore != null)
        {
            return new TestResult { TestName = test.Name, Ingore = attribute.Ingore };
        }

        var watch = new Stopwatch();
        if (attribute.Expected != null)
        {
            try
            {
                watch.Start();
                test.Invoke(testClass, null);
                watch.Stop();
                return new TestResult
                {
                    TestName = test.Name,
                    Time = watch.ElapsedMilliseconds,
                    ResultType = ResultType.Failed,
                    Expected = attribute.Expected,
                };
            }
            catch (TargetInvocationException e) when (e.InnerException != null && e.InnerException.GetType() == attribute.Expected)
            {
                watch.Stop();
                return new TestResult
                {
                    TestName = test.Name,
                    Time = watch.ElapsedMilliseconds,
                    ResultType = ResultType.Passed,
                };
            }
            catch (TargetInvocationException e)
            {
                watch.Stop();
                return new TestResult
                {
                    TestName = test.Name,
                    Time = watch.ElapsedMilliseconds,
                    ResultType = ResultType.Failed,
                    Exception = e.InnerException,
                    Expected = attribute.Expected,
                };
            }
        }

        try
        {
            watch.Start();
            test.Invoke(testClass, null);
            watch.Stop();
            return new TestResult
            {
                TestName = test.Name,
                Time = watch.ElapsedMilliseconds,
                ResultType = ResultType.Passed,
            };
        }
        catch (TargetInvocationException e)
        {
            watch.Stop();
            return new TestResult
            {
                TestName = test.Name,
                Time = watch.ElapsedMilliseconds,
                ResultType = ResultType.Failed,
                Exception = e.InnerException,
            };
        }
    }
}
