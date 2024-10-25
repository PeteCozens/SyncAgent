namespace Common.Extensions
{
    public static class DateTimeExtensions
    {
        /// <summary>
        /// Calculates the first calendar day of the month specified
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static DateTime StartOfMonth(this DateTime value)
        {
            return new DateTime(value.Year, value.Month, 1);
        }

        /// <summary>
        /// Calculates the last calendar day of the month specified
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static DateTime EndOfMonth(this DateTime value)
        {
            return value.StartOfMonth().AddMonths(1).AddDays(-1);
        }
    }
}
