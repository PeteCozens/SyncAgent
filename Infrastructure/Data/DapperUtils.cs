using Common.Interfaces;
using Dapper;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Infrastructure.Data
{
    [ExcludeFromCodeCoverage]
    internal static class DapperUtils
    {

        /// <summary>
        /// Finds any models that implement IDapper and maps any properties with the [Column] attribute to the specified database fields
        /// </summary>
        public static void MapColumnAttributesForDapper()
        {
            var tdm = typeof(IDapperModel);
            var tca = typeof(ColumnAttribute);

            var types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly =>
                {
                    try
                    {
                        return assembly.GetTypes();
                    }
                    catch (ReflectionTypeLoadException ex)
                    {
                        return ex.Types.Where(type => type != null);
                    }
                    catch
                    {
                        return Type.EmptyTypes;
                    }
                })
                .Where(tdm.IsAssignableFrom);

            foreach (var type in types)
            {
                if (type == null)
                    continue;

                var props = type.GetProperties();
                if (props == null)
                    continue;

                if (!props.Any(prop => Attribute.IsDefined(prop, tca)))
                    continue;

#pragma warning disable CS8603 // Possible null reference return.
                var map = new CustomPropertyTypeMap(type, (type, columnName)
                  => props.FirstOrDefault(prop => prop.GetColumnName().Equals(columnName, StringComparison.OrdinalIgnoreCase)));
#pragma warning restore CS8603 // Possible null reference return.

                SqlMapper.SetTypeMap(type, map);
            }
        }

        private static string GetColumnName(this MemberInfo member)
        {
            if (member == null)
                return string.Empty;

            var attrib = Attribute.GetCustomAttribute(member, typeof(ColumnAttribute), false) as ColumnAttribute;
            return (attrib?.Name ?? member.Name).ToLower();
        }
    }
}
