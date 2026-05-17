using System.IO;
using System.Runtime.InteropServices;

using AwesomeAssertions;
using AwesomeAssertions.Execution;

namespace TrxLib.Tests;

public class TrxParserTests
{
    private static string GetSampleFilePath(string fileName)
    {
        return Path.Combine(
            IntelliTect.Multitool.RepositoryPaths.GetDefaultRepoRoot(),
            "TrxLib.Tests", "SampleTrxFiles", fileName);
    }

    [Test]
    public void Parse_OneTestFailureTrx_ParsesClassNamesCorrectly()
    {
        var results = TrxParser.Parse(new FileInfo(GetSampleFilePath("OneTestFailure.trx")));
        results.Should().HaveCount(19);
        results.Select(r => r.TestMethod?.ClassName + "." + r.TestMethod?.Name ?? throw new InvalidOperationException("TestMethod is null"))
               .Should()
               .ContainEquivalentOf("AddisonWesley.Michaelis.EssentialCSharp.Chapter01.Listing01_03.Tests.HelloWorldTests.Main_UpDown");
    }

    [Test]
    public void Parse_OneTestFailureTrx_ParsesTestNamesCorrectly()
    {
        var results = TrxParser.Parse(new FileInfo(GetSampleFilePath("OneTestFailure.trx")));
        results.Select(r => r.FullyQualifiedTestName)
               .Should()
               .Contain("AddisonWesley.Michaelis.EssentialCSharp.Chapter01.Listing01_03.Tests.HelloWorldTests.Main_UpDown");
    }

    [Test]
    public void Parse_OneTestFailureTrx_CorrectlyParsesOutcomes()
    {
        var results = TrxParser.Parse(new FileInfo(GetSampleFilePath("OneTestFailure.trx")));
        // 19 tests in the file, 18 passed, 1 failed
        results.Should().HaveCount(19);
        results.Count(r => r.Outcome == TestOutcome.Passed).Should().Be(18);
        results.Count(r => r.Outcome == TestOutcome.Failed).Should().Be(1);
    }

    [Test]
    public void Parse_OneTestFailureTrx_ParsesErrorMessageAndStackTraceCorrectly()
    {
        var results = TrxParser.Parse(new FileInfo(GetSampleFilePath("OneTestFailure.trx")));
        var errorTest = results.FirstOrDefault(t => t.ErrorMessage?.Contains("System.Exception") == true);
        errorTest.Should().NotBeNull();

        errorTest.ErrorMessage.Should().Contain("The expected length of 15 does not match the output length of 8.");

        errorTest.StackTrace.Should().Contain("IntelliTect.TestTools.Console.ConsoleAssert.AssertExpectation(String expectedOutput, String output, Func`3 areEquivalentOperator, String equivalentOperatorErrorMessage)");
        errorTest.StackTrace.Should().Contain("AddisonWesley.Michaelis.EssentialCSharp.Chapter01.Listing01_03.Tests.HelloWorldTests.Main_UpDown()");
    }

    [Test]
    public void Parse_Example1OSXTrx_ParsesTestNamesCorrectly()
    {
        var results = TrxParser.Parse(new FileInfo(GetSampleFilePath(Path.Combine("1", "example1_OSX.trx"))));
        results.Select(r => r.FullyQualifiedTestName)
               .Should()
               .Contain("Microsoft.DotNet.Cli.Utils.Tests.GivenARootedCommandResolver.It_returns_a_CommandSpec_with_CommandName_as_Path_when_CommandName_is_rooted");
    }

    [Test]
    public void Parse_Example1OSXTrx_ParsesOutcomesCorrectly()
    {
        var results = TrxParser.Parse(new FileInfo(GetSampleFilePath(Path.Combine("1", "example1_OSX.trx"))));
        // 88 tests in the file, 83 passed, 4 not executed, 1 failed
        results.Should().HaveCount(88);
        results.Count(r => r.Outcome == TestOutcome.Passed).Should().Be(83);
        results.Count(r => r.Outcome == TestOutcome.NotExecuted).Should().Be(4);
        results.Count(r => r.Outcome == TestOutcome.Failed).Should().Be(1);
    }

