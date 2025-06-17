using System.Xml.Serialization;

namespace TrxLib;

/// <summary>
/// Represents error information for a failed test in a TRX (Test Results XML) file.
/// Contains the error message and stack trace details of the test failure.
/// </summary>
public class ErrorInfo
{
    /// <summary>
    /// Gets or sets the error message describing why the test failed.
    /// </summary>
    [XmlElement("Message")]
    public string? Message { get; set; }

    /// <summary>
    /// Gets or sets the stack trace information from the test failure.
    /// Contains the call stack at the point of the exception that caused the test to fail.
    /// </summary>
    [XmlElement("StackTrace")]
    public string? StackTrace { get; set; }
}
