namespace TrxLib;

/// <summary>
/// Represents a single TestEntry that links a test definition to its execution record.
/// </summary>
public class TestEntry
{
    /// <summary>Gets or sets the test definition identifier.</summary>
    public string? TestId { get; set; }

    /// <summary>Gets or sets the execution identifier.</summary>
    public string? ExecutionId { get; set; }

    /// <summary>Gets or sets the test list identifier this entry belongs to.</summary>
    public string? TestListId { get; set; }
}
