// Copyright (c) 2024
//
// Use of this source code is governed by an MIT license
// that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

namespace MyNUnit;

using System.Collections.Concurrent;
using System.Diagnostics;
using System.Reflection;

public static class TestRunner
{
    public static async Task<Dictionary<string, List<TestResult>>> RunTests(Assembly assembly)
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
            var results = await RunTestsInClass(type);
            Parallel.ForEach(afterClassMethods, method =>
            {
                method.Invoke(null, null);
            });

            report[type.Name] = results;
        }

        return report;
    }

    public static async Task<List<TestResult>> RunTestsInClass(Type testType)
    {
        var methods = testType.GetMethods();
        var tests = methods.Where(m => m.GetCustomAttributes(typeof(TestAttribute), false).Length > 0);
        var before = methods.Where(m => m.GetCustomAttributes(typeof(BeforeAttribute), false).Length > 0);
        var after = methods.Where(m => m.GetCustomAttributes(typeof(AfterAttribute), false).Length > 0);
        var testClass = Activator.CreateInstance(testType);
        var results = new ConcurrentBag<TestResult>();
        Parallel.ForEach(tests, async test =>
        {
            Parallel.ForEach(before, method =>
            {
                method.Invoke(testClass, null);
            });
            var attribute = test.GetCustomAttribute(typeof(TestAttribute)) as TestAttribute;
            if (attribute != null)
            {
                results.Add(await RunTest(test, testClass, attribute));
            }

            Parallel.ForEach(after, method =>
            {
                method.Invoke(testClass, null);
            });
        });

        return[.. results];
    }

    public static async Task<TestResult> RunTest(MethodInfo test, object? testClass, TestAttribute attribute)
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
