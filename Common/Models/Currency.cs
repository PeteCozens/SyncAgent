using Common.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace Common.Models
{
    public class Currency : ITemporalTable
    {
        [Key]
        public int CurrencyId { get; set; }

        [StringLength(10)]
        public string Code { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Description { get; set; } = string.Empty;

        [StringLength(10)]
        public string Symbol { get; set; } = string.Empty;

        [Required]
        public int Decimals { get; set; }

        [Required]
        public int MinorUnits { get; set; }

        [StringLength(100)]
        public string MajorSingle { get; set; } = string.Empty;

        [StringLength(100)]
        public string MajorPlural { get; set; } = string.Empty;

        [StringLength(100)]
        public string MinorSingle { get; set; } = string.Empty;

        [StringLength(100)]
        public string MinorPlural { get; set; } = string.Empty;



        #region Temporal Table

        public string SysUpdatedByUser { get; set; } = string.Empty;
        public string SysUpdateHostMachine { get; set; } = string.Empty;
        public bool SysIsDeleted { get; set; }
        public byte[] SysRowVersion { get; set; } = [];

        #endregion
    }
}