    [Test]
    public void Parse_ComplexTrx_ParsesErrorMessageAndStackTraceCorrectly()
    {
        var results = TrxParser.Parse(new FileInfo(GetSampleFilePath("complex.trx")));
        var errorTest = results.FirstOrDefault(t => t.ErrorMessage?.Contains("Index (zero based)") == true);
        errorTest.Should().NotBeNull();
        errorTest.StackTrace.Should().Contain("System.Text.StringBuilder.AppendFormatHelper");
    }

    [Test]
    public void Parse_ComplexTrx_ParsesStdOutCorrectly()
    {
        var results = TrxParser.Parse(new FileInfo(GetSampleFilePath("complex.trx")));
        var stdOutTest = results.FirstOrDefault(t => t.FullyQualifiedTestName == "System.CommandLine.Tests.ParserTests.Option_arguments_can_match_subcommands");
        stdOutTest.Should().NotBeNull();
        stdOutTest.StdOut.Should().Contain("ParseResult: ![ testhost.net462.x86");
    }

    [Test]
    public void Parse_Example1WindowsTrx_ParsesDurationsCorrectly()
    {
        var results = TrxParser.Parse(new FileInfo(GetSampleFilePath(Path.Combine("example1_Windows.trx"))));
        var test = results.Single(r => r.FullyQualifiedTestName == "Microsoft.DotNet.Cli.Sln.Internal.Tests.GivenAnSlnFile.WhenGivenAValidPathItReadsAnSlnFile");
        test.Duration.Should().Be(TimeSpan.FromMilliseconds(138));
    }

    [Test]
    public void Parse_Example1WindowsTrx_ParsesStartTimesCorrectly()
    {
        var results = TrxParser.Parse(new FileInfo(GetSampleFilePath(Path.Combine("example1_Windows.trx"))));
        var test = results.Single(r => r.FullyQualifiedTestName == "Microsoft.DotNet.Cli.Sln.Internal.Tests.GivenAnSlnFile.WhenGivenAValidPathItReadsAnSlnFile");
        test.StartTime.Should().Be(DateTimeOffset.Parse("2016-12-21T11:15:51.8308573-08:00"));
    }

    [Test]
    public void Parse_Example1WindowsTrx_ParsesEndTimesCorrectly()
    {
        var results = TrxParser.Parse(new FileInfo(GetSampleFilePath(Path.Combine("example1_Windows.trx"))));
        var test = results.Single(r => r.FullyQualifiedTestName == "Microsoft.DotNet.Cli.Sln.Internal.Tests.GivenAnSlnFile.WhenGivenAValidPathItReadsAnSlnFile");
        test.EndTime.Should().Be(DateTimeOffset.Parse("2016-12-21T11:15:51.8308573-08:00"));
    }

    [Test]
    public void Parse_Example1WindowsTrx_ParsesPassOutcomeCorrectly()
    {
        var results = TrxParser.Parse(new FileInfo(GetSampleFilePath(Path.Combine("example1_Windows.trx"))));
        var test = results.Single(r => r.FullyQualifiedTestName == "Microsoft.DotNet.Cli.Sln.Internal.Tests.GivenAnSlnFile.WhenGivenAValidPathItReadsAnSlnFile");
        test.Outcome.Should().Be(TestOutcome.Passed);
    }

    [Test]
    public void Parse_Example1WindowsTrx_ParsesFailOutcomeCorrectly()
    {
        var results = TrxParser.Parse(new FileInfo(GetSampleFilePath(Path.Combine("example1_Windows.trx"))));
        var test = results.Single(r => r.FullyQualifiedTestName == "Microsoft.DotNet.Cli.Sln.Internal.Tests.GivenAnSlnFile.WhenGivenAValidPathItReadsModifiesThenWritesAnSln");
        test.Outcome.Should().Be(TestOutcome.Failed);
    }

