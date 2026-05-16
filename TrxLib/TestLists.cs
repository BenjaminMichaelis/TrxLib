using System.Xml.Serialization;

namespace TrxLib;

/// <summary>
/// Represents the TestLists element of a TRX file.
/// Contains the list categories used to group test results.
/// </summary>
public class TestLists
{
    /// <summary>Gets or sets the individual test list entries.</summary>
    [XmlElement("TestList")]
    public List<TestList>? Items { get; set; }
}
