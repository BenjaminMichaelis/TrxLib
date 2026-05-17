using System.IO;

using AwesomeAssertions;

namespace TrxLib.Tests;

/// <summary>
/// Regression tests verifying TRX parsing correctness against the vstest object model.
/// </summary>
public class TrxParserRegressionTests
{
    // All vstest outcome strings must round-trip correctly through the parser.
    // A null/missing outcome attribute maps to Error: TestOutcome.Error is ordinal 0
    // (the enum default), so vstest omits the attribute rather than writing "Error".
    [Test]
    [Arguments("Error",       TestOutcome.Error)]
    [Arguments("Aborted",     TestOutcome.Aborted)]
    [Arguments("NotRunnable", TestOutcome.NotRunnable)]
    [Arguments(null,          TestOutcome.Error)] // absent attribute = Error
    public void Parse_OutcomeAttribute_RoundTrips(string? outcomeAttr, TestOutcome expected)
    {
        using var trxFile = new TempTrxFile(MinimalTrx(outcome: outcomeAttr));
        var results = TrxParser.Parse(trxFile.FileInfo);
        results.Single().Outcome.Should().Be(expected);
    }

    // TestProjectDirectory must resolve to the project root for all standard .NET SDK
    // output layouts, including RID-qualified and publish subdirectories nested under bin/.
    public static IEnumerable<(string, string[])> DirectoryLayouts =>
    [
        ("fake_project_rid",         ["bin", "Debug",   "net8.0",                            "win-x64", "MyProject.dll"]),
        ("fake_project_publish",     ["bin", "Release", "net8.0",              "publish",               "MyProject.dll"]),
        ("fake_project_rid_publish", ["bin", "Release", "net8.0", "linux-x64", "publish",               "MyProject.dll"]),
    ];

    [Test, MethodDataSource(nameof(DirectoryLayouts))]
    public void Parse_TestProjectDirectory_ResolvesFromBinAnchor(string subfolder, string[] segments)
    {
        var projectRoot = Path.GetFullPath(Path.Combine(Path.GetTempPath(), subfolder));
        var codebase = Path.Combine(new[] { projectRoot }.Concat(segments).ToArray());
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
