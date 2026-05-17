using AwesomeAssertions;

namespace TrxLib.Tests;

/// <summary>
/// Tests that document currently known bugs found by comparing our object model to the
/// upstream vstest TRX ObjectModel (commit ba0077af).
/// Every test in this file is expected to FAIL until the corresponding bug is fixed.
/// </summary>
public class KnownBugsTests
{
    // -------------------------------------------------------------------------
    // Bug 3 – Outcomes defined by vstest (Error, Aborted, NotRunnable, Disconnected,
    //          Warning, Completed, InProgress, PassedButRunAborted) are absent from
    //          local TestOutcome enum.  The parser's catch-all arm (TrxParser.cs:52)
    //          maps every unrecognised string to NotExecuted, causing silent
    //          misclassification of distinct failure modes.
    // -------------------------------------------------------------------------

    [Fact]
    public void Bug_Parse_ErrorOutcome_IsNotSilentlyMappedToNotExecuted()
    {
        // vstest writes outcome="Error" when the test adapter itself crashes or the
        // test process aborts unexpectedly.  This is NOT the same as "not executed".
        using var trxFile = new TempTrxFile(MinimalTrxWithOutcome("Error"));
        var results = TrxParser.Parse(trxFile.FileInfo);

        // FAILS: TrxParser catch-all maps "error" → TestOutcome.NotExecuted (line 51).
        results.Single().Outcome.Should().NotBe(TestOutcome.NotExecuted,
            because: "outcome=\"Error\" is a distinct vstest state and must not be coerced to NotExecuted");
    }

    [Fact]
    public void Bug_Parse_AbortedOutcome_IsNotSilentlyMappedToNotExecuted()
    {
        // vstest writes outcome="Aborted" when the framework (not the user) terminates
        // the test mid-execution.
        using var trxFile = new TempTrxFile(MinimalTrxWithOutcome("Aborted"));
        var results = TrxParser.Parse(trxFile.FileInfo);

        // FAILS: TrxParser catch-all maps "aborted" → TestOutcome.NotExecuted (line 51).
        results.Single().Outcome.Should().NotBe(TestOutcome.NotExecuted,
            because: "outcome=\"Aborted\" is a distinct vstest state and must not be coerced to NotExecuted");
    }

    [Fact]
    public void Bug_Parse_NotRunnableOutcome_IsNotSilentlyMappedToNotExecuted()
    {
        // vstest writes outcome="NotRunnable" when ITestElement.IsRunnable == false.
        using var trxFile = new TempTrxFile(MinimalTrxWithOutcome("NotRunnable"));
        var results = TrxParser.Parse(trxFile.FileInfo);

        // FAILS: TrxParser catch-all maps "notrunnable" → TestOutcome.NotExecuted (line 51).
        results.Single().Outcome.Should().NotBe(TestOutcome.NotExecuted,
            because: "outcome=\"NotRunnable\" is a distinct vstest state and must not be coerced to NotExecuted");
    }

    // -------------------------------------------------------------------------
    // Bug 4 – Test project directory is derived by a hardcoded 3-level upward
    //          traversal from the codeBase DLL path (TrxParser.cs:96-101).
    //          This breaks for .NET 8+ RID-qualified output layouts where the DLL
    //          is 4 levels below the project root:
    //            <project>/bin/Debug/net8.0/<rid>/Foo.dll
    // -------------------------------------------------------------------------

    [Fact]
    public void Bug_Parse_TestProjectDirectory_IsCorrectForRidQualifiedOutputPath()
    {
        // Represents: <project>/bin/Debug/net8.0/win-x64/MyProject.dll
        // Going up 3 levels lands on bin/ — one level too shallow.
        var projectRoot = Path.GetFullPath(Path.Combine(Path.GetTempPath(), "fake_project_rid"));
        var codebase    = Path.Combine(projectRoot, "bin", "Debug", "net8.0", "win-x64", "MyProject.dll");

        using var trxFile = new TempTrxFile(MinimalTrxWithCodebase(codebase));
        var results = TrxParser.Parse(trxFile.FileInfo);

        // FAILS: hardcoded 3-level traversal returns bin/ instead of the project root.
        results.Single().TestProjectDirectory!.FullName.TrimEnd(Path.DirectorySeparatorChar)
            .Should().Be(projectRoot.TrimEnd(Path.DirectorySeparatorChar));
    }

