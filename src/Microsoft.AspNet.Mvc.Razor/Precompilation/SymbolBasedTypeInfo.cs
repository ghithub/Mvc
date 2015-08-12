// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.AspNet.Razor.Runtime.TagHelpers;
using Microsoft.CodeAnalysis;

namespace Microsoft.AspNet.Mvc.Razor.Precompilation
{
    [DebuggerDisplay("{FullName}")]
    public class SymbolBasedTypeInfo : ITypeInfo
    {
        private readonly ITypeSymbol _type;
        private readonly INamedTypeSymbol _openGenericDictionaryTypeSymbol;
        private string _fullName;

        public SymbolBasedTypeInfo(
            ITypeSymbol type,
            INamedTypeSymbol openGenericDictionaryTypeSymbol)
        {
            _type = type;
            _openGenericDictionaryTypeSymbol = openGenericDictionaryTypeSymbol;
        }

        public string FullName
        {
            get
            {
                if (_fullName == null)
                {
                    _fullName = SymbolUtilities.GetFullName(_type);
                }

                return _fullName;
            }
        }

        public bool IsAbstract => _type.IsAbstract;

        public bool IsGenericType => _type.Kind == SymbolKind.NamedType && ((INamedTypeSymbol)_type).IsGenericType;

        public bool IsNested => _type.ContainingType != null;

        public bool IsPublic => _type.DeclaredAccessibility == Accessibility.Public;

        public string Name => _type.Name;

        public IEnumerable<IPropertyInfo> Properties =>
            _type.GetMembers()
                .Where(member => member.Kind == SymbolKind.Property &&
                    member.DeclaredAccessibility == Accessibility.Public)
                .Cast<IPropertySymbol>()
                .Select(property => new SymbolBasedPropertyInfo(property, _openGenericDictionaryTypeSymbol));

        public IEnumerable<TAttribute> GetCustomAttributes<TAttribute>() where TAttribute : Attribute =>
            SymbolUtilities.GetCustomAttributes<TAttribute>(_type);

        public bool ImplementsInterface(System.Reflection.TypeInfo interfaceType) =>
            _type
                .AllInterfaces
                .Any(implementedInteface =>
                    string.Equals(
                        SymbolUtilities.GetFullName(implementedInteface),
                        interfaceType.FullName,
                        StringComparison.Ordinal));

        public string[] GetGenericDictionaryParameterNames()
        {
            INamedTypeSymbol dictionaryInterface;
            if (_type.Kind == SymbolKind.NamedType &&
                IsDictionaryType((INamedTypeSymbol)_type))
            {
                dictionaryInterface = (INamedTypeSymbol)_type;
            }
            else
            {
                dictionaryInterface = _type.AllInterfaces.FirstOrDefault(IsDictionaryType);
            }

            if (dictionaryInterface != null &&
                dictionaryInterface.TypeArguments.Length == 2)
            {
                return new[]
                {
                    SymbolUtilities.GetFullName(dictionaryInterface.TypeArguments[0]),
                    SymbolUtilities.GetFullName(dictionaryInterface.TypeArguments[1])
                };
            }

            return null;
        }

        private bool IsDictionaryType(INamedTypeSymbol implementedInteface) =>
            implementedInteface.ConstructedFrom == _openGenericDictionaryTypeSymbol;
    }
}