namespace TrxLib;

/// <summary>
/// Represents the ResultSummary element of a TRX file.
/// Contains the overall run outcome, authoritative vstest-computed counters, and run-level output.
/// </summary>
public class ResultSummary
{
    /// <summary>
    /// Gets or sets the overall outcome of the test run (e.g. "Passed", "Failed", "Completed").
    /// </summary>
    public string? Outcome { get; set; }

    /// <summary>
    /// Gets or sets the test result counters for the run.
    /// </summary>
    public Counters? Counters { get; set; }

    /// <summary>
    /// Gets or sets the run-level output (e.g. run-level stdout written by the test host).
    /// </summary>
    public Output? Output { get; set; }
}
