using System.Xml.Serialization;

namespace TrxLib;

/// <summary>
/// Represents the result of a unit test execution in a TRX (Test Results XML) file.
/// This class is used for XML deserialization of test results from Visual Studio or other test runners.
/// </summary>
public class UnitTestResult
{
    /// <summary>
    /// Gets or sets the unique identifier of the test definition referenced by this test result.
    /// This ID maps to a UnitTest element in the TestDefinitions section.
    /// </summary>
    [XmlAttribute("testId")]
    public string? TestId { get; set; }

    /// <summary>
    /// Gets or sets the name of the test that was executed.
    /// </summary>
    [XmlAttribute("testName")]
    public string? TestName { get; set; }

    /// <summary>
    /// Gets or sets the outcome of the test execution.
    /// Common values include "Passed", "Failed", "NotExecuted", "Inconclusive", "Timeout", and "Pending".
    /// </summary>
    [XmlAttribute("outcome")]
    public string? Outcome { get; set; }

    /// <summary>
    /// Gets or sets the start time of the test execution in ISO 8601 format.
    /// </summary>
    [XmlAttribute("startTime")]
    public string? StartTime { get; set; }

    /// <summary>
    /// Gets or sets the end time of the test execution in ISO 8601 format.
    /// </summary>
    [XmlAttribute("endTime")]
    public string? EndTime { get; set; }

    /// <summary>
    /// Gets or sets the duration of the test execution, typically in the format "hh:mm:ss.fffffff".
    /// </summary>
    [XmlAttribute("duration")]
    public string? Duration { get; set; }

    /// <summary>
    /// Gets or sets the name of the computer where the test was executed.
    /// </summary>
    [XmlAttribute("computerName")]
    public string? ComputerName { get; set; }

    /// <summary>
    /// Gets or sets the output information of the test execution, including error information and standard output.
    /// </summary>
    [XmlElement("Output")]
    public Output? Output { get; set; }
}
