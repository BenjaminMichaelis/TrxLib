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
    Pending,

    /// <summary>
    /// A system error occurred during test execution (e.g., failure to copy result attachments).
    /// In TRX files written by vstest, this outcome is represented by the absence of the
    /// <c>outcome</c> attribute on <c>&lt;UnitTestResult&gt;</c> (Error is the enum default).
    /// </summary>
    Error,

    /// <summary>
    /// The test was aborted by the framework (not by a user gesture).
    /// </summary>
    Aborted,

    /// <summary>
    /// The test could not be run because <c>ITestElement.IsRunnable</c> is <c>false</c>.
    /// </summary>
    NotRunnable,

    /// <summary>
    /// The test run was disconnected before it finished.
    /// </summary>
    Disconnected,

    /// <summary>
    /// The test produced a warning-level result. Typically a run-level outcome.
    /// </summary>
    Warning,

    /// <summary>
    /// The test completed but no qualitative measure of completeness was established.
    /// Typically a run-level outcome.
    /// </summary>
    Completed,

    /// <summary>
    /// The test is currently in progress.
    /// </summary>
    InProgress,

    /// <summary>
    /// The test passed but the run was aborted before all tests completed.
    /// </summary>
    PassedButRunAborted,
}
