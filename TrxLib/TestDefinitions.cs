namespace TrxLib;

/// <summary>
/// Represents the collection of test definitions in a TRX (Test Results XML) file.
/// Contains all the unit tests that were executed during the test run.
/// </summary>
public class TestDefinitions
{
    /// <summary>
    /// Gets or sets the list of unit test definitions.
    /// </summary>
    public List<UnitTest>? UnitTests { get; set; }
}
