using System.Runtime.InteropServices;

using AwesomeAssertions;

namespace TrxLib.Tests;

public class TrxParserTests
{
    private static string GetSampleFilePath(string fileName)
    {
        return Path.Combine(
            IntelliTect.Multitool.RepositoryPaths.GetDefaultRepoRoot(),
            "TrxLib.Tests", "SampleTrxFiles", fileName);
    }

    [Fact]
    public void Parse_OneTestFailureTrx_ParsesClassNamesCorrectly()
    {
        var results = TrxParser.Parse(new FileInfo(GetSampleFilePath("OneTestFailure.trx")));
        results.Should().HaveCount(19);
        results.Select(r => r.TestMethod?.ClassName + "." + r.TestMethod?.Name ?? throw new InvalidOperationException("TestMethod is null"))
               .Should()
               .ContainEquivalentOf("AddisonWesley.Michaelis.EssentialCSharp.Chapter01.Listing01_03.Tests.HelloWorldTests.Main_UpDown");
    }

    [Fact]
    public void Parse_OneTestFailureTrx_ParsesTestNamesCorrectly()
    {
        var results = TrxParser.Parse(new FileInfo(GetSampleFilePath("OneTestFailure.trx")));
        results.Select(r => r.FullyQualifiedTestName)
               .Should()
               .Contain("AddisonWesley.Michaelis.EssentialCSharp.Chapter01.Listing01_03.Tests.HelloWorldTests.Main_UpDown");
    }

    [Fact]
    public void Parse_OneTestFailureTrx_CorrectlyParsesOutcomes()
    {
        var results = TrxParser.Parse(new FileInfo(GetSampleFilePath("OneTestFailure.trx")));
        // 19 tests in the file, 18 passed, 1 failed
        results.Should().HaveCount(19);
        results.Count(r => r.Outcome == TestOutcome.Passed).Should().Be(18);
        results.Count(r => r.Outcome == TestOutcome.Failed).Should().Be(1);
    }

    [Fact]
    public void Parse_OneTestFailureTrx_ParsesErrorMessageAndStackTraceCorrectly()
    {
        var results = TrxParser.Parse(new FileInfo(GetSampleFilePath("OneTestFailure.trx")));
        var errorTest = results.FirstOrDefault(t => t.ErrorMessage?.Contains("System.Exception") == true);
        errorTest.Should().NotBeNull();

        errorTest.ErrorMessage.Should().Contain("The expected length of 15 does not match the output length of 8.");

        errorTest.StackTrace.Should().Contain("IntelliTect.TestTools.Console.ConsoleAssert.AssertExpectation(String expectedOutput, String output, Func`3 areEquivalentOperator, String equivalentOperatorErrorMessage)");
        errorTest.StackTrace.Should().Contain("AddisonWesley.Michaelis.EssentialCSharp.Chapter01.Listing01_03.Tests.HelloWorldTests.Main_UpDown()");
    }

    [Fact]
    public void Parse_Example1OSXTrx_ParsesTestNamesCorrectly()
    {
        var results = TrxParser.Parse(new FileInfo(GetSampleFilePath(Path.Combine("1", "example1_OSX.trx"))));
        results.Select(r => r.FullyQualifiedTestName)
               .Should()
               .Contain("Microsoft.DotNet.Cli.Utils.Tests.GivenARootedCommandResolver.It_returns_a_CommandSpec_with_CommandName_as_Path_when_CommandName_is_rooted");
    }

    [Fact]
    public void Parse_Example1OSXTrx_ParsesOutcomesCorrectly()
    {
        var results = TrxParser.Parse(new FileInfo(GetSampleFilePath(Path.Combine("1", "example1_OSX.trx"))));
        // 88 tests in the file, 83 passed, 4 not executed, 1 failed
        results.Should().HaveCount(88);
        results.Count(r => r.Outcome == TestOutcome.Passed).Should().Be(83);
        results.Count(r => r.Outcome == TestOutcome.NotExecuted).Should().Be(4);
        results.Count(r => r.Outcome == TestOutcome.Failed).Should().Be(1);
    }

    [Fact]
    public void Parse_ComplexTrx_ParsesErrorMessageAndStackTraceCorrectly()
    {
        var results = TrxParser.Parse(new FileInfo(GetSampleFilePath("complex.trx")));
        var errorTest = results.FirstOrDefault(t => t.ErrorMessage?.Contains("Index (zero based)") == true);
        errorTest.Should().NotBeNull();
        errorTest.StackTrace.Should().Contain("System.Text.StringBuilder.AppendFormatHelper");
    }

