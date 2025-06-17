using System.Reflection;

using Xunit.Abstractions;
using Xunit.Sdk;

namespace Xunit.NetCore.Extensions
{
    public class ConditionalFactDiscoverer : FactDiscoverer
    {
        private readonly IMessageSink _diagnosticMessageSink;

        public ConditionalFactDiscoverer(IMessageSink diagnosticMessageSink) : base(diagnosticMessageSink)
        {
            _diagnosticMessageSink = diagnosticMessageSink;
        }

        public override IEnumerable<IXunitTestCase> Discover(
            ITestFrameworkDiscoveryOptions discoveryOptions, ITestMethod testMethod, IAttributeInfo factAttribute)
        {
            var conditionMemberNames = (factAttribute.GetConstructorArguments().FirstOrDefault() as string[]) ?? Array.Empty<string>();
            var testCases = base.Discover(discoveryOptions, testMethod, factAttribute);
            return ConditionalTestDiscoverer.Discover(discoveryOptions, _diagnosticMessageSink, testMethod, testCases, conditionMemberNames);
        }
    }


    internal class ConditionalTestDiscoverer
    {
        // This helper method evaluates the given condition member names for a given set of test cases.
        // If any condition member evaluates to 'false', the test cases are marked to be skipped.
        // The skip reason is the collection of all the condition members that evalated to 'false'.
        internal static IEnumerable<IXunitTestCase> Discover(
                                                        ITestFrameworkDiscoveryOptions discoveryOptions,
                                                        IMessageSink diagnosticMessageSink,
                                                        ITestMethod testMethod,
                                                        IEnumerable<IXunitTestCase> testCases,
                                                        IEnumerable<string> conditionMemberNames)
        {
            // A null or empty list of conditionMemberNames is treated as "no conditions".
            // and the test cases will not be skipped.
            // Example: [ConditionalFact()] or [ConditionalFact((string[]) null)]
            int conditionCount = conditionMemberNames == null ? 0 : conditionMemberNames.Count();
            if (conditionCount == 0)
            {
                return testCases;
            }

            MethodInfo testMethodInfo = testMethod.Method.ToRuntimeMethod();
            Type? testMethodDeclaringType = testMethodInfo.DeclaringType;
            if (testMethodDeclaringType == null)
            {
                return testCases;
            }

            List<string> falseConditions = new(conditionCount);

            if (conditionMemberNames is null)
            {
                // If conditionMemberNames is null, we treat it as an empty list.
                // This is to handle cases where the attribute is used without any conditions.
                return testCases;
            }

            foreach (string entry in conditionMemberNames)
            {
                string conditionMemberName = entry;

                // Null condition member names are silently tolerated
                if (string.IsNullOrWhiteSpace(conditionMemberName))
                {
                    continue;
                }

                string[] symbols = conditionMemberName.Split('.');
                Type declaringType = testMethodDeclaringType;

                if (symbols.Length == 2)
                {
                    conditionMemberName = symbols[1];
                    ITypeInfo? type = testMethod.TestClass.Class.Assembly.GetTypes(false).Where(t => t.Name.Contains(symbols[0])).FirstOrDefault();
                    if (type != null)
                    {
                        declaringType = type.ToRuntimeType();
                    }
                }

                MethodInfo? conditionMethodInfo;
                if ((conditionMethodInfo = LookupConditionalMethod(declaringType, conditionMemberName)) == null)
                {
                    return
                    [
                        new ExecutionErrorTestCase(
                            diagnosticMessageSink,
                            discoveryOptions.MethodDisplayOrDefault(),
                            discoveryOptions.MethodDisplayOptionsOrDefault(),
                            testMethod,
                            GetFailedLookupString(conditionMemberName))
                    ];
                }

                // In the case of multiple conditions, collect the results of all
                // of them to produce a summary skip reason.
                try
                {
                    var result = conditionMethodInfo.Invoke(null, null);
                    if (result is not bool || !(bool)result)
                    {
                        falseConditions.Add(conditionMemberName);
                    }
                }
                catch (Exception exc)
                {
                    falseConditions.Add($"{conditionMemberName} ({exc.GetType().Name})");
                }
            }

            // Compose a summary of all conditions that returned false.
            if (falseConditions.Count > 0)
            {
                string skippedReason = string.Format("Condition(s) not met: \"{0}\"", string.Join("\", \"", falseConditions));
                return testCases.Select(tc => new SkippedTestCase(tc, skippedReason));
            }

            // No conditions returned false (including the absence of any conditions).
            return testCases;
        }

        internal static string GetFailedLookupString(string name)
        {
            return
                "An appropriate member \"" + name + "\" could not be found. " +
                "The conditional method needs to be a static method or property on this or any ancestor type, " +
                "of any visibility, accepting zero arguments, and having a return type of Boolean.";
        }

        internal static MethodInfo? LookupConditionalMethod(Type? t, string? name)
        {
            if (t == null || name == null)
                return null;

            TypeInfo ti = t.GetTypeInfo();

            MethodInfo? mi = ti.GetDeclaredMethod(name);
            if (mi != null && mi.IsStatic && mi.GetParameters().Length == 0 && mi.ReturnType == typeof(bool))
                return mi;

            PropertyInfo? pi = ti.GetDeclaredProperty(name);
            if (pi != null && pi.PropertyType == typeof(bool) && pi.GetMethod != null && pi.GetMethod.IsStatic && pi.GetMethod.GetParameters().Length == 0)
                return pi.GetMethod;

            return LookupConditionalMethod(ti.BaseType, name);
        }
    }
}

/// <summary>Wraps another test case that should be skipped.</summary>
internal sealed class SkippedTestCase : LongLivedMarshalByRefObject, IXunitTestCase
{
    [Obsolete("Called by the de-serializer; should only be called by deriving classes for de-serialization purposes")]
    public SkippedTestCase()
    {
        _testCase = null!;
        _skippedReason = string.Empty;
    }

    private readonly IXunitTestCase _testCase;
    private readonly string _skippedReason;

    internal SkippedTestCase(IXunitTestCase testCase, string skippedReason)
    {
        _testCase = testCase ?? throw new ArgumentNullException(nameof(testCase));
        _skippedReason = skippedReason ?? throw new ArgumentNullException(nameof(skippedReason));
    }

    public string DisplayName => _testCase.DisplayName;

    public IMethodInfo Method => _testCase.Method;

    public string SkipReason => _skippedReason;

    public ISourceInformation SourceInformation { get => _testCase.SourceInformation; set => _testCase.SourceInformation = value; }

    public ITestMethod TestMethod => _testCase.TestMethod;

    public object[] TestMethodArguments => _testCase.TestMethodArguments;

    public Dictionary<string, List<string>> Traits => _testCase.Traits;

    public string UniqueID => _testCase.UniqueID;

    public Exception InitializationException => throw new NotImplementedException();

    public int Timeout => throw new NotImplementedException();

    public void Deserialize(IXunitSerializationInfo info) { _testCase.Deserialize(info); }

    public Task<RunSummary> RunAsync(
        IMessageSink diagnosticMessageSink, IMessageBus messageBus, object[] constructorArguments,
        ExceptionAggregator aggregator, CancellationTokenSource cancellationTokenSource)
    {
        return new XunitTestCaseRunner(this, DisplayName, _skippedReason, constructorArguments, TestMethodArguments, messageBus, aggregator, cancellationTokenSource).RunAsync();
    }

    public void Serialize(IXunitSerializationInfo info) { _testCase.Serialize(info); }
}
