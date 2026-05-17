namespace TrxLib;

/// <summary>
/// Represents the root element of a TRX (Test Results XML) file.
/// Contains all test definitions, results, and metadata about the test run.
/// </summary>
public class TestRun
{
    /// <summary>
    /// Gets or sets the collection of test results from the test run.
    /// </summary>
    public Results? Results { get; set; }

    /// <summary>
    /// Gets or sets the collection of test definitions used in the test run.
    /// </summary>
    public TestDefinitions? TestDefinitions { get; set; }

    /// <summary>
    /// Gets or sets the name of the test run.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier of the test run.
    /// </summary>
    public string? Id { get; set; }

    /// <summary>
    /// Gets or sets the timing information for the test run.
    /// </summary>
    public Times? Times { get; set; }

    /// <summary>
    /// Gets or sets the configuration settings used for the test run.
    /// </summary>
    public TestSettings? TestSettings { get; set; }

    /// <summary>
    /// Gets or sets the user account that initiated the test run (the runUser attribute).
    /// </summary>
    public string? RunUser { get; set; }

    /// <summary>
    /// Gets or sets the result summary for the test run, including the overall outcome
    /// and authoritative vstest-computed counters.
    /// </summary>
    public ResultSummary? ResultSummary { get; set; }

    /// <summary>
    /// Gets or sets the test list categories used to group results.
    /// </summary>
    public TestLists? TestLists { get; set; }

    /// <summary>
    /// Gets or sets the test entries index linking test IDs to execution IDs.
    /// </summary>
    public TestEntries? TestEntries { get; set; }
}
