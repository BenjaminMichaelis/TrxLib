namespace TrxLib;

/// <summary>
/// Represents a test method definition in a TRX (Test Results XML) file.
/// Contains metadata about the test method including its location and identity.
/// </summary>
public class TestMethod
{
    /// <summary>
    /// Gets or sets the path to the assembly containing the test method.
    /// </summary>
    public string? CodeBase { get; set; }

    /// <summary>
    /// Gets or sets the fully qualified name of the class containing the test method.
    /// </summary>
    public string? ClassName { get; set; }

    /// <summary>
    /// Gets or sets the name of the test method.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Gets or sets the fully qualified name of the test adapter used to run the test.
    /// </summary>
    public string? AdapterTypeName { get; set; }
}
