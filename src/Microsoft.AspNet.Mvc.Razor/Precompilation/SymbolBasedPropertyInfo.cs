// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.AspNet.Razor.Runtime.TagHelpers;
using Microsoft.CodeAnalysis;
using Microsoft.Framework.Internal;

namespace Microsoft.AspNet.Mvc.Razor.Precompilation
{
    [DebuggerDisplay("{Name, PropertyType}")]
    public class SymbolBasedPropertyInfo : IPropertyInfo
    {
        private readonly IPropertySymbol _propertySymbol;
        private readonly INamedTypeSymbol _openGenericDictionaryTypeSymbol;

        public SymbolBasedPropertyInfo(
            [NotNull] IPropertySymbol propertySymbol,
            [NotNull] INamedTypeSymbol openGenericDictionaryTypeSymbol)
        {
            _propertySymbol = propertySymbol;
            _openGenericDictionaryTypeSymbol = openGenericDictionaryTypeSymbol;
        }

        public bool HasPublicGetter =>
            _propertySymbol.GetMethod != null &&
            _propertySymbol.GetMethod.DeclaredAccessibility == Accessibility.Public;

        public bool HasPublicSetter =>
            _propertySymbol.SetMethod != null &&
            _propertySymbol.SetMethod.DeclaredAccessibility == Accessibility.Public;

        public string Name => _propertySymbol.Name;

        public ITypeInfo PropertyType =>
            new SymbolBasedTypeInfo(_propertySymbol.Type, _openGenericDictionaryTypeSymbol);

        public IEnumerable<TAttribute> GetCustomAttributes<TAttribute>() where TAttribute : Attribute
            => SymbolUtilities.GetCustomAttributes<TAttribute>(_propertySymbol);
    }
}
