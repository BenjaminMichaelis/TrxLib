using AwesomeAssertions;

namespace TrxLib.Tests;

/// <summary>
/// Regression tests for bugs found by comparing our object model to the
/// upstream vstest TRX ObjectModel.
/// </summary>
public class KnownBugsTests
{
    // -------------------------------------------------------------------------
    // Bug 3 – All vstest outcome strings (Error, Aborted, NotRunnable, Disconnected,
    //          Warning, Completed, InProgress, PassedButRunAborted) must round-trip
    //          correctly through the parser. A missing outcome attribute must map
    //          to TestOutcome.Error, not NotExecuted.
    // -------------------------------------------------------------------------

    [Fact]
    public void Bug_Parse_ErrorOutcome_IsNotSilentlyMappedToNotExecuted()
    {
        // vstest writes outcome="Error" when the test adapter itself crashes or the
        // test process aborts unexpectedly. This is NOT the same as "not executed".
        using var trxFile = new TempTrxFile(MinimalTrx(outcome: "Error"));
        var results = TrxParser.Parse(trxFile.FileInfo);

        results.Single().Outcome.Should().Be(TestOutcome.Error,
            because: "outcome=\"Error\" is a distinct vstest state indicating a system error");
    }

    [Fact]
    public void Bug_Parse_AbortedOutcome_IsNotSilentlyMappedToNotExecuted()
    {
        // vstest writes outcome="Aborted" when the framework (not the user) terminates
        // the test mid-execution.
        using var trxFile = new TempTrxFile(MinimalTrx(outcome: "Aborted"));
        var results = TrxParser.Parse(trxFile.FileInfo);

        results.Single().Outcome.Should().Be(TestOutcome.Aborted,
            because: "outcome=\"Aborted\" is a distinct vstest state and must not be coerced to NotExecuted");
    }

    [Fact]
    public void Bug_Parse_NotRunnableOutcome_IsNotSilentlyMappedToNotExecuted()
    {
        // vstest writes outcome="NotRunnable" when ITestElement.IsRunnable == false.
        using var trxFile = new TempTrxFile(MinimalTrx(outcome: "NotRunnable"));
        var results = TrxParser.Parse(trxFile.FileInfo);

        results.Single().Outcome.Should().Be(TestOutcome.NotRunnable,
            because: "outcome=\"NotRunnable\" is a distinct vstest state and must not be coerced to NotExecuted");
    }

    [Fact]
    public void Bug_Parse_MissingOutcomeAttribute_MapsToError()
    {
        // In vstest's serialization, TestOutcome.Error == 0 is the enum default.
        // XmlPersistence.SaveSimpleField() skips writing the attribute when value == default,
        // so a real TRX file with an attachment error has NO outcome= attribute on
        // <UnitTestResult>. A missing attribute is the live form of the Error outcome.
        using var trxFile = new TempTrxFile(MinimalTrx());
        var results = TrxParser.Parse(trxFile.FileInfo);

        results.Single().Outcome.Should().Be(TestOutcome.Error,
            because: "a missing outcome attribute means TestOutcome.Error in vstest's serialization model");
    }

    // -------------------------------------------------------------------------
    // Bug 4 – TestProjectDirectory must be resolved correctly for all standard
    //          .NET SDK output layouts, including RID-qualified and publish
    //          subdirectories nested under bin/.
    // -------------------------------------------------------------------------

    [Fact]
    public void Bug_Parse_TestProjectDirectory_IsCorrectForRidQualifiedOutputPath() =>
        AssertProjectRootResolves("fake_project_rid", "bin", "Debug", "net8.0", "win-x64", "MyProject.dll");

    [Fact]
    public void Bug_Parse_TestProjectDirectory_IsCorrectForPublishOutputPath() =>
        AssertProjectRootResolves("fake_project_publish", "bin", "Release", "net8.0", "publish", "MyProject.dll");

    [Fact]
    public void Bug_Parse_TestProjectDirectory_IsCorrectForRidPlusPublishOutputPath() =>
        AssertProjectRootResolves("fake_project_rid_publish", "bin", "Release", "net8.0", "linux-x64", "publish", "MyProject.dll");

    private static void AssertProjectRootResolves(string subfolder, params string[] relativeSegments)
    {
        var projectRoot = Path.GetFullPath(Path.Combine(Path.GetTempPath(), subfolder));
        var codebase = Path.Combine(new[] { projectRoot }.Concat(relativeSegments).ToArray());
        using var trxFile = new TempTrxFile(MinimalTrx(codeBase: codebase));
        var results = TrxParser.Parse(trxFile.FileInfo);
        results.Single().TestProjectDirectory!.FullName.TrimEnd(Path.DirectorySeparatorChar)
            .Should().Be(projectRoot.TrimEnd(Path.DirectorySeparatorChar));
    }

    // -------------------------------------------------------------------------
    // Helpers
    // -------------------------------------------------------------------------

    private static string MinimalTrx(string? outcome = null, string codeBase = "test.dll")
    {
        const string testId = "aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa";
        const string execId = "bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb";
        var codebaseXml = codeBase.Replace('\\', '/');
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
                  {(outcome is null ? "" : $"outcome=\"{outcome}\"")}
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
