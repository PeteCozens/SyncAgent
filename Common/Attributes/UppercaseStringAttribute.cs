using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Common.Attributes
{
    [ExcludeFromCodeCoverage]
    [AttributeUsage(AttributeTargets.Property)]
    public class UppercaseStringAttribute : DisplayFormatAttribute
    {
        public UppercaseStringAttribute() : base()
        {
            DataFormatString = "{0:uppercase}";
            ApplyFormatInEditMode = true;
        }
    }
}
