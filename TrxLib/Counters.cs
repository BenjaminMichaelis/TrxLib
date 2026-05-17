using System.Xml.Serialization;

namespace TrxLib;

/// <summary>
/// Represents the test result counters from the ResultSummary element of a TRX file.
/// Contains the authoritative vstest-computed counts for each outcome category.
/// </summary>
public class Counters
{
    /// <summary>Gets or sets the total number of tests.</summary>
    [XmlAttribute("total")]
    public int Total { get; set; }

    /// <summary>Gets or sets the number of tests that were executed.</summary>
    [XmlAttribute("executed")]
    public int Executed { get; set; }

    /// <summary>Gets or sets the number of tests that passed.</summary>
    [XmlAttribute("passed")]
    public int Passed { get; set; }

    /// <summary>Gets or sets the number of tests that failed.</summary>
    [XmlAttribute("failed")]
    public int Failed { get; set; }

    /// <summary>Gets or sets the number of tests that encountered a system error.</summary>
    [XmlAttribute("error")]
    public int Error { get; set; }

    /// <summary>Gets or sets the number of tests that timed out.</summary>
    [XmlAttribute("timeout")]
    public int Timeout { get; set; }

    /// <summary>Gets or sets the number of tests that were aborted.</summary>
    [XmlAttribute("aborted")]
    public int Aborted { get; set; }

    /// <summary>Gets or sets the number of tests with inconclusive results.</summary>
    [XmlAttribute("inconclusive")]
    public int Inconclusive { get; set; }

    /// <summary>Gets or sets the number of tests that passed but the run was aborted.</summary>
    [XmlAttribute("passedButRunAborted")]
    public int PassedButRunAborted { get; set; }

    /// <summary>Gets or sets the number of tests that were not runnable.</summary>
    [XmlAttribute("notRunnable")]
    public int NotRunnable { get; set; }

    /// <summary>Gets or sets the number of tests that were not executed.</summary>
    [XmlAttribute("notExecuted")]
    public int NotExecuted { get; set; }

    /// <summary>Gets or sets the number of tests that were disconnected.</summary>
    [XmlAttribute("disconnected")]
    public int Disconnected { get; set; }

    /// <summary>Gets or sets the number of tests with a warning outcome.</summary>
    [XmlAttribute("warning")]
    public int Warning { get; set; }

    /// <summary>Gets or sets the number of completed tests.</summary>
    [XmlAttribute("completed")]
    public int Completed { get; set; }

    /// <summary>Gets or sets the number of tests currently in progress.</summary>
    [XmlAttribute("inProgress")]
    public int InProgress { get; set; }

    /// <summary>Gets or sets the number of tests that are pending.</summary>
    [XmlAttribute("pending")]
    public int Pending { get; set; }
}
