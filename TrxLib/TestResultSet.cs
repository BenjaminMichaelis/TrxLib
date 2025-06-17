using System.Collections;

namespace TrxLib;

/// <summary>
/// Represents a set of test results, grouped by outcome.
/// </summary>
public class TestResultSet : IReadOnlyList<TestResult>
{
    private readonly List<TestResult> _all = new();
    private readonly List<TestResult> _passed = new();
    private readonly List<TestResult> _failed = new();
    private readonly List<TestResult> _notExecuted = new();
    private readonly List<TestResult> _inconclusive = new();
    private readonly List<TestResult> _timeout = new();
    private readonly List<TestResult> _pending = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="TestResultSet"/> class.
    /// </summary>
    /// <param name="testResults">Optional collection of test results to initialize the set with. The results are ordered by test name.</param>
    public TestResultSet(IEnumerable<TestResult>? testResults = null)
    {
        foreach (var testResult in (testResults ?? Enumerable.Empty<TestResult>()).OrderBy(r => r.FullyQualifiedTestName))
        {
            Add(testResult);
        }
    }

    /// <summary>
    /// Gets or sets the creation time of the test run.
    /// </summary>
    public DateTimeOffset CreatedTime { get; set; } = DateTimeOffset.UtcNow;
    
    /// <summary>
    /// Gets or sets the time when the test run was queued for execution.
    /// </summary>
    public DateTimeOffset QueuedTime { get; set; } = DateTimeOffset.UtcNow;
    
    /// <summary>
    /// Gets or sets the time when the test run started execution.
    /// </summary>
    public DateTimeOffset StartedTime { get; set; } = DateTimeOffset.UtcNow;
    
    /// <summary>
    /// Gets or sets the time when the test run completed execution.
    /// </summary>
    public DateTimeOffset CompletedTime { get; set; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// Gets or sets the unique identifier for this test result set.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Gets or sets the name of this test result set.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets the collection of tests that passed.
    /// </summary>
    public IReadOnlyCollection<TestResult> Passed => _passed;
    
    /// <summary>
    /// Gets the collection of tests that failed.
    /// </summary>
    public IReadOnlyCollection<TestResult> Failed => _failed;
    
    /// <summary>
    /// Gets the collection of tests that were not executed.
    /// </summary>
    public IReadOnlyCollection<TestResult> NotExecuted => _notExecuted;
    
    /// <summary>
    /// Gets the collection of tests with inconclusive results.
    /// </summary>
    public IReadOnlyCollection<TestResult> Inconclusive => _inconclusive;
    
    /// <summary>
    /// Gets the collection of tests that timed out.
    /// </summary>
    public IReadOnlyCollection<TestResult> Timeout => _timeout;
    
    /// <summary>
    /// Gets the collection of tests that are pending.
    /// </summary>
    public IReadOnlyCollection<TestResult> Pending => _pending;

    /// <summary>
    /// Gets the total number of test results in this set.
    /// </summary>
    public int Count => _all.Count;
    
    /// <summary>
    /// Gets or sets the name of the test run.
    /// </summary>
    public string TestRunName { get; set; } = $"{Environment.UserName}@{Environment.MachineName} {DateTime.Now:O}";
    
    /// <summary>
    /// Gets or sets the full path to the TRX file that was parsed.
    /// </summary>
    public string TestFilePath { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the test run ID from the TRX file.
    /// </summary>
    public string TestRunId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the deployment root path from the TRX file.
    /// </summary>
    public string DeploymentRoot { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the test settings name from the TRX file.
    /// </summary>
    public string TestSettingsName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the original TestRun object that was used to create this TestResultSet.
    /// Contains the raw XML data from the TRX file.
    /// </summary>
    public TrxLib.TestRun? OriginalTestRun { get; set; }

    /// <summary>
    /// Gets a reference to the TestDefinitions from the original TestRun.
    /// </summary>
    public TrxLib.TestDefinitions? TestDefinitions => OriginalTestRun?.TestDefinitions;

    /// <summary>
    /// Gets a reference to the Results from the original TestRun.
    /// </summary>
    public TrxLib.Results? Results => OriginalTestRun?.Results;

    /// <summary>
    /// Gets a reference to the Times from the original TestRun.
    /// </summary>
    public TrxLib.Times? Times => OriginalTestRun?.Times;

    /// <summary>
    /// Gets a reference to the TestSettings from the original TestRun.
    /// </summary>
    public TrxLib.TestSettings? TestSettings => OriginalTestRun?.TestSettings;

    /// <summary>
    /// Gets the test result at the specified index.
    /// </summary>
    /// <param name="index">The zero-based index of the test result to get.</param>
    /// <returns>The test result at the specified index.</returns>
    public TestResult this[int index] => _all[index];

    /// <summary>
    /// Returns an enumerator that iterates through the test results.
    /// </summary>
    /// <returns>An enumerator that can be used to iterate through the test results.</returns>
    public IEnumerator<TestResult> GetEnumerator() => _all.GetEnumerator();
    
    /// <summary>
    /// Returns an enumerator that iterates through the test results.
    /// </summary>
    /// <returns>An enumerator that can be used to iterate through the test results.</returns>
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <summary>
    /// Adds a test result to the appropriate collections based on its outcome.
    /// </summary>
    /// <param name="testResult">The test result to add.</param>
    public void Add(TestResult testResult)
    {
        _all.Add(testResult);
        switch (testResult.Outcome)
        {
            case TestOutcome.Passed:
                _passed.Add(testResult);
                break;
            case TestOutcome.Failed:
                _failed.Add(testResult);
                break;
            case TestOutcome.NotExecuted:
                _notExecuted.Add(testResult);
                break;
            case TestOutcome.Inconclusive:
                _inconclusive.Add(testResult);
                break;
            case TestOutcome.Timeout:
                _timeout.Add(testResult);
                break;
            case TestOutcome.Pending:
                _pending.Add(testResult);
                break;
        }
    }
}