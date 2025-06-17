using Xunit.Sdk;

namespace Xunit;

[XunitTestCaseDiscoverer("Xunit.NetCore.Extensions.ConditionalFactDiscoverer", "Xunit.NetCore.Extensions")]
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public sealed class ConditionalFactAttribute : FactAttribute
{
    public string[] ConditionMemberNames { get; private set; }

    public ConditionalFactAttribute(params string[] conditionMemberNames)
    {
        ConditionMemberNames = conditionMemberNames;
    }
}