    // -------------------------------------------------------------------------
    // Helpers
    // -------------------------------------------------------------------------

    private static string MinimalTrxWithOutcome(string outcome)
    {
        const string testId = "aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa";
        const string execId = "bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb";
        return $"""
            <?xml version="1.0" encoding="UTF-8"?>
            <TestRun id="cccccccc-cccc-cccc-cccc-cccccccccccc" name="test run"
                     xmlns="http://microsoft.com/schemas/VisualStudio/TeamTest/2010">
              <Results>
                <UnitTestResult executionId="{execId}" testId="{testId}"
                  testName="SomeNamespace.SomeClass.SomeTest" computerName="host"
                  duration="00:00:00.0010000"
                  startTime="2024-01-01T00:00:00.0000000Z"
                  endTime="2024-01-01T00:00:00.0000000Z"
                  testType="13cdc9d9-ddb5-4fa4-a97d-d965ccfc6d4b"
                  outcome="{outcome}"
                  testListId="8c84fa94-04c1-424b-9868-57a2d4851a1d"
                  relativeResultsDirectory="{execId}" />
              </Results>
              <TestDefinitions>
                <UnitTest name="SomeTest" id="{testId}">
                  <TestMethod className="SomeNamespace.SomeClass" name="SomeTest"
                    adapterTypeName="executor://mstestadapter/v2" codeBase="test.dll" />
                </UnitTest>
              </TestDefinitions>
            </TestRun>
            """;
    }

    private static string MinimalTrxWithCodebase(string codebasePath)
    {
        const string testId = "aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa";
        const string execId = "bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb";
        // XML attribute value must use forward slashes to avoid escaping issues
        var codebaseXml = codebasePath.Replace('\\', '/');
        return $"""
            <?xml version="1.0" encoding="UTF-8"?>
            <TestRun id="cccccccc-cccc-cccc-cccc-cccccccccccc" name="test run"
                     xmlns="http://microsoft.com/schemas/VisualStudio/TeamTest/2010">
              <Results>
                <UnitTestResult executionId="{execId}" testId="{testId}"
                  testName="SomeNamespace.SomeClass.SomeTest" computerName="host"
                  duration="00:00:00.0010000"
                  startTime="2024-01-01T00:00:00.0000000Z"
                  endTime="2024-01-01T00:00:00.0000000Z"
                  testType="13cdc9d9-ddb5-4fa4-a97d-d965ccfc6d4b"
                  outcome="Passed"
                  testListId="8c84fa94-04c1-424b-9868-57a2d4851a1d"
                  relativeResultsDirectory="{execId}" />
              </Results>
              <TestDefinitions>
                <UnitTest name="SomeTest" id="{testId}">
                  <TestMethod className="SomeNamespace.SomeClass" name="SomeTest"
                    adapterTypeName="executor://mstestadapter/v2"
                    codeBase="{codebaseXml}" />
                </UnitTest>
              </TestDefinitions>
            </TestRun>
            """;
    }

    /// <summary>Helper that writes a TRX string to a temp file and deletes it on dispose.</summary>
    private sealed class TempTrxFile : IDisposable
    {
        public FileInfo FileInfo { get; }

        public TempTrxFile(string content)
        {
            var path = Path.Combine(Path.GetTempPath(), $"trxtest_{Guid.NewGuid():N}.trx");
            File.WriteAllText(path, content);
            FileInfo = new FileInfo(path);
        }

        public void Dispose() => FileInfo.Delete();
    }
}
