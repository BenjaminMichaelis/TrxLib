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
    /// The test run was aborted before the test completed. This is distinct from
    /// <see cref="NotExecuted"/> (deliberately skipped) — the test was running but stopped.
    /// </summary>
    Aborted,

    /// <summary>
    /// The test agent was disconnected during execution.
    /// </summary>
    Disconnected,

    /// <summary>
    /// The test passed but produced warnings.
    /// </summary>
    Warning,

    /// <summary>
    /// An infrastructure error occurred during test execution (not a test assertion failure).
    /// </summary>
    Error,

    /// <summary>
    /// The test could not be run, typically due to incorrect configuration or missing dependencies.
    /// </summary>
    NotRunnable,

    /// <summary>
    /// The individual test passed, but the overall test run was aborted.
    /// </summary>
    PassedButRunAborted,

    /// <summary>
    /// The test was still in progress when results were collected.
    /// </summary>
    InProgress
}
