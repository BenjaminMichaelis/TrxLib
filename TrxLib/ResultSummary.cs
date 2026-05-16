namespace TrxLib;

/// <summary>
/// Represents the ResultSummary element of a TRX file, containing the overall
/// outcome and vstest-authoritative test counters for the run.
/// </summary>
public class ResultSummary
{
    /// <summary>
    /// Gets or sets the overall outcome of the test run (e.g., "Passed", "Failed").
    /// </summary>
    public string? Outcome { get; set; }

    /// <summary>
    /// Gets or sets the authoritative test counters computed by vstest.
    /// </summary>
    public Counters? Counters { get; set; }

    /// <summary>
    /// Gets or sets the run-level output (e.g., stdout from the test host).
    /// </summary>
    public Output? Output { get; set; }
}
