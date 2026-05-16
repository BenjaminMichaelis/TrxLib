namespace TrxLib;

/// <summary>
/// Represents the test result counters from the ResultSummary element of a TRX file.
/// Contains the authoritative vstest-computed counts for each outcome category.
/// </summary>
public class Counters
{
    /// <summary>Gets or sets the total number of tests.</summary>
    public int Total { get; set; }

    /// <summary>Gets or sets the number of tests that were executed.</summary>
    public int Executed { get; set; }

    /// <summary>Gets or sets the number of tests that passed.</summary>
    public int Passed { get; set; }

    /// <summary>Gets or sets the number of tests that failed.</summary>
    public int Failed { get; set; }

    /// <summary>Gets or sets the number of tests that encountered a system error.</summary>
    public int Error { get; set; }

    /// <summary>Gets or sets the number of tests that timed out.</summary>
    public int Timeout { get; set; }

    /// <summary>Gets or sets the number of tests that were aborted.</summary>
    public int Aborted { get; set; }

    /// <summary>Gets or sets the number of tests with inconclusive results.</summary>
    public int Inconclusive { get; set; }

    /// <summary>Gets or sets the number of tests that passed but the run was aborted.</summary>
    public int PassedButRunAborted { get; set; }

    /// <summary>Gets or sets the number of tests that were not runnable.</summary>
    public int NotRunnable { get; set; }

    /// <summary>Gets or sets the number of tests that were not executed.</summary>
    public int NotExecuted { get; set; }

    /// <summary>Gets or sets the number of tests that were disconnected.</summary>
    public int Disconnected { get; set; }

    /// <summary>Gets or sets the number of tests with a warning outcome.</summary>
    public int Warning { get; set; }

    /// <summary>Gets or sets the number of completed tests.</summary>
    public int Completed { get; set; }

    /// <summary>Gets or sets the number of tests currently in progress.</summary>
    public int InProgress { get; set; }

    /// <summary>Gets or sets the number of tests that are pending.</summary>
    public int Pending { get; set; }
}
