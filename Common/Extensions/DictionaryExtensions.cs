namespace Common.Extensions
{
    public static class DictionaryExtensions
    {
        /// <summary>
        /// Gets a value from the dictionary. If the value is not stored in the dictionary, then this method will call the 
        /// method specified to fetch it from source, then add it to the dictionary before returning it. This allows the 
        /// dictionary to act like an object cache
        /// </summary>
        /// <typeparam name="K">Type of the dictionary's key</typeparam>
        /// <typeparam name="T">Type of the values stored in the dictionary</typeparam>
        /// <param name="dictionary"></param>
        /// <param name="key">Key value</param>
        /// <param name="getValue">Method to fetch the appropriate value for key, so that it can be added to the dictionary and returned</param>
        /// <returns></returns>
        public static T Get<K, T>(this Dictionary<K, T> dictionary, K key, Func<K, T> getValue)
            where T : class
            where K : notnull
        {
            if (!dictionary.ContainsKey(key))
                dictionary.Add(key, getValue.Invoke(key));

            return dictionary[key];
        }
    }
}
