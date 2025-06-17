using System.Xml.Serialization;

namespace TrxLib;

/// <summary>
/// Represents the collection of test results in a TRX (Test Results XML) file.
/// Contains all individual test execution results from the test run.
/// </summary>
public class Results
{
    /// <summary>
    /// Gets or sets the list of unit test results from the test run.
    /// </summary>
    [XmlElement("UnitTestResult")]
    public List<UnitTestResult>? UnitTestResults { get; set; }
}
