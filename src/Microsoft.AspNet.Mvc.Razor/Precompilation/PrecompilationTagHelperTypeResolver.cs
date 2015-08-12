// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Microsoft.AspNet.Razor.Runtime.TagHelpers;
using Microsoft.CodeAnalysis;
using Microsoft.Framework.Internal;

namespace Microsoft.AspNet.Mvc.Razor.Precompilation
{
    /// <summary>
    /// <see cref="TagHelperDescriptorResolver"/> used during Razor precompilation.
    /// </summary>
    public class PrecompilationTagHelperTypeResolver : TagHelperTypeResolver
    {
        private readonly object _assemblyLookupLock = new object();
        private readonly Dictionary<string, IEnumerable<ITypeInfo>> _assemblyLookup
            = new Dictionary<string, IEnumerable<ITypeInfo>>(StringComparer.OrdinalIgnoreCase);
        private readonly CodeAnalysis.Compilation _compilation;
        private INamedTypeSymbol _openGenericDictionaryTypeSymbol;

        public PrecompilationTagHelperTypeResolver([NotNull] CodeAnalysis.Compilation compilation)
        {
            _compilation = compilation;
        }

        /// <inheritdoc />
        protected override IEnumerable<ITypeInfo> GetExportedTypes([NotNull] AssemblyName assemblyName)
        {
            lock (_assemblyLookup)
            {
                IEnumerable<ITypeInfo> result;
                if (!_assemblyLookup.TryGetValue(assemblyName.Name, out result))
                {
                    result = GetExportedTypes(assemblyName.Name);
                    _assemblyLookup[assemblyName.Name] = result;
                }

                return result;
            }
        }

        private INamedTypeSymbol OpenGenericDictionaryType
        {
            get
            {
                if (_openGenericDictionaryTypeSymbol == null)
                {
                    _openGenericDictionaryTypeSymbol = _compilation.GetTypeByMetadataName(
                        typeof(IDictionary<,>).FullName);

                    Debug.Assert(_openGenericDictionaryTypeSymbol != null);
                }

                return _openGenericDictionaryTypeSymbol;
            }
        }

        // Internal for unit testing
        internal IEnumerable<ITypeInfo> GetExportedTypes(string assemblyName)
        {
            if (string.Equals(_compilation.AssemblyName, assemblyName, StringComparison.Ordinal))
            {
                return GetExportedTypes(_compilation.Assembly);
            }
            else
            {
                foreach (var reference in _compilation.References)
                {
                    var compilationReference = reference as CompilationReference;
                    if (compilationReference != null &&
                        string.Equals(
                            compilationReference.Compilation.AssemblyName,
                            assemblyName,
                            StringComparison.Ordinal))
                    {
                        return GetExportedTypes(compilationReference.Compilation.Assembly);
                    }

                    var assemblyReference = reference as PortableExecutableReference;
                    if (assemblyReference != null)
                    {
                        var assemblyOrModuleSymbol = _compilation.GetAssemblyOrModuleSymbol(reference);
                        if (assemblyOrModuleSymbol != null &&
                            assemblyOrModuleSymbol.Kind == SymbolKind.Assembly)
                        {
                            var assemblySymbol = (IAssemblySymbol)assemblyOrModuleSymbol;
                            if (string.Equals(
                                    assemblySymbol.Identity.Name,
                                    assemblyName,
                                    StringComparison.Ordinal))
                            {
                                return GetExportedTypes(assemblySymbol);
                            }
                        }
                    }
                }

                return Enumerable.Empty<ITypeInfo>();
            }
        }

        private List<ITypeInfo> GetExportedTypes(IAssemblySymbol assembly)
        {
            var exportedTypes = new List<ITypeInfo>();
            GetExportedTypes(assembly.GlobalNamespace, exportedTypes);
            return exportedTypes;
        }

        private void GetExportedTypes(INamespaceSymbol namespaceSymbol, List<ITypeInfo> exportedTypes)
        {
            foreach (var type in namespaceSymbol.GetTypeMembers().Where(IsPublicType))
            {
                exportedTypes.Add(new SymbolBasedTypeInfo(type, OpenGenericDictionaryType));

                // TODO: Nested types get eventually filtered out. Could we make it part of the contract?
                var nestedPublicTypes = type.GetTypeMembers()
                    .Where(IsPublicType)
                    .Select(nestedType => new SymbolBasedTypeInfo(nestedType, OpenGenericDictionaryType));

                exportedTypes.AddRange(nestedPublicTypes);
            }

            foreach (var subNamespace in namespaceSymbol.GetNamespaceMembers())
            {
                GetExportedTypes(subNamespace, exportedTypes);
            }
        }

        private static bool IsPublicType(INamedTypeSymbol type) =>
            type.TypeKind == TypeKind.Class &&
            type.DeclaredAccessibility == Accessibility.Public;
    }
}