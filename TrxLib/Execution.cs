using System.Xml.Serialization;

namespace TrxLib;

/// <summary>
/// Represents the execution element of a unit test definition in a TRX file.
/// Contains the execution ID that links a test definition to its result.
/// </summary>
public class Execution
{
    /// <summary>
    /// Gets or sets the execution identifier. Links this test definition to the
    /// corresponding UnitTestResult via its executionId attribute.
    /// </summary>
    [XmlAttribute("id")]
    public string? Id { get; set; }
}