    [Fact]
    public void Parse_ComplexTrx_ParsesStdOutCorrectly()
    {
        var results = TrxParser.Parse(new FileInfo(GetSampleFilePath("complex.trx")));
        var stdOutTest = results.FirstOrDefault(t => t.FullyQualifiedTestName == "System.CommandLine.Tests.ParserTests.Option_arguments_can_match_subcommands");
        stdOutTest.Should().NotBeNull();
        stdOutTest.StdOut.Should().Contain("ParseResult: ![ testhost.net462.x86");
    }

    [Fact]
    public void Parse_Example1WindowsTrx_ParsesDurationsCorrectly()
    {
        var results = TrxParser.Parse(new FileInfo(GetSampleFilePath(Path.Combine("example1_Windows.trx"))));
        var test = results.Single(r => r.FullyQualifiedTestName == "Microsoft.DotNet.Cli.Sln.Internal.Tests.GivenAnSlnFile.WhenGivenAValidPathItReadsAnSlnFile");
        test.Duration.Should().Be(TimeSpan.FromMilliseconds(138));
    }

    [Fact]
    public void Parse_Example1WindowsTrx_ParsesStartTimesCorrectly()
    {
        var results = TrxParser.Parse(new FileInfo(GetSampleFilePath(Path.Combine("example1_Windows.trx"))));
        var test = results.Single(r => r.FullyQualifiedTestName == "Microsoft.DotNet.Cli.Sln.Internal.Tests.GivenAnSlnFile.WhenGivenAValidPathItReadsAnSlnFile");
        test.StartTime.Should().Be(DateTimeOffset.Parse("2016-12-21T11:15:51.8308573-08:00"));
    }

    [Fact]
    public void Parse_Example1WindowsTrx_ParsesEndTimesCorrectly()
    {
        var results = TrxParser.Parse(new FileInfo(GetSampleFilePath(Path.Combine("example1_Windows.trx"))));
        var test = results.Single(r => r.FullyQualifiedTestName == "Microsoft.DotNet.Cli.Sln.Internal.Tests.GivenAnSlnFile.WhenGivenAValidPathItReadsAnSlnFile");
        test.EndTime.Should().Be(DateTimeOffset.Parse("2016-12-21T11:15:51.8308573-08:00"));
    }

    [Fact]
    public void Parse_Example1WindowsTrx_ParsesPassOutcomeCorrectly()
    {
        var results = TrxParser.Parse(new FileInfo(GetSampleFilePath(Path.Combine("example1_Windows.trx"))));
        var test = results.Single(r => r.FullyQualifiedTestName == "Microsoft.DotNet.Cli.Sln.Internal.Tests.GivenAnSlnFile.WhenGivenAValidPathItReadsAnSlnFile");
        test.Outcome.Should().Be(TestOutcome.Passed);
    }

    [Fact]
    public void Parse_Example1WindowsTrx_ParsesFailOutcomeCorrectly()
    {
        var results = TrxParser.Parse(new FileInfo(GetSampleFilePath(Path.Combine("example1_Windows.trx"))));
        var test = results.Single(r => r.FullyQualifiedTestName == "Microsoft.DotNet.Cli.Sln.Internal.Tests.GivenAnSlnFile.WhenGivenAValidPathItReadsModifiesThenWritesAnSln");
        test.Outcome.Should().Be(TestOutcome.Failed);
    }

    [ConditionalFact(nameof(IsWindows))]
    public void Parse_Example1WindowsTrx_ParsesTestProjectDirectoryCorrectly()
    {
        var results = TrxParser.Parse(new FileInfo(GetSampleFilePath(Path.Combine("example1_Windows.trx"))));
        var test = results.Single(r => r.FullyQualifiedTestName == "Microsoft.DotNet.Cli.Sln.Internal.Tests.GivenAnSlnFile.WhenGivenAValidPathItReadsModifiesThenWritesAnSln");
        test.TestProjectDirectory?.FullName.Should().Be(@"C:\dev\github\cli\test\Microsoft.DotNet.Cli.Sln.Internal.Tests");
    }

    [ConditionalFact(nameof(IsWindows))]
    public void Parse_Example1WindowsTrx_ParsesCodebaseCorrectly()
    {
        var results = TrxParser.Parse(new FileInfo(GetSampleFilePath(Path.Combine("example1_Windows.trx"))));
        var test = results.Single(r => r.FullyQualifiedTestName == "Microsoft.DotNet.Cli.Sln.Internal.Tests.GivenAnSlnFile.WhenGivenAValidPathItReadsModifiesThenWritesAnSln");
        test.Codebase?.FullName.Should().Be(@"C:\dev\github\cli\test\Microsoft.DotNet.Cli.Sln.Internal.Tests\bin\Debug\netcoreapp1.0\Microsoft.DotNet.Cli.Sln.Internal.Tests.dll");
    }

