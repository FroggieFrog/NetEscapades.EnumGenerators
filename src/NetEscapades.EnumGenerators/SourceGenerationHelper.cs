using System.Text;

namespace NetEscapades.EnumGenerators;

public static class SourceGenerationHelper
{
    private const string Header = @"//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by the NetEscapades.EnumGenerators source generator
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

#nullable enable";

    public const string Attribute = Header + @"

#if NETESCAPADES_ENUMGENERATORS_EMBED_ATTRIBUTES
namespace NetEscapades.EnumGenerators
{
    /// <summary>
    /// Add to enums to indicate that extension methods should be generated for the type
    /// </summary>
    [System.AttributeUsage(System.AttributeTargets.Enum)]
    [System.Diagnostics.Conditional(""NETESCAPADES_ENUMGENERATORS_USAGES"")]
    public class EnumExtensionsAttribute : System.Attribute
    {
        /// <summary>
        /// The namespace to generate the extension class.
        /// If not provided the namespace of the enum will be used
        /// </summary>
        public string? ExtensionClassNamespace { get; set; }

        /// <summary>
        /// The name to use for the extension class.
        /// If not provided, the enum name with ""Extensions"" will be used.
        /// For example for an Enum called StatusCodes, the default name
        /// will be StatusCodesExtensions
        /// </summary>
        public string? ExtensionClassName { get; set; }
    }
}
#endif
";
    public static string GenerateExtensionClass(StringBuilder sb, EnumToGenerate enumToGenerate)
    {
        sb.Append(Header);

        if (!string.IsNullOrEmpty(enumToGenerate.Namespace))
        {
            sb.Append(@"
namespace ").Append(enumToGenerate.Namespace).Append(@"
{");
        }

        sb.Append(@"
    ").Append(enumToGenerate.IsPublic ? "public" : "internal").Append(@" static partial class ").Append(enumToGenerate.Name).Append(@"
    {
        public static string ToStringFast(this ").Append(enumToGenerate.FullyQualifiedName).Append(@" value)
            => value switch
            {");
        foreach (var member in enumToGenerate.Names)
        {
            sb.Append(@"
                ").Append(enumToGenerate.FullyQualifiedName).Append('.').Append(member.Key)
                .Append(" => ");

            if (member.Value is null)
            {
                sb.Append("nameof(").Append(enumToGenerate.FullyQualifiedName).Append('.').Append(member.Key).Append("),");
            }
            else
            {
                sb.Append('"').Append(member.Value).Append(@""",");
            }
        }

        sb.Append(@"
                _ => value.ToString(),
            };");

        if (enumToGenerate.HasFlags)
        {
            sb.Append(@"

        public static bool HasFlag(this ").Append(enumToGenerate.FullyQualifiedName).Append(@" value, ").Append(enumToGenerate.FullyQualifiedName).Append(@" flag)
            => value switch
            {
                0  => flag.Equals(0),
                _ => (value & flag) != 0,
            };");
        }

        sb.Append(@"

       public static bool IsDefined(").Append(enumToGenerate.FullyQualifiedName).Append(@" value)
            => value switch
            {");
        foreach (var member in enumToGenerate.Names)
        {
            sb.Append(@"
                ").Append(enumToGenerate.FullyQualifiedName).Append('.').Append(member.Key)
                .Append(" => true,");
        }
        sb.Append(@"
                _ => false,
            };");

        if (enumToGenerate.IsDisplaAttributeUsed)
        {
            sb.Append(@"

        public static bool IsDefined(string name, bool allowMatchingDisplayAttribute = false)
        {
            var isDefinedInDisplayAttribute = false;
            if (allowMatchingDisplayAttribute)
            {
                isDefinedInDisplayAttribute = name switch
                {");
            foreach (var member in enumToGenerate.Names)
            {
                if (member.Value is not null)
                {
                    sb.Append(@"
                    """).Append(member.Value).Append(@""" => true,");
                }
            }

            sb.Append(@"
                    _ => false,
                };
            }

            if (isDefinedInDisplayAttribute)
            {
                return true;
            }

            return name switch
            {");
            foreach (var member in enumToGenerate.Names)
            {
                sb.Append(@"
                nameof(").Append(enumToGenerate.FullyQualifiedName).Append('.').Append(member.Key).Append(@") => true,");
            }

            sb.Append(@"
                _ => false,
            };
        }");
        }
        else
        {
            sb.Append(@"

        public static bool IsDefined(string name)
            => name switch
            {");
            foreach (var member in enumToGenerate.Names)
            {
                sb.Append(@"
                nameof(").Append(enumToGenerate.FullyQualifiedName).Append('.').Append(member.Key).Append(@") => true,");
            }

            sb.Append(@"
                _ => false,
            };");
        }

        if (enumToGenerate.IsDisplaAttributeUsed)
        {
            sb.Append(@"

        public static bool TryParse(
#if NETCOREAPP3_0_OR_GREATER
            [System.Diagnostics.CodeAnalysis.NotNullWhen(true)]
#endif
            string? name, 
            bool ignoreCase, 
            out ").Append(enumToGenerate.FullyQualifiedName).Append(@" value, 
            bool allowMatchingDisplayAttribute = false)
            => ignoreCase ? TryParse(name, System.StringComparison.OrdinalIgnoreCase, out value, allowMatchingDisplayAttribute) : TryParse(name, System.StringComparison.Ordinal, out value, allowMatchingDisplayAttribute);

        private static bool TryParse(
#if NETCOREAPP3_0_OR_GREATER
            [System.Diagnostics.CodeAnalysis.NotNullWhen(true)]
#endif
            string? name, 
            System.StringComparison stringComparisonOption, 
            out ").Append(enumToGenerate.FullyQualifiedName).Append(@" value, 
            bool allowMatchingDisplayAttribute)
        {
            if (allowMatchingDisplayAttribute)
            {
                switch (name)
                {");
            foreach (var member in enumToGenerate.Names)
            {
                if (member.Value is not null)
                {
                    sb.Append(@"
                    case { } s when s.Equals(""").Append(member.Value).Append(@""", stringComparisonOption):
                        value = ").Append(enumToGenerate.FullyQualifiedName).Append('.').Append(member.Key).Append(@";
                        return true;");
                }
            }

            sb.Append(@"
                    default:
                        break;
                };
            }

            switch (name)
            {");
            foreach (var member in enumToGenerate.Names)
            {
                sb.Append(@"
                case { } s when s.Equals(nameof(").Append(enumToGenerate.FullyQualifiedName).Append('.').Append(member.Key).Append(@"), stringComparisonOption):
                    value = ").Append(enumToGenerate.FullyQualifiedName).Append('.').Append(member.Key).Append(@";
                    return true;");
            }

            sb.Append(@"
                case { } s when ").Append(enumToGenerate.UnderlyingType).Append(@".TryParse(name, out var val):
                    value = (").Append(enumToGenerate.FullyQualifiedName).Append(@")val;
                    return true;
                default:
                    value = default;
                    return false;
            }
        }

        public static bool TryParse(
#if NETCOREAPP3_0_OR_GREATER
            [System.Diagnostics.CodeAnalysis.NotNullWhen(true)]
#endif
            string? name, 
            out ").Append(enumToGenerate.FullyQualifiedName).Append(@" value, 
            bool allowMatchingDisplayAttribute = false)
            => TryParse(name, System.StringComparison.Ordinal, out value, allowMatchingDisplayAttribute);");
        }
        else
        {
            sb.Append(@"

        public static bool TryParse(
#if NETCOREAPP3_0_OR_GREATER
            [System.Diagnostics.CodeAnalysis.NotNullWhen(true)]
#endif
            string? name, 
            bool ignoreCase, 
            out ").Append(enumToGenerate.FullyQualifiedName).Append(@" value)
            => ignoreCase ? TryParse(name, System.StringComparison.OrdinalIgnoreCase, out value) : TryParse(name, System.StringComparison.Ordinal, out value);

        private static bool TryParse(
#if NETCOREAPP3_0_OR_GREATER
            [System.Diagnostics.CodeAnalysis.NotNullWhen(true)]
#endif
            string? name, 
            System.StringComparison stringComparisonOption, 
            out ").Append(enumToGenerate.FullyQualifiedName).Append(@" value)
        {
            switch (name)
            {");
            foreach (var member in enumToGenerate.Names)
            {
                sb.Append(@"
                case { } s when s.Equals(nameof(").Append(enumToGenerate.FullyQualifiedName).Append('.').Append(member.Key).Append(@"), stringComparisonOption):
                    value = ").Append(enumToGenerate.FullyQualifiedName).Append('.').Append(member.Key).Append(@";
                    return true;");
            }

            sb.Append(@"
                case { } s when ").Append(enumToGenerate.UnderlyingType).Append(@".TryParse(name, out var val):
                    value = (").Append(enumToGenerate.FullyQualifiedName).Append(@")val;
                    return true;
                default:
                    value = default;
                    return false;
            }
        }

        public static bool TryParse(
#if NETCOREAPP3_0_OR_GREATER
            [System.Diagnostics.CodeAnalysis.NotNullWhen(true)]
#endif
            string? name, 
            out ").Append(enumToGenerate.FullyQualifiedName).Append(@" value)
            => TryParse(name, System.StringComparison.Ordinal, out value);");
        }

        sb.Append(@"

        public static ").Append(enumToGenerate.FullyQualifiedName).Append(@"[] GetValues()
        {
            return new[]
            {");
        foreach (var member in enumToGenerate.Names)
        {
            sb.Append(@"
                ").Append(enumToGenerate.FullyQualifiedName).Append('.').Append(member.Key).Append(',');
        }

        sb.Append(@"
            };
        }");

        sb.Append(@"

        public static string[] GetNames()
        {
            return new[]
            {");
        foreach (var member in enumToGenerate.Names)
        {
            sb.Append(@"
                nameof(").Append(enumToGenerate.FullyQualifiedName).Append('.').Append(member.Key).Append("),");
        }
        sb.Append(@"
            };
        }
    }");
        if (!string.IsNullOrEmpty(enumToGenerate.Namespace))
        {
            sb.Append(@"
}");
        }

        return sb.ToString();
    }
}