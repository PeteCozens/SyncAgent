using System.ComponentModel;

namespace Common.Models
{
    public enum SomeEnum
    {
        None,
        [Description("Some Options")]
        Some,
        [Description("All Options")]
        All
    }
}
