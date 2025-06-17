using System.Xml.Serialization;

namespace TrxLib;

/// <summary>
/// Represents deployment information for a test run in a TRX (Test Results XML) file.
/// Contains details about where test files and results are deployed.
/// </summary>
public class Deployment
{
    /// <summary>
    /// Gets or sets the root directory path where test run files are deployed.
    /// </summary>
    [XmlAttribute("runDeploymentRoot")]
    public string? RunDeploymentRoot { get; set; }
}