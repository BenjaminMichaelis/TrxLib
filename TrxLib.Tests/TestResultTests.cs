using TUnit.Assertions;

namespace TrxLib.Tests;

public class TestResultTests
{
    [Test]
    [Arguments("namespace.class.test", "namespace")]
    [Arguments("deeper.namespace.class.test", "deeper.namespace")]
    [Arguments("still.deeper.namespace.class.test", "still.deeper.namespace")]
    public async Task Namespace_is_parsed_correctly(
        string fullyQualifiedTestName,
        string expectedNamespace)
    {
        var testResult = new TestResult(fullyQualifiedTestName, TestOutcome.NotExecuted);
        await Assert.That(testResult.Namespace).IsEqualTo(expectedNamespace);
    }

    [Test]
    [Arguments("namespace.class.test")]
    [Arguments("deeper.namespace.class.test")]
    [Arguments("still.deeper.namespace.class.test")]
    public async Task TestName_is_parsed_correctly(
        string fullyQualifiedTestName)
    {
        var testResult = new TestResult(fullyQualifiedTestName, TestOutcome.NotExecuted);
        await Assert.That(testResult.TestName).IsEqualTo("test");
    }

    [Test]
    [Arguments("namespace.class.test")]
    [Arguments("deeper.namespace.class.test")]
    [Arguments("still.deeper.namespace.class.test")]
    public async Task ClassName_is_parsed_correctly(
        string fullyQualifiedTestName)
    {
        var testResult = new TestResult(fullyQualifiedTestName, TestOutcome.NotExecuted);
        await Assert.That(testResult.ClassName).IsEqualTo("class");
    }

    [Test]
    [Arguments("namespace.class.test", "namespace.class")]
    [Arguments("deeper.namespace.class.test", "deeper.namespace.class")]
    [Arguments("still.deeper.namespace.class.test", "still.deeper.namespace.class")]
    [Arguments("deeper.namespace.class.theorytest(command: \"build\")", "deeper.namespace.class")]
    public async Task FullyQualifiedClassName_is_parsed_correctly(
        string fullyQualifiedTestName,
        string expected)
    {
        var testResult = new TestResult(fullyQualifiedTestName, TestOutcome.NotExecuted);
        await Assert.That(testResult.FullyQualifiedClassName).IsEqualTo(expected);
    }

    [Test]
    public async Task Theory_test_is_parsed_correctly()
    {
        var testResult = new TestResult(
            "Microsoft.DotNet.Cli.MSBuild.IntegrationTests.GivenDotnetInvokesMSBuild.When_dotnet_command_invokes_msbuild_Then_env_vars_and_m_are_passed(command: \"build\")",
            outcome: TestOutcome.Passed);

        await Assert.That(testResult.TestName).IsEqualTo("When_dotnet_command_invokes_msbuild_Then_env_vars_and_m_are_passed(command: \"build\")");
        await Assert.That(testResult.Namespace).IsEqualTo("Microsoft.DotNet.Cli.MSBuild.IntegrationTests");
        await Assert.That(testResult.ClassName).IsEqualTo("GivenDotnetInvokesMSBuild");
    }

    [Test]
    [Arguments("Cell 1: #r \"nuget:TRexLib\"")]
    [Arguments("Cell 1: Console.Write(\"Hello world.\";")]
    public async Task Inferred_properties_are_not_inferred_from_fully_qualified_test_name_if_they_do_not_match_dotnet_standards(
        string fullyQualifiedTestName)
    {
        var testResult = new TestResult(fullyQualifiedTestName, TestOutcome.NotExecuted);
        await Assert.That(testResult.ClassName).IsNull();
        await Assert.That(testResult.FullyQualifiedClassName).IsNull();
        await Assert.That(testResult.Namespace).IsNull();
        await Assert.That(testResult.TestName).IsEqualTo(fullyQualifiedTestName);
    }

    [Test]
    public async Task Theory_test_with_dotted_param_is_parsed_correctly_when_testMethod_provided()
    {
        // When testMethod is supplied the constructor must use testMethod.ClassName
        // directly instead of splitting the FQTN on '.'.  Without the fix, a param
        // value like "foo.bar" causes ClassName and TestName to be corrupt.
        const string className = "System.CommandLine.Tests.ParserTests";
        const string methodName = "Parse_theory_method";
        const string fqtn = $"{className}.{methodName}(param: \"foo.bar\")";

        var testResult = new TestResult(fqtn, TestOutcome.Passed,
            testMethod: new TestMethod { ClassName = className, Name = methodName });

        await Assert.That(testResult.FullyQualifiedTestName).IsEqualTo(fqtn);
        await Assert.That(testResult.FullyQualifiedClassName).IsEqualTo(className);
        await Assert.That(testResult.ClassName).IsEqualTo("ParserTests");
        await Assert.That(testResult.Namespace).IsEqualTo("System.CommandLine.Tests");
        await Assert.That(testResult.TestName).IsEqualTo($"{methodName}(param: \"foo.bar\")");
    }

    [Test]
    public void ToString_DoesNotThrow_ForOutcomeValueNotInEnum()
    {
        // TestResult.ToString() has a _ => throw arm that crashes on any enum value
        // not listed in its switch expression (e.g. future additions to TestOutcome, or
        // values written by vstest that TrxLib doesn't yet map, such as "Completed").
        var testResult = new TestResult("some.namespace.SomeClass.SomeTest", (TestOutcome)99);
        _ = testResult.ToString();
    }
}
