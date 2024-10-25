using System.Reflection;

namespace Common.Extensions
{
    public static class AssemblyExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="assembly">Can be found by calling typeof(T).Assembly;</param>
        /// <returns></returns>
        public static IEnumerable<string> GetEmbeddedResourceNames(this Assembly assembly)
        {
            return assembly.GetManifestResourceNames();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="assembly">Can be found by calling typeof(T).Assembly;</param>
        /// <param name="resourceName"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static Stream? GetEmbeddedResourceStream(this Assembly assembly, string resourceName)
        {
            var assemblyName = assembly.GetName()?.Name 
                ?? throw new Exception("Unable to get the assembly name");

            if (!resourceName.StartsWith(assemblyName))
                resourceName = $"{assemblyName}.{resourceName}";

            var stream = assembly.GetManifestResourceStream(resourceName);
            if (stream != null)
                return stream;

            throw new Exception(@$"Resource {resourceName} not found. Please check the name and ensure that you've set the file's ""Build Action"" to ""Embedded Resource""");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="assembly">Can be found by calling typeof(T).Assembly;</param>
        /// <param name="resourceName"></param>
        /// <returns></returns>
        public static string GetEmbeddedResourceString(this Assembly assembly, string resourceName)
        {
            using var stream = assembly.GetEmbeddedResourceStream(resourceName) 
                ?? throw new Exception(@$"Resource {resourceName} not found. Please check the name and ensure that you've set the file's ""Build Action"" to ""Embedded Resource""");
            using var reader = new StreamReader(stream);
            return reader.ReadToEnd();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="assembly">Can be found by calling typeof(T).Assembly;</param>
        /// <param name="resourceName"></param>
        /// <returns></returns>
        public static byte[] GetEmbeddedResourceData(this Assembly assembly, string resourceName)
        {
            using var stream = assembly.GetEmbeddedResourceStream(resourceName)
                ?? throw new Exception(@$"Resource {resourceName} not found. Please check the name and ensure that you've set the file's ""Build Action"" to ""Embedded Resource""");
            using var reader = new BinaryReader(stream);
            return reader.ReadBytes((int)stream.Length);
        }
    }
}