    [Test, WindowsOnly]
    public void Parse_Example1WindowsTrx_ParsesTestProjectDirectoryCorrectly()
    {
        var results = TrxParser.Parse(new FileInfo(GetSampleFilePath(Path.Combine("example1_Windows.trx"))));
        var test = results.Single(r => r.FullyQualifiedTestName == "Microsoft.DotNet.Cli.Sln.Internal.Tests.GivenAnSlnFile.WhenGivenAValidPathItReadsModifiesThenWritesAnSln");
        test.TestProjectDirectory?.FullName.Should().Be(@"C:\dev\github\cli\test\Microsoft.DotNet.Cli.Sln.Internal.Tests");
    }

    [Test, WindowsOnly]
    public void Parse_Example1WindowsTrx_ParsesCodebaseCorrectly()
    {
        var results = TrxParser.Parse(new FileInfo(GetSampleFilePath(Path.Combine("example1_Windows.trx"))));
        var test = results.Single(r => r.FullyQualifiedTestName == "Microsoft.DotNet.Cli.Sln.Internal.Tests.GivenAnSlnFile.WhenGivenAValidPathItReadsModifiesThenWritesAnSln");
        test.Codebase?.FullName.Should().Be(@"C:\dev\github\cli\test\Microsoft.DotNet.Cli.Sln.Internal.Tests\bin\Debug\netcoreapp1.0\Microsoft.DotNet.Cli.Sln.Internal.Tests.dll");
    }

    [Test, NonWindowsOnly]
    public void Parse_Example1OSXTrx_ParsesTestProjectDirectoryCorrectly()
    {
        var results = TrxParser.Parse(new FileInfo(GetSampleFilePath(Path.Combine("1", "example1_OSX.trx"))));
        var test = results.Single(r => r.FullyQualifiedTestName == "Microsoft.DotNet.Cli.Utils.Tests.GivenARootedCommandResolver.It_escapes_CommandArguments_when_returning_a_CommandSpec");
        test.TestProjectDirectory?.FullName.Should().Be(@"/Users/josequ/dev/cli/test/Microsoft.DotNet.Cli.Utils.Tests/");
    }

    private static bool IsWindows()
    {
        return RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
    }

    private static bool NotWindows()
    {
        return !RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
    }

    [Test]
    public void Parse_TheoryTestsTrx_AppendsSuffixToFqtnForParameterizedTests()
    {
        var results = TrxParser.Parse(new FileInfo(GetSampleFilePath("theory-tests.trx")));
        results.Select(r => r.FullyQualifiedTestName)
               .Should()
               .Contain("Acme.Tests.MathTests.AddNumbers(left: 1, right: 2)");
        results.Select(r => r.FullyQualifiedTestName)
               .Should()
               .Contain("Acme.Tests.MathTests.AddNumbers(left: 0, right: 0)");
    }

    [Test]
    public void Parse_TheoryTestsTrx_DoesNotAppendSuffixForNonParameterizedTest()
    {
        var results = TrxParser.Parse(new FileInfo(GetSampleFilePath("theory-tests.trx")));
        results.Single(r => r.FullyQualifiedTestName == "Acme.Tests.MathTests.PlainTest")
               .Should().NotBeNull();
    }

    [Test]
    public void Parse_TheoryTestsTrx_ParsesAllThreeTestResults()
    {
        var results = TrxParser.Parse(new FileInfo(GetSampleFilePath("theory-tests.trx")));
        results.Should().HaveCount(3);
        results.Count(r => r.Outcome == TestOutcome.Passed).Should().Be(3);
    }

