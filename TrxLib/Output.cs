using System.Xml.Serialization;

namespace TrxLib;

/// <summary>
/// Represents the output data from a test execution in a TRX (Test Results XML) file.
/// Contains error information and standard output captured during the test execution.
/// </summary>
public class Output
{
    /// <summary>
    /// Gets or sets the error information if the test failed.
    /// Contains the error message and stack trace details.
    /// </summary>
    [XmlElement("ErrorInfo")]
    public ErrorInfo? ErrorInfo { get; set; }

    /// <summary>
    /// Gets or sets the standard output text captured during test execution.
    /// </summary>
    [XmlElement("StdOut")]
    public string? StdOut { get; set; }
}