    [ConditionalFact(nameof(NotWindows))]
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

    [ConditionalFact(nameof(NotWindows))]
    public void Parse_Example1OSXTrx_ParsesCodebaseCorrectly()
    {
        var results = TrxParser.Parse(new FileInfo(GetSampleFilePath(Path.Combine("1", "example1_OSX.trx"))));
        var test = results.Single(r => r.FullyQualifiedTestName == "Microsoft.DotNet.Cli.Utils.Tests.GivenARootedCommandResolver.It_escapes_CommandArguments_when_returning_a_CommandSpec");
        test.Codebase?.FullName.Should().Be(@"/Users/josequ/dev/cli/test/Microsoft.DotNet.Cli.Utils.Tests/bin/Debug/netcoreapp1.0/Microsoft.DotNet.Cli.Utils.Tests.dll");
    }

    [Fact]
    public void Parse_Example2WindowsTrx_TestsDoNotAppearWithMoreThanOneOutcome()
    {
        var results = TrxParser.Parse(new FileInfo(GetSampleFilePath(Path.Combine("2", "example2_Windows.trx"))));
        var recombinedCount = results.Count(r => r.Outcome == TestOutcome.Passed)
            + results.Count(r => r.Outcome == TestOutcome.Failed)
            + results.Count(r => r.Outcome == TestOutcome.NotExecuted);
        results.Count.Should().Be(recombinedCount);
    }

    [Fact]
    public void Parse_ComplexTrx_ParsesTestRunNameCorrectly()
    {
        var results = TrxParser.Parse(new FileInfo(GetSampleFilePath("complex.trx")));
        results.TestRunName.Should().Be("josequ@JOSEQU10 2022-06-13 12:30:01");
    }

    [Fact]
    public void Parse_Example1OSXTrx_ParsesTestRunIdCorrectly()
    {
        var results = TrxParser.Parse(new FileInfo(GetSampleFilePath(Path.Combine("1", "example1_OSX.trx"))));
        results.OriginalTestRun.Should().NotBeNull();
        results.OriginalTestRun?.Id.Should().Be("c6378a10-566e-4745-8f62-ed85e8dc7147");
    }

    [Fact]
    public void Parse_Example1OSXTrx_ParsesDeploymentRootCorrectly()
    {
        var results = TrxParser.Parse(new FileInfo(GetSampleFilePath(Path.Combine("1", "example1_OSX.trx"))));
        results.OriginalTestRun.Should().NotBeNull();
        results.OriginalTestRun?.TestSettings?.Deployment?.RunDeploymentRoot.Should().Be("_josequMac 2017-01-17 10:39:30");
    }

    [Fact]
    public void Parse_Example1OSXTrx_ParsesTestSettingsNameCorrectly()
    {
        var results = TrxParser.Parse(new FileInfo(GetSampleFilePath(Path.Combine("1", "example1_OSX.trx"))));
        results.OriginalTestRun.Should().NotBeNull();
        results.OriginalTestRun?.TestSettings?.Name.Should().Be("default");
    }

    [Fact]
    public void Parse_Example1OSXTrx_ParsesCreationTimeCorrectly()
    {
        var results = TrxParser.Parse(new FileInfo(GetSampleFilePath(Path.Combine("1", "example1_OSX.trx"))));
        results.CreatedTime.Should().Be(DateTimeOffset.Parse("2017-01-17T10:39:30.9542170-08:00"));
    }

    [Fact]
    public void Parse_Example1OSXTrx_ParsesQueueingTimeCorrectly()
    {
        var results = TrxParser.Parse(new FileInfo(GetSampleFilePath(Path.Combine("1", "example1_OSX.trx"))));
        results.QueuedTime.Should().Be(DateTimeOffset.Parse("2017-01-17T10:39:30.9542630-08:00"));
    }

    [Fact]
    public void Parse_Example1OSXTrx_ParsesStartTimeCorrectly()
    {
        var results = TrxParser.Parse(new FileInfo(GetSampleFilePath(Path.Combine("1", "example1_OSX.trx"))));
        results.StartedTime.Should().Be(DateTimeOffset.Parse("2017-01-17T10:39:27.7784340-08:00"));
    }

    [Fact]
    public void Parse_Example1OSXTrx_ParsesFinishTimeCorrectly()
    {
        var results = TrxParser.Parse(new FileInfo(GetSampleFilePath(Path.Combine("1", "example1_OSX.trx"))));
        results.CompletedTime.Should().Be(DateTimeOffset.Parse("2017-01-17T10:39:57.1294340-08:00"));
    }
}