using System.Xml.Serialization;

namespace TrxLib;

/// <summary>
/// Represents the TestEntries element of a TRX file.
/// Contains the execution index linking test IDs to execution IDs and test list categories.
/// </summary>
public class TestEntries
{
    /// <summary>Gets or sets the individual test entry records.</summary>
    [XmlElement("TestEntry")]
    public List<TestEntry>? Items { get; set; }
}
