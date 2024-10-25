using System.Text.Json;

namespace Common.Extensions
{
    public static class GenericExtensions
    {
        /// <summary>
        /// Serializes the object specified to JSON
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="writeIndented"></param>
        /// <returns></returns>
        public static string ToJson<T>(this T value, bool writeIndented)
        {
            var options = new JsonSerializerOptions { WriteIndented = writeIndented };
            return value.ToJson(options);
        }

        public static string ToJson<T>(this T value, JsonSerializerOptions? options = null)
        {
            return JsonSerializer.Serialize(value, options);
        }

        /// <summary>
        /// Determines whether or not the object specified exists in the array provided
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="array"></param>
        /// <returns></returns>
        public static bool In<T>(this T value, params T[] array) where T : IComparable
        {
            return array.Contains(value);
        }

        /// <summary>
        /// Determines whether or not the object specified exists in the array provided
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="array"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public static bool In<T>(this T value, IEnumerable<T> array, IEqualityComparer<T>? comparer = null)
        {
            return array.Contains(value, comparer);
        }

        /// <summary>
        /// Determine if the value specified is between the two values provided
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="lowValue"></param>
        /// <param name="highValue"></param>
        /// <returns></returns>
        public static bool Between<T>(this T value, T lowValue, T highValue) where T : IComparable
        {
            if (lowValue.CompareTo(highValue) > 0)
                (lowValue, highValue) = (highValue, lowValue);    // low and high values are the wrong way around, so swap them

            return value.CompareTo(lowValue) >= 0 && value.CompareTo(highValue) <= 0;
        }
    }
}
