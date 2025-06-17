using System.Xml.Serialization;

namespace TrxLib;

/// <summary>
/// Provides methods to parse TRX (Test Results XML) files into a TestResultSet.
/// </summary>
public class TrxParser
{
    /// <summary>
    /// Parses a TRX file and converts it into a TestResultSet containing structured test results.
    /// </summary>
    /// <param name="trxFile">The TRX file to parse.</param>
    /// <returns>A TestResultSet containing the parsed test results. If the file cannot be parsed, an empty TestResultSet is returned.</returns>
    public static TestResultSet Parse(FileInfo trxFile)
    {
        using var stream = trxFile.OpenRead();
        var serializer = new XmlSerializer(typeof(TestRun));
        TestRun? testRun = serializer.Deserialize(stream) as TestRun;
        if (testRun == null)
            return new TestResultSet();

        // Build a lookup for UnitTest definitions
        var testDefinitions = testRun.TestDefinitions?.UnitTests?.Where(u => u.Id != null).ToDictionary(u => u.Id!) ?? new();

        var results = new List<TestResult>();
        foreach (var result in testRun.Results?.UnitTestResults ?? Enumerable.Empty<UnitTestResult>())
        {
            // Find the test definition
            UnitTest? unitTest = null;
            if (result.TestId != null)
                testDefinitions.TryGetValue(result.TestId, out unitTest);
            var testMethod = unitTest?.TestMethod;

            var testMethodDomain = testMethod == null ? null : new TestMethod
            {
                CodeBase = testMethod.CodeBase ?? string.Empty,
                ClassName = testMethod.ClassName ?? string.Empty,
                Name = testMethod.Name ?? string.Empty,
                AdapterTypeName = testMethod.AdapterTypeName ?? string.Empty
            };

            var outcome = result.Outcome?.ToLowerInvariant() switch
            {
                "passed" => TestOutcome.Passed,
                "failed" => TestOutcome.Failed,
                "notexecuted" => TestOutcome.NotExecuted,
                "inconclusive" => TestOutcome.Inconclusive,
                "timeout" => TestOutcome.Timeout,
                "pending" => TestOutcome.Pending,
                _ => TestOutcome.NotExecuted
            };

            DateTimeOffset? resultStartTime = null, resultEndTime = null;
            TimeSpan? duration = null;
            if (DateTimeOffset.TryParse(result.StartTime, out var resultSt)) resultStartTime = resultSt;
            if (DateTimeOffset.TryParse(result.EndTime, out var et)) resultEndTime = et;
            if (TimeSpan.TryParse(result.Duration, out var dur)) duration = dur;

            var errorMessage = result.Output?.ErrorInfo?.Message ?? string.Empty;
            var stackTrace = result.Output?.ErrorInfo?.StackTrace ?? string.Empty;

            // Build the fully qualified test name correctly
            string fullyQualifiedTestName = string.Empty;
            if (testMethodDomain != null)
            {
                if (!string.IsNullOrEmpty(testMethodDomain.ClassName) && !string.IsNullOrEmpty(testMethodDomain.Name))
                {
                    // If Name already starts with ClassName, use Name as-is
                    if (testMethodDomain.Name.StartsWith(testMethodDomain.ClassName + "."))
                        fullyQualifiedTestName = testMethodDomain.Name;
                    else
                        fullyQualifiedTestName = $"{testMethodDomain.ClassName}.{testMethodDomain.Name}";
                }
                else
                {
                    fullyQualifiedTestName = result.TestName ?? string.Empty;
                }
            }
            else
            {
                fullyQualifiedTestName = result.TestName ?? string.Empty;
            }

            // Assign codebase from TestMethod if available
            FileInfo? codebaseFile = null;
            if (!string.IsNullOrWhiteSpace(testMethodDomain?.CodeBase))
            {
                codebaseFile = new FileInfo(testMethodDomain.CodeBase);
            }

            // Calculate test project directory from codebase if available
            DirectoryInfo? testProjectDirectory = null;
            if (codebaseFile != null && codebaseFile.Directory != null)
            {
                // Go up 3 levels to get the project directory (bin/Debug/netcoreappX.X/)
                var dir = codebaseFile.Directory;
                for (int i = 0; i < 3 && dir?.Parent != null; i++)
                {
                    dir = dir.Parent;
                }
                testProjectDirectory = dir;
            }

            // Extract StdOut if available
            var stdOut = result.Output?.StdOut ?? string.Empty;

            results.Add(new TestResult(
                fullyQualifiedTestName: fullyQualifiedTestName,
                outcome: outcome,
                duration: duration,
                startTime: resultStartTime,
                endTime: resultEndTime,
                stackTrace: stackTrace,
                errorMessage: errorMessage,
                stdOut: stdOut,
                testProjectDirectory: testProjectDirectory,
                testOutputFile: trxFile,
                codebase: codebaseFile,
                testMethod: testMethodDomain
            )
            {
                ComputerName = result.ComputerName ?? string.Empty
            });
        }

        // Parse Times element
        DateTimeOffset? creationTime = null, queueingTime = null, startTime = null, finishTime = null;
        if (DateTimeOffset.TryParse(testRun.Times?.Creation, out var ct)) creationTime = ct;
        if (DateTimeOffset.TryParse(testRun.Times?.Queuing, out var qt)) queueingTime = qt;
        if (DateTimeOffset.TryParse(testRun.Times?.Start, out var stTime)) startTime = stTime;
        if (DateTimeOffset.TryParse(testRun.Times?.Finish, out var ft)) finishTime = ft;

        var testResultSet = new TestResultSet(results)
        {
            TestRunName = testRun.Name ?? string.Empty,
            TestFilePath = trxFile.FullName,
            OriginalTestRun = testRun
        };

        // Set timing properties from the Times element if available
        if (creationTime.HasValue)
            testResultSet.CreatedTime = creationTime.Value;
        if (queueingTime.HasValue)
            testResultSet.QueuedTime = queueingTime.Value;
        if (startTime.HasValue)
            testResultSet.StartedTime = startTime.Value;
        if (finishTime.HasValue)
            testResultSet.CompletedTime = finishTime.Value;

        return testResultSet;
    }
}
