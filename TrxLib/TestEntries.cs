namespace TrxLib;

/// <summary>
/// Represents the TestEntries element, which indexes test definitions to their execution records.
/// </summary>
public class TestEntries
{
    /// <summary>Gets or sets the collection of test entry records.</summary>
    public List<TestEntry>? Items { get; set; }
}
