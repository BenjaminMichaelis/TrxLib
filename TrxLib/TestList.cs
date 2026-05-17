using System.Xml.Serialization;

namespace TrxLib;

/// <summary>
/// Represents a single TestList entry in the TestLists section of a TRX file.
/// vstest always writes two default lists: "Results Not in a List" and "All Loaded Results".
/// </summary>
public class TestList
{
    /// <summary>Gets or sets the display name of the test list.</summary>
    [XmlAttribute("name")]
    public string? Name { get; set; }

    /// <summary>Gets or sets the unique identifier of the test list.</summary>
    [XmlAttribute("id")]
    public string? Id { get; set; }
}
