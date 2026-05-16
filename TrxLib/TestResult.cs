namespace TrxLib;

/// <summary>
/// Represents the result of a single test case execution.
/// </summary>
public class TestResult
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TestResult"/> class.
    /// </summary>
    public TestResult(
        string fullyQualifiedTestName,
        TestOutcome outcome,
        TimeSpan? duration = null,
        DateTimeOffset? startTime = null,
        DateTimeOffset? endTime = null,
        DirectoryInfo? testProjectDirectory = null,
        FileInfo? testOutputFile = null,
        FileInfo? codebase = null,
        string? stackTrace = null,
        string? errorMessage = null,
        string? stdOut = null,
        TestMethod? testMethod = null)
    {
        FullyQualifiedTestName = fullyQualifiedTestName;
        Duration = duration;
        StartTime = startTime;
        EndTime = endTime;
        Outcome = outcome;
        TestProjectDirectory = testProjectDirectory;
        TestOutputFile = testOutputFile;
        Codebase = codebase;
        StackTrace = stackTrace;
        ErrorMessage = errorMessage;
        StdOut = stdOut;
        TestMethod = testMethod;

        if (testMethod != null && !string.IsNullOrEmpty(testMethod.ClassName))
        {
            // Use the authoritative TRX TestMethod.className field directly.
            // Splitting the FQTN on '.' is unreliable when theory-test parameter
            // values contain dots (e.g. "(param: \"foo.bar\")").
            FullyQualifiedClassName = testMethod.ClassName;
            var lastDot = testMethod.ClassName.LastIndexOf('.');
            if (lastDot >= 0)
            {
                ClassName = testMethod.ClassName[(lastDot + 1)..];
                Namespace = testMethod.ClassName[..lastDot];
            }
            else
            {
                ClassName = testMethod.ClassName;
            }

            // TestName = everything after "FullyQualifiedClassName." in the FQTN,
            // which preserves the theory-test parameter suffix intact.
            var prefix = testMethod.ClassName + ".";
            TestName = fullyQualifiedTestName.StartsWith(prefix, StringComparison.Ordinal)
                ? fullyQualifiedTestName[prefix.Length..]
                : (testMethod.Name ?? fullyQualifiedTestName);
        }
        else
        {
            // Fallback: derive from dot-splitting the FQTN.
            // Works for standard dotted names; breaks only when parameter values
            // contain dots and no testMethod metadata is available.
            var testNameParts = fullyQualifiedTestName.Split('.');

            if (testNameParts.Length > 1)
            {
                var fullyQualifiedClassName = string.Join(".", testNameParts.Take(testNameParts.Length - 1));

                // only infer these if fullyQualifiedTestName is typical of .NET tests
                if (!fullyQualifiedClassName.Contains(" "))
                {
                    TestName = testNameParts[^1];
                    ClassName = testNameParts[^2];
                    FullyQualifiedClassName = fullyQualifiedClassName;
                    Namespace = string.Join(".", testNameParts.Take(testNameParts.Length - 2));
                }
            }

            if (string.IsNullOrEmpty(TestName))
            {
                TestName = FullyQualifiedTestName;
            }
        }
    }

    /// <summary>
    /// Gets or sets the namespace of the test class.
    /// </summary>
    public string? Namespace { get; set; }
    /// <summary>
    /// Gets or sets the test method name.
    /// </summary>
    public string TestName { get; set; }
    /// <summary>
    /// Gets the fully qualified test name.
    /// </summary>
    public string FullyQualifiedTestName { get; }
    /// <summary>
    /// Gets or sets the fully qualified class name.
    /// </summary>
    public string? FullyQualifiedClassName { get; set; }
    /// <summary>
    /// Gets or sets the computer name where the test was run.
    /// </summary>
    public string? ComputerName { get; set; }
    /// <summary>
    /// Gets or sets the class name containing the test.
    /// </summary>
    public string? ClassName { get; set; }

    /// <summary>
    /// Gets the outcome of the test.
    /// </summary>
    public TestOutcome Outcome { get; }

    /// <summary>
    /// Gets the directory of the test project.
    /// </summary>
    public DirectoryInfo? TestProjectDirectory { get; }
    /// <summary>
    /// Gets the test output file.
    /// </summary>
    public FileInfo? TestOutputFile { get; }
    /// <summary>
    /// Gets the codebase file.
    /// </summary>
    public FileInfo? Codebase { get; }

    /// <summary>
    /// Gets the start time of the test.
    /// </summary>
    public DateTimeOffset? StartTime { get; }
    /// <summary>
    /// Gets the end time of the test.
    /// </summary>
    public DateTimeOffset? EndTime { get; }
    /// <summary>
    /// Gets the duration of the test.
    /// </summary>
    public TimeSpan? Duration { get; }

    /// <summary>
    /// Gets or sets the error message if the test failed.
    /// </summary>
    public string? ErrorMessage { get; set; }
    /// <summary>
    /// Gets the stack trace if the test failed.
    /// </summary>
    public string? StackTrace { get; }
    /// <summary>
    /// Gets or sets the standard output of the test.
    /// </summary>
    public string? StdOut { get; set; }

    /// <summary>
    /// Gets the test method metadata.
    /// </summary>
    public TestMethod? TestMethod { get; }

    /// <inheritdoc/>
    public override string ToString()
    {
        var badge = Outcome switch
        {
            TestOutcome.Failed => "❌",
            TestOutcome.Passed => "✅",
            TestOutcome.NotExecuted => "⚠️",
            TestOutcome.Inconclusive => "⚠️",
            TestOutcome.Timeout => "⌚",
            TestOutcome.Pending => "⏳",
            _ => "❓"
        };

        return $"{badge} {FullyQualifiedTestName}";
    }
}