namespace Common.Interfaces
{
    /// <summary>
    /// Used to flag a class as being a Data Model for use with the Dapper ORM library. Any classes
    /// implementing this Interface will be interrogated for [Column] attributes to map properties to
    /// the appropriate columns when the Utility.MapColumnAttributesForDapper() method is called at
    /// application startup.
    /// </summary>
    public interface IDapperModel
    {
    }
}
