using System.Xml.Serialization;

namespace TrxLib;

/// <summary>
/// Represents a unit test definition in a TRX (Test Results XML) file.
/// Contains metadata about a specific test that was executed during the test run.
/// </summary>
public class UnitTest
{
    /// <summary>
    /// Gets or sets the unique identifier of the unit test.
    /// This ID is referenced by UnitTestResult elements.
    /// </summary>
    [XmlAttribute("id")]
    public string? Id { get; set; }

    /// <summary>
    /// Gets or sets the name of the unit test.
    /// </summary>
    [XmlAttribute("name")]
    public string? Name { get; set; }

    /// <summary>
    /// Gets or sets the test method information for this unit test.
    /// Contains details about the method that implements the test.
    /// </summary>
    [XmlElement("TestMethod")]
    public TestMethod? TestMethod { get; set; }

    /// <summary>
    /// Gets or sets the path to the test assembly. Stored as a lowercased path by vstest.
    /// </summary>
    [XmlAttribute("storage")]
    public string? Storage { get; set; }

    /// <summary>
    /// Gets or sets the execution element containing the execution ID for this test definition.
    /// </summary>
    [XmlElement("Execution")]
    public Execution? Execution { get; set; }
}
