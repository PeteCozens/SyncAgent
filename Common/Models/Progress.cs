using System.ComponentModel.DataAnnotations;

namespace Common.Models
{
    public class Progress
    {
        [StringLength(100)]
        public required string SyncSet { get; set; }

        [StringLength(100)]
        public required string Field { get; set; }

        [StringLength(100)]
        public required string Type { get; set; }

        [StringLength(100)]
        public string? Value { get; set; }
    }
}
