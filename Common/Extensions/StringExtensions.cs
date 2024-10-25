using System.Collections.ObjectModel;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Text;

namespace Common.Extensions
{
    public static class StringExtensions
    {
        public static string Left(this string value, int length)
        {
            if (length < 0)
                throw new ArgumentOutOfRangeException(nameof(length), "Length should be zero or greater.");

            if (string.IsNullOrEmpty(value))
                return value;

            return length >= value.Length ? value : value[..length];
        }

        public static string Mid(this string value, int startIndex, int length)
        {
            if (startIndex < 0)
                throw new ArgumentOutOfRangeException(nameof(length), "StartIndex should be zero or greater.");

            if (length < 0)
                throw new ArgumentOutOfRangeException(nameof(length), "Length should be zero or greater.");

            if (string.IsNullOrEmpty(value))
                return value;

            if ((length == 0) || (startIndex >= value.Length))
                return string.Empty;

            if (startIndex + length >= value.Length)
                return value[startIndex..];

            return value.Substring(startIndex, length);
        }

        public static string Right(this string value, int length)
        {
            if (length < 0)
                throw new ArgumentOutOfRangeException(nameof(length), "The length provided may not be less than Zero");

            if (string.IsNullOrEmpty(value))
                return value;

            return length >= value.Length ? value : value[^length..];
        }

        /// <summary>
        /// Removes any "invalid" characters from the string
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string RemoveNonAlphaNumerics(this string value)
        {
#pragma warning disable SYSLIB1045 // Convert to 'GeneratedRegexAttribute'.
            return Regex.Replace(value, "[^A-Za-z0-9.]", "");
#pragma warning restore SYSLIB1045 // Convert to 'GeneratedRegexAttribute'.
        }

        /// <summary>
        /// Attempts to convert the string to an integer, returning the default value if it fails
        /// </summary>
        /// <param name="value"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static int ToInt(this string value, int defaultValue = 0)
        {
            return int.TryParse(value, out var i) ? i : defaultValue;
        }

        /// <summary>
        /// Converts a string for use as a human-readable parameter in a url
        /// </summary>
        /// <param name="value"></param>
        /// <param name="forceLowerCase"></param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "SYSLIB1045:Convert to 'GeneratedRegexAttribute'.", Justification = "<Pending>")]
        public static string ToSlug(this string value, bool forceLowerCase = false)
        {
            // Remove any extraneous white space
            value = value.Trim();
            // Replace accented characters with Latin equivalents
            value = value.Normalize(NormalizationForm.FormD);
            // Remove invalid characters
            value = Regex.Replace(value, @"[^a-zA-Z0-9\s-_]", "");
            // Convert multiple spaces into one space   
            value = Regex.Replace(value, @"\s+", " ").Trim();
            // Replace spaces with underscores
            value = Regex.Replace(value, @"\s", "-");
            // Lowercase if required
            if (forceLowerCase)
                value = value.ToLowerInvariant();
            return value;
        }

        /// <summary>
        /// Splits a delimited string using the specified delimiter
        /// </summary>
        public static string[] SplitEx(this string value, string separator,
            StringSplitOptions options = StringSplitOptions.RemoveEmptyEntries)
        {
            return value.SplitEx(new string[] { separator }, options);
        }

        /// <summary>
        /// Splits a delimited string using multiple delimiters
        /// </summary>
        /// <param name="value"></param>
        /// <param name="separators"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static string[] SplitEx(this string value, string[] separators,
            StringSplitOptions options = StringSplitOptions.RemoveEmptyEntries)
        {
            var items = value?
                .Split(separators, options)
                .Select(x => x.Trim());

            if (items == null || !items.Any())
                return [];

            if (options == StringSplitOptions.RemoveEmptyEntries)
                items = items.Where(t => !String.IsNullOrEmpty(t));

            return items.ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        /// <returns></returns>
        public static T? ParseJson<T>(this string json)
        {
            return JsonSerializer.Deserialize<T>(json, DefaultJsonParserOptions) ?? default;
        }

        private static readonly JsonSerializerOptions DefaultJsonParserOptions = new()
        {
            PropertyNameCaseInsensitive = true,
            NumberHandling = JsonNumberHandling.AllowReadingFromString,
            ReadCommentHandling = JsonCommentHandling.Skip,
        };

        public static string InsertSpacesBeforeCaptials(this string value)
        {
            if (string.IsNullOrEmpty(value))
                return value;

            value = value.Replace('_', ' ');
            var result = new StringBuilder();

            result.Append(value[0]);

            for (int i = 1; i < value.Length; i++)
            {
                if (char.IsUpper(value[i]))
                {
                    if (value[i - 1] != ' ' && (!char.IsUpper(value[i - 1]) || (i < value.Length - 1 && char.IsLower(value[i + 1]))))
                    {
                        result.Append(' ');
                    }
                }
                result.Append(value[i]);
            }

            return result.ToString();
        }

        public static string LastWord(this string value)
        {
            var i = value.LastIndexOf(' ');
            return i >= 0 ? value[(i + 1)..] : value;
        }

        public static string ReplacePlaceholders(this string template, Dictionary<string, object> parameters)
        {
            return Regex.Replace(template, @"\{(\w+)\}", match =>
            {
                string key = match.Groups[1].Value;
                if (parameters.ContainsKey(key))
                {
                    var value = parameters[key];
                    if (value is DateTime dateTimeValue)
                        return dateTimeValue.ToString("yyyy-MM-dd");

                    return value?.ToString() ?? string.Empty;
                }
                return match.Value; // return the placeholder unchanged if key not found
            });
        }

        public static List<string> ExtractSqlParameters(this string sql)
        {
            var parameters = new List<string>();
            //var regex = new Regex(@"(?<!@)@\w+");
            var regex = new Regex(@"(?<!')(?<!@)@\w+(?!')");
            foreach (Match match in regex.Matches(sql))
                parameters.Add(match.Value);
            return parameters;
        }
    }
}
