namespace Common.Interfaces
{
    public interface ITemporalTable
    {
        string SysUpdatedByUser { get; set; }

        string SysUpdateHostMachine { get; set; }

        bool SysIsDeleted { get; set; }

        byte[] SysRowVersion { get; set; }
    }
}
