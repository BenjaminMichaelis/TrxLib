namespace TrxLib;

/// <summary>
/// Represents the possible outcomes of a test execution.
/// </summary>
public enum TestOutcome
{
    /// <summary>
    /// The test was executed successfully and all assertions passed.
    /// </summary>
    Passed,
    
    /// <summary>
    /// The test was executed but failed one or more assertions or threw an exception.
    /// </summary>
    Failed,
    
    /// <summary>
    /// The test was not executed, typically due to being skipped or ignored.
    /// </summary>
    NotExecuted,
    
    /// <summary>
    /// The test execution completed but the result was inconclusive.
    /// </summary>
    Inconclusive,
    
    /// <summary>
    /// The test execution exceeded the allowed time limit.
    /// </summary>
    Timeout,
    
    /// <summary>
    /// The test is awaiting execution or further action.
    /// </summary>
    Pending
}
