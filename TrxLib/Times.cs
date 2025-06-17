using System.Xml.Serialization;

namespace TrxLib;

/// <summary>
/// Represents the timing information for a test run in a TRX (Test Results XML) file.
/// Contains timestamps for the creation, queuing, start, and finish of the test run.
/// </summary>
public class Times
{
    /// <summary>
    /// Gets or sets the timestamp when the test run was created.
    /// </summary>
    [XmlAttribute("creation")]
    public string? Creation { get; set; }

    /// <summary>
    /// Gets or sets the timestamp when the test run was queued for execution.
    /// </summary>
    [XmlAttribute("queuing")]
    public string? Queuing { get; set; }

    /// <summary>
    /// Gets or sets the timestamp when the test run started execution.
    /// </summary>
    [XmlAttribute("start")]
    public string? Start { get; set; }

    /// <summary>
    /// Gets or sets the timestamp when the test run finished execution.
    /// </summary>
    [XmlAttribute("finish")]
    public string? Finish { get; set; }
}