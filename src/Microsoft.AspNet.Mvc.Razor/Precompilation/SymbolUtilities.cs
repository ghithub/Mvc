using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
#if DNXCORE50
using System.Reflection;
#endif
using Microsoft.CodeAnalysis;
using Microsoft.Framework.Internal;

namespace Microsoft.AspNet.Mvc.Razor.Precompilation
{
    public static class SymbolUtilities
    {
        public static IEnumerable<TAttribute> GetCustomAttributes<TAttribute>(ISymbol symbol) where TAttribute : Attribute
        {
            var attributes = symbol.GetAttributes();
            var attributeFullName = typeof(TAttribute).FullName;
            return attributes
                .Where(
                    attribute => string.Equals(GetFullName(attribute.AttributeClass), attributeFullName, StringComparison.Ordinal))
                .Select(a => CreateAttribute<TAttribute>(a));
        }

        public static string GetFullName(ITypeSymbol typeSymbol)
        {
            var nameBuilder = new StringBuilder(typeSymbol.Name);
            if (typeSymbol.ContainingType != null)
            {
                nameBuilder.Insert(0, '+')
                    .Insert(0, typeSymbol.ContainingType.Name);
            }

            var containingNamespace = typeSymbol.ContainingNamespace;
            while (containingNamespace != null && !containingNamespace.IsGlobalNamespace)
            {
                nameBuilder.Insert(0, '.')
                    .Insert(0, containingNamespace.Name);

                containingNamespace = containingNamespace.ContainingNamespace;
            }

            return nameBuilder.ToString();
        }

        private static TAttribute CreateAttribute<TAttribute>(AttributeData attributeData) where TAttribute : Attribute
        {
            var constructorArguments = GetConstructorArguments(attributeData);
            var attribute = (TAttribute)Activator.CreateInstance(typeof(TAttribute), constructorArguments);

            if (attributeData.NamedArguments.Length > 0)
            {
                var helpers = PropertyHelper.GetVisibleProperties(attribute);
                foreach (var item in attributeData.NamedArguments)
                {
                    var helper = helpers.FirstOrDefault(h => string.Equals(h.Name, item.Key, StringComparison.Ordinal));
                    helper?.SetValue(attribute, item.Value.Value);
                }
            }

            return attribute;
        }

        private static object[] GetConstructorArguments(AttributeData attributeData)
        {
            var arguments = new object[attributeData.ConstructorArguments.Length];
            for (var i = 0; i < arguments.Length; i++)
            {
                object value;
                var constructorArgument = attributeData.ConstructorArguments[i];
                switch  (constructorArgument.Kind)
                {
                    case TypedConstantKind.Enum:
                        var enumType = Type.GetType(
                            $"{constructorArgument.Type},{constructorArgument.Type.ContainingAssembly.Identity}", 
                            throwOnError: true);
                        value = Enum.Parse(enumType, constructorArgument.Value.ToString());
                        break;
                    case TypedConstantKind.Primitive:
                    case TypedConstantKind.Type:
                        value = constructorArgument.Value;
                        break;
                    default:
                        throw new NotSupportedException();
                }

                arguments[i] = value;
            }

            return arguments;
        }
    }
}
