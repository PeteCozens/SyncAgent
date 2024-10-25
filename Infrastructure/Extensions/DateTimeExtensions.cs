namespace Infrastructure.Extensions
{
    internal static class DateTimeExtensions
    {
        private static readonly DateTime _minDate = new(1973, 1, 1);

        public static DateTime SqlSafe(this DateTime value) => value < _minDate ? _minDate : value;
    }
}
