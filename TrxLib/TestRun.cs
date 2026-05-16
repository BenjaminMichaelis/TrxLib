using System.Xml.Serialization;

namespace TrxLib;

/// <summary>
/// Represents the root element of a TRX (Test Results XML) file.
/// Contains all test definitions, results, and metadata about the test run.
/// </summary>
[XmlRoot("TestRun", Namespace = "http://microsoft.com/schemas/VisualStudio/TeamTest/2010")]
public class TestRun
{
    /// <summary>
    /// Gets or sets the collection of test results from the test run.
    /// </summary>
    [XmlElement("Results")]
    public Results? Results { get; set; }

    /// <summary>
    /// Gets or sets the collection of test definitions used in the test run.
    /// </summary>
    [XmlElement("TestDefinitions")]
    public TestDefinitions? TestDefinitions { get; set; }

    /// <summary>
    /// Gets or sets the name of the test run.
    /// </summary>
    [XmlAttribute("name")]
    public string? Name { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier of the test run.
    /// </summary>
    [XmlAttribute("id")]
    public string? Id { get; set; }

    /// <summary>
    /// Gets or sets the timing information for the test run.
    /// </summary>
    [XmlElement("Times")]
    public Times? Times { get; set; }

    /// <summary>
    /// Gets or sets the configuration settings used for the test run.
    /// </summary>
    [XmlElement("TestSettings")]
    public TestSettings? TestSettings { get; set; }

    /// <summary>
    /// Gets or sets the user account that initiated the test run.
    /// </summary>
    [XmlAttribute("runUser")]
    public string? RunUser { get; set; }

    /// <summary>
    /// Gets or sets the result summary for the test run, including the overall outcome
    /// and authoritative vstest-computed counters.
    /// </summary>
    [XmlElement("ResultSummary")]
    public ResultSummary? ResultSummary { get; set; }

    /// <summary>
    /// Gets or sets the test list categories used to group results.
    /// </summary>
    [XmlElement("TestLists")]
    public TestLists? TestLists { get; set; }

    /// <summary>
    /// Gets or sets the test entries index linking test IDs to execution IDs.
    /// </summary>
    [XmlElement("TestEntries")]
    public TestEntries? TestEntries { get; set; }
}