    [Test, NonWindowsOnly]
    public void Parse_Example1OSXTrx_ParsesCodebaseCorrectly()
    {
        var results = TrxParser.Parse(new FileInfo(GetSampleFilePath(Path.Combine("1", "example1_OSX.trx"))));
        var test = results.Single(r => r.FullyQualifiedTestName == "Microsoft.DotNet.Cli.Utils.Tests.GivenARootedCommandResolver.It_escapes_CommandArguments_when_returning_a_CommandSpec");
        test.Codebase?.FullName.Should().Be(@"/Users/josequ/dev/cli/test/Microsoft.DotNet.Cli.Utils.Tests/bin/Debug/netcoreapp1.0/Microsoft.DotNet.Cli.Utils.Tests.dll");
    }

    // ── Parser regression tests ──────────────────────────────────────────────────
    // These cover attributes/elements present in sample TRX files that were
    // previously silently discarded. All tests below should be GREEN.

    [Test]
    public void Parse_OneTestFailureTrx_PopulatesTestRunId()
    {
        // TestResultSet.TestRunId is never assigned; parser must set it from TestRun.Id.
        var results = TrxParser.Parse(new FileInfo(GetSampleFilePath("OneTestFailure.trx")));
        results.TestRunId.Should().Be("a1293f2f-ba2d-4aa4-91a7-ceed97fd4735");
    }

    [Test]
    public void Parse_OneTestFailureTrx_ParsesRunUser()
    {
        // runUser attribute on <TestRun> is present in every TRX file but currently not parsed.
        var results = TrxParser.Parse(new FileInfo(GetSampleFilePath("OneTestFailure.trx")));
        results.OriginalTestRun.Should().NotBeNull();
        results.OriginalTestRun!.RunUser.Should().Be("BenjaminMichaelis");
    }

    [Test]
    public void Parse_OneTestFailureTrx_ParsesResultSummaryOutcome()
    {
        // <ResultSummary outcome="..."> is present in every TRX file but currently not parsed.
        var results = TrxParser.Parse(new FileInfo(GetSampleFilePath("OneTestFailure.trx")));
        results.OriginalTestRun.Should().NotBeNull();
        results.OriginalTestRun!.ResultSummary.Should().NotBeNull();
        results.OriginalTestRun!.ResultSummary!.Outcome.Should().Be("Failed");
    }

    [Test]
    public void Parse_OneTestFailureTrx_ParsesCountersTotals()
    {
        // <Counters total="19" passed="18" failed="1" ...> inside <ResultSummary> but currently not parsed.
        var results = TrxParser.Parse(new FileInfo(GetSampleFilePath("OneTestFailure.trx")));
        results.OriginalTestRun.Should().NotBeNull();
        var counters = results.OriginalTestRun!.ResultSummary?.Counters;
        counters.Should().NotBeNull();
        using var _ = new AssertionScope();
        counters!.Total.Should().Be(19);
        counters.Passed.Should().Be(18);
        counters.Failed.Should().Be(1);
        counters.Executed.Should().Be(19);
    }

    [Test]
    public void Parse_OneTestFailureTrx_ParsesTestLists()
    {
        // <TestLists> always has two default entries but currently not parsed.
        var results = TrxParser.Parse(new FileInfo(GetSampleFilePath("OneTestFailure.trx")));
        results.OriginalTestRun.Should().NotBeNull();
        var testLists = results.OriginalTestRun!.TestLists?.Items;
        testLists.Should().NotBeNull();
        testLists.Should().HaveCount(2);
        testLists.Should().ContainSingle(l => l.Name == "Results Not in a List" && l.Id == "8c84fa94-04c1-424b-9868-57a2d4851a1d");
        testLists.Should().ContainSingle(l => l.Name == "All Loaded Results"     && l.Id == "19431567-8539-422a-85d7-44ee4e166bda");
    }

