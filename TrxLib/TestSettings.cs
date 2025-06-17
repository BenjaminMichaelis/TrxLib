using System.Xml.Serialization;

namespace TrxLib;

/// <summary>
/// Represents the configuration settings for a test run in a TRX (Test Results XML) file.
/// Contains settings that affect how tests are executed and where outputs are stored.
/// </summary>
public class TestSettings
{
    /// <summary>
    /// Gets or sets the name of the test settings configuration.
    /// </summary>
    [XmlAttribute("name")]
    public string? Name { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier of the test settings.
    /// </summary>
    [XmlAttribute("id")]
    public string? Id { get; set; }

    /// <summary>
    /// Gets or sets the deployment information for the test run.
    /// Contains details about where test files are deployed.
    /// </summary>
    [XmlElement("Deployment")]
    public Deployment? Deployment { get; set; }
}