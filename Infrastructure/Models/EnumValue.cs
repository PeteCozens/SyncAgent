using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Infrastructure.Models
{
    [Table("Enums")]
    internal class EnumValue
    {
        // Compound Primary Key defined in AppDbContext.ConfigureCompoundKeys()

        [StringLength(128)]
        public string SchemaName { get; set; } = string.Empty;
        [StringLength(128)]
        public string TableName { get; set; } = string.Empty;
        [StringLength(128)]
        public string ColumnName { get; set; } = string.Empty;
        public int Value { get; set; }
        [StringLength(128)]
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}