    [Test]
    public void Parse_OneTestFailureTrx_ParsesTestEntries()
    {
        // <TestEntries> is present in every TRX file but currently not parsed.
        var results = TrxParser.Parse(new FileInfo(GetSampleFilePath("OneTestFailure.trx")));
        results.OriginalTestRun.Should().NotBeNull();
        var entries = results.OriginalTestRun!.TestEntries?.Items;
        entries.Should().NotBeNull();
        entries.Should().HaveCount(19);
        // Spot-check one entry links testId -> executionId -> testListId correctly.
        entries.Should().ContainSingle(e =>
            e.TestId       == "35e9c03c-7c18-2ee8-7216-91e69cfe406e" &&
            e.ExecutionId  == "9682c228-090a-47f3-96d2-26ffa23c9a53" &&
            e.TestListId   == "8c84fa94-04c1-424b-9868-57a2d4851a1d");
    }

    [Test]
    public void Parse_OneTestFailureTrx_ParsesUnitTestResultExecutionId()
    {
        // executionId attribute on <UnitTestResult> is present in every TRX but currently not parsed.
        var results = TrxParser.Parse(new FileInfo(GetSampleFilePath("OneTestFailure.trx")));
        results.OriginalTestRun.Should().NotBeNull();
        var rawResults = results.OriginalTestRun!.Results?.UnitTestResults;
        rawResults.Should().NotBeNull();
        rawResults.Should().ContainSingle(r => r.ExecutionId == "9682c228-090a-47f3-96d2-26ffa23c9a53");
    }

    [Test]
    public void Parse_OneTestFailureTrx_ParsesUnitTestDefinitionAttributes()
    {
        // Both storage and <Execution id="..."/> are on <UnitTest> but currently not parsed.
        var results = TrxParser.Parse(new FileInfo(GetSampleFilePath("OneTestFailure.trx")));
        var unitTests = results.OriginalTestRun?.TestDefinitions?.UnitTests;
        unitTests.Should().NotBeNull();
        using var _ = new AssertionScope();
        unitTests.Should().Contain(u => u.Storage == @"essentialcsharp\src\chapter01.tests\bin\debug\net6.0\chapter01.tests.dll");
        unitTests.Should().Contain(u => u.Execution != null && u.Execution.Id == "86ff1fe0-ef46-4281-b70a-7ecc3ed2376b");
    }

    [Test]
    public void Parse_ComplexTrx_FullyQualifiedTestNameIncludesTheoryParameters()
    {
        // The parser builds FQTN from ClassName.Name, discarding the (params) suffix from testName.
        // For theory tests every invocation gets the same FQTN, making distinct runs indistinguishable.
        var results = TrxParser.Parse(new FileInfo(GetSampleFilePath("complex.trx")));
        const string expectedFqtn =
            "System.CommandLine.Tests.ParserTests+MultiplePositions" +
            ".When_an_option_is_shared_between_an_outer_and_inner_command_then_specifying_in_one_does_not_result_in_error_on_other" +
            "(commandLine: \"outer --the-option xyz inner\")";
        results.Select(r => r.FullyQualifiedTestName).Should().Contain(expectedFqtn);
    }

    [Test]
    public void Parse_Example2WindowsTrx_TestsDoNotAppearWithMoreThanOneOutcome()
    {
        var results = TrxParser.Parse(new FileInfo(GetSampleFilePath(Path.Combine("2", "example2_Windows.trx"))));
        var recombinedCount = results.Count(r => r.Outcome == TestOutcome.Passed)
            + results.Count(r => r.Outcome == TestOutcome.Failed)
            + results.Count(r => r.Outcome == TestOutcome.NotExecuted);
        results.Count.Should().Be(recombinedCount);
    }

    // ── Regression tests ─────────────────────────────────────────────────────────

    [Test]
    public void Parse_AbortedRunTrx_ParsesRealAbortedRunFixture()
    {
        var results = TrxParser.Parse(new FileInfo(GetSampleFilePath("aborted-outcome.trx")));
        results.Should().HaveCount(1);
        results.Single().Outcome.Should().Be(TestOutcome.Passed);
        results.OriginalTestRun?.ResultSummary?.Outcome.Should().Be("Failed");
    }

