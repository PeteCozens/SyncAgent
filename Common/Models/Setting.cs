using System.ComponentModel.DataAnnotations;

namespace Common.Models
{
    public class Setting
    {
        [Key]
        [StringLength(100)]
        public required string Key { get; set; }

        [StringLength(100)]
        public string Value { get; set; } = string.Empty;
    }
}
