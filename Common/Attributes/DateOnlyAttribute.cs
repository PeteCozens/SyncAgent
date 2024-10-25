using System.Diagnostics.CodeAnalysis;

namespace Common.Attributes
{
    [ExcludeFromCodeCoverage]
    [AttributeUsage(AttributeTargets.Property)]
    public class DateOnlyAttribute : Attribute
    {
    }
}
