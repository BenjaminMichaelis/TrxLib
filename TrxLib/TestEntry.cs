using System.Xml.Serialization;

namespace TrxLib;

/// <summary>
/// Represents a single TestEntry in the TestEntries section of a TRX file.
/// Links a test definition (testId) to its execution record (executionId) and list category (testListId).
/// </summary>
public class TestEntry
{
    /// <summary>Gets or sets the test definition identifier.</summary>
    [XmlAttribute("testId")]
    public string? TestId { get; set; }

    /// <summary>Gets or sets the execution identifier.</summary>
    [XmlAttribute("executionId")]
    public string? ExecutionId { get; set; }

    /// <summary>Gets or sets the test list identifier.</summary>
    [XmlAttribute("testListId")]
    public string? TestListId { get; set; }
}
