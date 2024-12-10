// Copyright (c) 2024
//
// Use of this source code is governed by an MIT license
// that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

using System.Reflection;
using MyNUnit;

static void PrintHelp()
{
    Console.WriteLine("""
                    This program takes path to a folder with .NET assemblies and runs all tests in them
                    Usage: MyNUnit [path]
                    """);
}

if (args.Length != 1)
{
    PrintHelp();
    return;
}

string path = args[0];
if (!Path.Exists(path))
{
    Console.WriteLine("Path does not exist!");
    return;
}

Console.WriteLine("Starting test execution, please wait");
Parallel.ForEach(Directory.GetFiles(path), entry =>
{
    if (entry.EndsWith(".dll") || entry.EndsWith(".exe"))
    {
        var assembly = Assembly.LoadFrom(entry);

        var report = TestRunner.RunTests(assembly);
        foreach (var (name, tests) in report.Keys.Zip(report.Values))
        {
            Console.WriteLine($"Tests in {name}");
            foreach (var test in tests)
            {
                switch (test.ResultType)
                {
                    case ResultType.Ignored:
                        Console.WriteLine($"Test: {test.TestName} was ignored. Justification: {test.Ingore}");
                        break;
                    case ResultType.Passed:
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"Test: {test.TestName} passed. Elapsed Time {test.Time} ms");
                        break;
                    case ResultType.Failed:
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"Test {test.TestName} failed. Elapsed Time {test.Time} ms");
                        if (test.Exception != null)
                        {
                            Console.WriteLine(test.Exception);
                        }
                        else if (test.Expected != null)
                        {
                            Console.WriteLine($"Expected {test.Expected} but no exception was thrown");
                        }

                        break;
                }
            }
        }
    }
});
