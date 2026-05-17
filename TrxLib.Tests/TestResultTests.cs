using AwesomeAssertions;
using AwesomeAssertions.Execution;

namespace TrxLib.Tests;

public class TestResultTests
{
    [Test]
    [Arguments("namespace.class.test", "namespace")]
    [Arguments("deeper.namespace.class.test", "deeper.namespace")]
    [Arguments("still.deeper.namespace.class.test", "still.deeper.namespace")]
    public void Namespace_is_parsed_correctly(
        string fullyQualifiedTestName,
        string expectedNamespace)
    {
        var testResult = new TestResult(fullyQualifiedTestName, TestOutcome.NotExecuted);
        testResult.Namespace.Should().Be(expectedNamespace);
    }

    [Test]
    [Arguments("namespace.class.test")]
    [Arguments("deeper.namespace.class.test")]
    [Arguments("still.deeper.namespace.class.test")]
    public void TestName_is_parsed_correctly(
        string fullyQualifiedTestName)
    {
        var testResult = new TestResult(fullyQualifiedTestName, TestOutcome.NotExecuted);
        testResult.TestName.Should().Be("test");
    }

    [Test]
    [Arguments("namespace.class.test")]
    [Arguments("deeper.namespace.class.test")]
    [Arguments("still.deeper.namespace.class.test")]
    public void ClassName_is_parsed_correctly(
        string fullyQualifiedTestName)
    {
        var testResult = new TestResult(fullyQualifiedTestName, TestOutcome.NotExecuted);
        testResult.ClassName.Should().Be("class");
    }

    [Test]
    [Arguments("namespace.class.test", "namespace.class")]
    [Arguments("deeper.namespace.class.test", "deeper.namespace.class")]
    [Arguments("still.deeper.namespace.class.test", "still.deeper.namespace.class")]
    [Arguments("deeper.namespace.class.theorytest(command: \"build\")", "deeper.namespace.class")]
    public void FullyQualifiedClassName_is_parsed_correctly(
        string fullyQualifiedTestName,
        string expected)
    {
        var testResult = new TestResult(fullyQualifiedTestName, TestOutcome.NotExecuted);
        testResult.FullyQualifiedClassName.Should().Be(expected);
    }

    [Test]
    public void Theory_test_is_parsed_correctly()
    {
        var testResult = new TestResult(
            "Microsoft.DotNet.Cli.MSBuild.IntegrationTests.GivenDotnetInvokesMSBuild.When_dotnet_command_invokes_msbuild_Then_env_vars_and_m_are_passed(command: \"build\")",
            outcome: TestOutcome.Passed);

        using var _ = new AssertionScope();
        testResult.TestName.Should().Be("When_dotnet_command_invokes_msbuild_Then_env_vars_and_m_are_passed(command: \"build\")");
        testResult.Namespace.Should().Be("Microsoft.DotNet.Cli.MSBuild.IntegrationTests");
        testResult.ClassName.Should().Be("GivenDotnetInvokesMSBuild");
    }

    [Test]
    [Arguments("Cell 1: #r \"nuget:TRexLib\"")]
    [Arguments("Cell 1: Console.Write(\"Hello world.\";")]
    public void Inferred_properties_are_not_inferred_from_fully_qualified_test_name_if_they_do_not_match_dotnet_standards(
        string fullyQualifiedTestName)
    {
        var testResult = new TestResult(fullyQualifiedTestName, TestOutcome.NotExecuted);
        testResult.ClassName.Should().BeNull();
        testResult.FullyQualifiedClassName.Should().BeNull();
        testResult.Namespace.Should().BeNull();
        testResult.TestName.Should().Be(fullyQualifiedTestName);
    }

    [Test]
    public void Theory_test_with_dotted_param_is_parsed_correctly_when_testMethod_provided()
    {
        // When testMethod is supplied the constructor must use testMethod.ClassName
        // directly instead of splitting the FQTN on '.'.  Without the fix, a param
        // value like "foo.bar" causes ClassName and TestName to be corrupt.
        const string className = "System.CommandLine.Tests.ParserTests";
        const string methodName = "Parse_theory_method";
        const string fqtn = $"{className}.{methodName}(param: \"foo.bar\")";

        var testResult = new TestResult(fqtn, TestOutcome.Passed,
            testMethod: new TestMethod { ClassName = className, Name = methodName });

        using var _ = new AssertionScope();
        testResult.FullyQualifiedTestName.Should().Be(fqtn);
        testResult.FullyQualifiedClassName.Should().Be(className);
        testResult.ClassName.Should().Be("ParserTests");
        testResult.Namespace.Should().Be("System.CommandLine.Tests");
        testResult.TestName.Should().Be($"{methodName}(param: \"foo.bar\")");
    }

    [Test]
    public void ToString_DoesNotThrow_ForOutcomeValueNotInEnum()
    {
        // TestResult.ToString() has a _ => throw arm that crashes on any enum value
        // not listed in its switch expression (e.g. future additions to TestOutcome, or
        // values written by vstest that TrxLib doesn't yet map, such as "Completed").
        var testResult = new TestResult("some.namespace.SomeClass.SomeTest", (TestOutcome)99);
        var act = () => testResult.ToString();
        act.Should().NotThrow<ArgumentOutOfRangeException>();
    }
}