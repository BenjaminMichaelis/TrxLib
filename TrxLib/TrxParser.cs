using System.Xml.Linq;

namespace TrxLib;

/// <summary>
/// Provides methods to parse TRX (Test Results XML) files into a TestResultSet.
/// </summary>
public class TrxParser
{
    private static readonly XNamespace TrxNs = XNamespace.Get("http://microsoft.com/schemas/VisualStudio/TeamTest/2010");

    /// <summary>
    /// Parses a TRX file and converts it into a TestResultSet containing structured test results.
    /// </summary>
    /// <param name="trxFile">The TRX file to parse.</param>
    /// <returns>A TestResultSet containing the parsed test results. If the file cannot be parsed, an empty TestResultSet is returned.</returns>
    public static TestResultSet Parse(FileInfo trxFile)
    {
        using var stream = trxFile.OpenRead();
        TestRun? testRun = DeserializeTestRun(stream);
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
                "passed"               => TestOutcome.Passed,
                "failed"               => TestOutcome.Failed,
                "notexecuted"          => TestOutcome.NotExecuted,
                "inconclusive"         => TestOutcome.Inconclusive,
                "timeout"              => TestOutcome.Timeout,
                "pending"              => TestOutcome.Pending,
                "error"                => TestOutcome.Error,
                "aborted"              => TestOutcome.Aborted,
                "notrunnable"          => TestOutcome.NotRunnable,
                "disconnected"         => TestOutcome.Disconnected,
                "warning"              => TestOutcome.Warning,
                "completed"            => TestOutcome.Completed,
                "inprogress"           => TestOutcome.InProgress,
                "passedbutrunaborted"  => TestOutcome.PassedButRunAborted,
                // A null/absent outcome attribute means Error in vstest's serialization:
                // TestOutcome.Error is ordinal 0 (the enum default), so XmlPersistence
                // omits the attribute when outcome == Error.
                null                   => TestOutcome.Error,
                _                      => TestOutcome.NotExecuted
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
                    string baseFqtn = testMethodDomain.Name.StartsWith(testMethodDomain.ClassName + ".", StringComparison.Ordinal)
                        ? testMethodDomain.Name
                        : $"{testMethodDomain.ClassName}.{testMethodDomain.Name}";

                    // For parameterized/theory tests, testName carries the suffix (e.g. "(arg1, arg2)").
                    // Extract the short method name and append the suffix from testName if present.
                    var methodShortName = testMethodDomain.Name.Contains('.')
                        ? testMethodDomain.Name.Substring(testMethodDomain.Name.LastIndexOf('.') + 1)
                        : testMethodDomain.Name;
                    var paramSuffix = string.Empty;
                    if (!string.IsNullOrEmpty(result.TestName))
                    {
                        string? candidate = null;

                        if (result.TestName.StartsWith(baseFqtn, StringComparison.Ordinal))
                            candidate = result.TestName.Substring(baseFqtn.Length);
                        else if (result.TestName.StartsWith(methodShortName, StringComparison.Ordinal))
                            candidate = result.TestName.Substring(methodShortName.Length);

                        if (candidate?.StartsWith("(", StringComparison.Ordinal) == true)
                            paramSuffix = candidate;
                    }
                    fullyQualifiedTestName = baseFqtn + paramSuffix;
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

            // Calculate test project directory from codebase if available.
            // Walk up to the 'bin' folder, then take its parent — this handles
            // all standard .NET SDK output layouts:
            //   bin/{config}/{tfm}/                 (depth 3 — classic)
            //   bin/{config}/{tfm}/{rid}/            (depth 4 — self-contained)
            //   bin/{config}/{tfm}/publish/          (depth 4 — publish output)
            //   bin/{config}/{tfm}/{rid}/publish/    (depth 5 — self-contained publish)
            DirectoryInfo? testProjectDirectory = null;
            if (codebaseFile?.Directory != null)
            {
                var dir = codebaseFile.Directory;
                while (dir?.Parent != null && !string.Equals(dir.Name, "bin", StringComparison.OrdinalIgnoreCase))
                {
                    dir = dir.Parent;
                }
                // dir is now the 'bin' folder (or root if no 'bin' found); its parent is the project root.
                testProjectDirectory = string.Equals(dir?.Name, "bin", StringComparison.OrdinalIgnoreCase)
                    ? dir!.Parent
                    : dir;
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
            TestRunId = testRun.Id ?? string.Empty,
            DeploymentRoot = testRun.TestSettings?.Deployment?.RunDeploymentRoot ?? string.Empty,
            TestSettingsName = testRun.TestSettings?.Name ?? string.Empty,
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

    private static TestRun? DeserializeTestRun(Stream stream)
    {
        XDocument doc;
        try
        {
            doc = XDocument.Load(stream);
        }
        catch (System.Xml.XmlException)
        {
            return null;
        }

        var root = doc.Root;
        if (root == null)
            return null;

        var testRun = new TestRun
        {
            Name = (string?)root.Attribute("name"),
            Id = (string?)root.Attribute("id"),
            RunUser = (string?)root.Attribute("runUser"),
        };

        var timesEl = root.Element(TrxNs + "Times");
        if (timesEl != null)
        {
            testRun.Times = new Times
            {
                Creation = (string?)timesEl.Attribute("creation"),
                Queuing = (string?)timesEl.Attribute("queuing"),
                Start = (string?)timesEl.Attribute("start"),
                Finish = (string?)timesEl.Attribute("finish"),
            };
        }

        var testSettingsEl = root.Element(TrxNs + "TestSettings");
        if (testSettingsEl != null)
        {
            testRun.TestSettings = new TestSettings
            {
                Name = (string?)testSettingsEl.Attribute("name"),
                Id = (string?)testSettingsEl.Attribute("id"),
                Deployment = testSettingsEl.Element(TrxNs + "Deployment") is XElement deployEl
                    ? new Deployment { RunDeploymentRoot = (string?)deployEl.Attribute("runDeploymentRoot") }
                    : null,
            };
        }

        var testDefsEl = root.Element(TrxNs + "TestDefinitions");
        if (testDefsEl != null)
        {
            testRun.TestDefinitions = new TestDefinitions
            {
                UnitTests = testDefsEl.Elements(TrxNs + "UnitTest").Select(ut =>
                {
                    var tmEl = ut.Element(TrxNs + "TestMethod");
                    return new UnitTest
                    {
                        Id = (string?)ut.Attribute("id"),
                        Name = (string?)ut.Attribute("name"),
                        Storage = (string?)ut.Attribute("storage"),
                        Execution = ut.Element(TrxNs + "Execution") is XElement execEl
                            ? new Execution { Id = (string?)execEl.Attribute("id") }
                            : null,
                        TestMethod = tmEl != null ? new TestMethod
                        {
                            CodeBase = (string?)tmEl.Attribute("codeBase"),
                            ClassName = (string?)tmEl.Attribute("className"),
                            Name = (string?)tmEl.Attribute("name"),
                            AdapterTypeName = (string?)tmEl.Attribute("adapterTypeName"),
                        } : null,
                    };
                }).ToList(),
            };
        }

        var resultsEl = root.Element(TrxNs + "Results");
        if (resultsEl != null)
        {
            testRun.Results = new Results
            {
                UnitTestResults = resultsEl.Elements(TrxNs + "UnitTestResult").Select(ur =>
                {
                    Output? output = null;
                    if (ur.Element(TrxNs + "Output") is XElement outputEl)
                    {
                        ErrorInfo? errorInfo = null;
                        if (outputEl.Element(TrxNs + "ErrorInfo") is XElement errorInfoEl)
                        {
                            errorInfo = new ErrorInfo
                            {
                                Message = (string?)errorInfoEl.Element(TrxNs + "Message"),
                                StackTrace = (string?)errorInfoEl.Element(TrxNs + "StackTrace"),
                            };
                        }
                        output = new Output
                        {
                            StdOut = (string?)outputEl.Element(TrxNs + "StdOut"),
                            ErrorInfo = errorInfo,
                        };
                    }
                    return new UnitTestResult
                    {
                        TestId = (string?)ur.Attribute("testId"),
                        TestName = (string?)ur.Attribute("testName"),
                        Outcome = (string?)ur.Attribute("outcome"),
                        StartTime = (string?)ur.Attribute("startTime"),
                        EndTime = (string?)ur.Attribute("endTime"),
                        Duration = (string?)ur.Attribute("duration"),
                        ComputerName = (string?)ur.Attribute("computerName"),
                        ExecutionId = (string?)ur.Attribute("executionId"),
                        TestListId = (string?)ur.Attribute("testListId"),
                        TestType = (string?)ur.Attribute("testType"),
                        RelativeResultsDirectory = (string?)ur.Attribute("relativeResultsDirectory"),
                        Output = output,
                    };
                }).ToList(),
            };
        }

        var resultSummaryEl = root.Element(TrxNs + "ResultSummary");
        if (resultSummaryEl != null)
        {
            Counters? counters = null;
            if (resultSummaryEl.Element(TrxNs + "Counters") is XElement countersEl)
            {
                counters = new Counters
                {
                    Total = (int?)countersEl.Attribute("total") ?? 0,
                    Executed = (int?)countersEl.Attribute("executed") ?? 0,
                    Passed = (int?)countersEl.Attribute("passed") ?? 0,
                    Failed = (int?)countersEl.Attribute("failed") ?? 0,
                    Error = (int?)countersEl.Attribute("error") ?? 0,
                    Timeout = (int?)countersEl.Attribute("timeout") ?? 0,
                    Aborted = (int?)countersEl.Attribute("aborted") ?? 0,
                    Inconclusive = (int?)countersEl.Attribute("inconclusive") ?? 0,
                    PassedButRunAborted = (int?)countersEl.Attribute("passedButRunAborted") ?? 0,
                    NotRunnable = (int?)countersEl.Attribute("notRunnable") ?? 0,
                    NotExecuted = (int?)countersEl.Attribute("notExecuted") ?? 0,
                    Disconnected = (int?)countersEl.Attribute("disconnected") ?? 0,
                    Warning = (int?)countersEl.Attribute("warning") ?? 0,
                    Completed = (int?)countersEl.Attribute("completed") ?? 0,
                    InProgress = (int?)countersEl.Attribute("inProgress") ?? 0,
                    Pending = (int?)countersEl.Attribute("pending") ?? 0,
                };
            }
            Output? summaryOutput = null;
            if (resultSummaryEl.Element(TrxNs + "Output") is XElement summaryOutputEl)
            {
                summaryOutput = new Output
                {
                    StdOut = (string?)summaryOutputEl.Element(TrxNs + "StdOut"),
                };
            }
            testRun.ResultSummary = new ResultSummary
            {
                Outcome = (string?)resultSummaryEl.Attribute("outcome"),
                Counters = counters,
                Output = summaryOutput,
            };
        }

        var testListsEl = root.Element(TrxNs + "TestLists");
        if (testListsEl != null)
        {
            testRun.TestLists = new TestLists
            {
                Items = testListsEl.Elements(TrxNs + "TestList").Select(tl => new TestList
                {
                    Name = (string?)tl.Attribute("name"),
                    Id = (string?)tl.Attribute("id"),
                }).ToList(),
            };
        }

        var testEntriesEl = root.Element(TrxNs + "TestEntries");
        if (testEntriesEl != null)
        {
            testRun.TestEntries = new TestEntries
            {
                Items = testEntriesEl.Elements(TrxNs + "TestEntry").Select(te => new TestEntry
                {
                    TestId = (string?)te.Attribute("testId"),
                    ExecutionId = (string?)te.Attribute("executionId"),
                    TestListId = (string?)te.Attribute("testListId"),
                }).ToList(),
            };
        }

        return testRun;
    }
}