    // Real NUnit TRX captured without xmlns on the root element, sourced from:
    // https://github.com/joaoopereira/dotnet-test-rerun/blob/main/test/dotnet-test-rerun.UnitTests/Fixtures/RerunCommand/NUnitTrxFileWithOneFailedTest.trx
    [Test]
    public void Parse_NoNamespaceTrx_ParsesResultsWithoutNamespace()
    {
        var results = TrxParser.Parse(new FileInfo(GetSampleFilePath("no-namespace.trx")));
        results.Should().HaveCount(5,
            "the parser must fall back to namespace-agnostic element matching when xmlns is absent");
    }

    [Test]
    public void Parse_ComplexTrx_ParsesTestRunNameCorrectly()
    {
        var results = TrxParser.Parse(new FileInfo(GetSampleFilePath("complex.trx")));
        results.TestRunName.Should().Be("josequ@JOSEQU10 2022-06-13 12:30:01");
    }

    [Test]
    public void Parse_Example1OSXTrx_ParsesTestRunIdCorrectly()
    {
        var results = TrxParser.Parse(new FileInfo(GetSampleFilePath(Path.Combine("1", "example1_OSX.trx"))));
        results.OriginalTestRun.Should().NotBeNull();
        results.OriginalTestRun?.Id.Should().Be("c6378a10-566e-4745-8f62-ed85e8dc7147");
    }

    [Test]
    public void Parse_Example1OSXTrx_ParsesDeploymentRootCorrectly()
    {
        var results = TrxParser.Parse(new FileInfo(GetSampleFilePath(Path.Combine("1", "example1_OSX.trx"))));
        results.OriginalTestRun.Should().NotBeNull();
        results.OriginalTestRun?.TestSettings?.Deployment?.RunDeploymentRoot.Should().Be("_josequMac 2017-01-17 10:39:30");
    }

    [Test]
    public void Parse_Example1OSXTrx_ParsesTestSettingsNameCorrectly()
    {
        var results = TrxParser.Parse(new FileInfo(GetSampleFilePath(Path.Combine("1", "example1_OSX.trx"))));
        results.OriginalTestRun.Should().NotBeNull();
        results.OriginalTestRun?.TestSettings?.Name.Should().Be("default");
    }

    [Test]
    public void Parse_Example1OSXTrx_ParsesCreationTimeCorrectly()
    {
        var results = TrxParser.Parse(new FileInfo(GetSampleFilePath(Path.Combine("1", "example1_OSX.trx"))));
        results.CreatedTime.Should().Be(DateTimeOffset.Parse("2017-01-17T10:39:30.9542170-08:00"));
    }

    [Test]
    public void Parse_Example1OSXTrx_ParsesQueueingTimeCorrectly()
    {
        var results = TrxParser.Parse(new FileInfo(GetSampleFilePath(Path.Combine("1", "example1_OSX.trx"))));
        results.QueuedTime.Should().Be(DateTimeOffset.Parse("2017-01-17T10:39:30.9542630-08:00"));
    }

    [Test]
    public void Parse_Example1OSXTrx_ParsesStartTimeCorrectly()
    {
        var results = TrxParser.Parse(new FileInfo(GetSampleFilePath(Path.Combine("1", "example1_OSX.trx"))));
        results.StartedTime.Should().Be(DateTimeOffset.Parse("2017-01-17T10:39:27.7784340-08:00"));
    }

    [Test]
    public void Parse_Example1OSXTrx_ParsesFinishTimeCorrectly()
    {
        var results = TrxParser.Parse(new FileInfo(GetSampleFilePath(Path.Combine("1", "example1_OSX.trx"))));
        results.CompletedTime.Should().Be(DateTimeOffset.Parse("2017-01-17T10:39:57.1294340-08:00"));
    }
}
