using Common.Attributes;
using Common.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Common.Models
{
    public class FxRate : ITemporalTable, IDapperModel
    {
        // Compound Primary Key defined in AppDbContext.ConfigureCompoundKeys()

        public int CurrencyId { get; set; }
        public virtual Currency? Currency { get; set; }

        [Required]
        [DateOnly]
        public DateTime FromDate { get; set; }

        [Required]
        [DateOnly]
        public DateTime ToDate { get; set; }

        [Column(TypeName = "decimal(20,8)")]
        public decimal Rate { get; set; }

        public decimal Reciprocal => 1.0m / Rate;



        #region Temporal Table

        public string SysUpdatedByUser { get; set; } = string.Empty;
        public string SysUpdateHostMachine { get; set; } = string.Empty;
        public bool SysIsDeleted { get; set; }
        public byte[] SysRowVersion { get; set; } = [];

        #endregion
    }
}
