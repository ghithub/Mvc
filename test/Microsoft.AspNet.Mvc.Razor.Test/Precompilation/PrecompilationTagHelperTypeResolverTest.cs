// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.AspNet.Mvc.Razor.Compilation;
using Microsoft.AspNet.Razor.Runtime.TagHelpers;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.Dnx.Compilation;
using Microsoft.Dnx.Runtime.Infrastructure;
using Microsoft.Framework.DependencyInjection;
using Xunit;

namespace Microsoft.AspNet.Mvc.Razor.Precompilation
{
    public class PrecompilationTagHelperTypeResolverTest
    {
        private static readonly string TypeNamespace = typeof(PrecompilationTagHelperTypeResolverTest).Namespace;
        private static readonly Assembly ExecutingAssembly = typeof(PrecompilationTagHelperTypeResolverTest).Assembly;
        private static readonly TypeInfo TagHelperTypeInfo = typeof(ITagHelper).GetTypeInfo();

        [Theory]
        [InlineData("TypeDerivingFromITagHelper",
            "Microsoft.AspNet.Mvc.Razor.Precompilation.TypeDerivingFromITagHelper")]
        [InlineData("TypeInGlobalNamespace", "TypeInGlobalNamespace")]
        public void TypesReturnedFromGetExportedTypes_ReturnTrueForImplmentsInterfaceTest(
            string typeName,
            string fullName)
        {
            // Arrange
            var compilation = GetCompilation($"{typeName}.cs");
            var tagHelperResolver = new PrecompilationTagHelperTypeResolver(compilation);

            // Act
            var exportedTypes = tagHelperResolver.GetExportedTypes(compilation.AssemblyName);

            // Assert
            var exportedType = Assert.Single(exportedTypes);
            Assert.True(exportedType.IsPublic);
            Assert.False(exportedType.IsNested);
            Assert.False(exportedType.IsAbstract);
            Assert.False(exportedType.IsGenericType);
            Assert.Equal(typeName, exportedType.Name);
            Assert.Equal(fullName, exportedType.FullName);
            Assert.True(exportedType.ImplementsInterface(TagHelperTypeInfo));
            Assert.Null(exportedType.GetGenericDictionaryParameterNames());

            var property = Assert.Single(exportedType.Properties);
            Assert.Equal("Order", property.Name);
            Assert.False(property.HasPublicSetter);
            Assert.True(property.HasPublicGetter);
            Assert.Equal(typeof(int).FullName, property.PropertyType.FullName);
            Assert.Null(property.PropertyType.GetGenericDictionaryParameterNames());
        }

        [Fact]
        public void GetExportedTypes_ReturnsRegularAndNestedPublicTypes()
        {
            // Arrange
            var compilation = GetCompilation("AssemblyWithNonPublicTypes.cs");
            var tagHelperResolver = new PrecompilationTagHelperTypeResolver(compilation);

            // Act
            var exportedTypes = tagHelperResolver.GetExportedTypes(compilation.AssemblyName);

            // Assert
            Action<ITypeInfo> commonAsserts = type =>
            {
                Assert.True(type.IsPublic);
                Assert.False(type.IsAbstract);
                Assert.False(type.IsGenericType);
                Assert.Empty(type.Properties);
                Assert.False(type.ImplementsInterface(TagHelperTypeInfo));
                Assert.Null(type.GetGenericDictionaryParameterNames());
            };

            Assert.Collection(exportedTypes,
                type =>
                {
                    Assert.Equal("PublicType", type.Name);
                    Assert.Equal(TypeNamespace + ".PublicType", type.FullName);
                    Assert.False(type.IsNested);
                    commonAsserts(type);
                },
                type =>
                {
                    Assert.Equal("ContainerType", type.Name);
                    Assert.Equal(TypeNamespace + ".ContainerType", type.FullName);
                    Assert.False(type.IsNested);
                    commonAsserts(type);
                },
                type =>
                {
                    Assert.Equal("NestedType", type.Name);
                    Assert.Equal(TypeNamespace + ".ContainerType+NestedType", type.FullName);
                    Assert.True(type.IsNested);
                    commonAsserts(type);
                });
        }

        [Fact]
        public void GetExportedTypes_PopulatesAttributes()
        {
            // Arrange
            var compilation = GetCompilation("TypeWithAttributes.cs");
            var tagHelperResolver = new PrecompilationTagHelperTypeResolver(compilation);

            // Act
            var exportedTypes = tagHelperResolver.GetExportedTypes(compilation.AssemblyName);

            // Assert
            var type = Assert.Single(exportedTypes);
            Assert.Equal("TypeWithAttributes", type.Name);
            Assert.Collection(type.GetCustomAttributes<TargetElementAttribute>(),
                attribute =>
                {
                    Assert.Equal("img", attribute.Tag);
                    Assert.Equal("asp-append-version,src", attribute.Attributes);
                },
                attribute =>
                {
                    Assert.Equal("image", attribute.Tag);
                    Assert.Equal("src", attribute.Attributes);
                });

            var editorBrowsable = Assert.Single(type.GetCustomAttributes<EditorBrowsableAttribute>());
            Assert.Equal(EditorBrowsableState.Never, editorBrowsable.State);

            Assert.Collection(type.Properties,
                property =>
                {
                    Assert.Equal("Src", property.Name);
                    var attributeName = Assert.Single(property.GetCustomAttributes<HtmlAttributeNameAttribute>());
                    Assert.Equal("src", attributeName.Name);
                    Assert.Null(attributeName.DictionaryAttributePrefix);
                },
                property =>
                {
                    Assert.Equal("AppendVersion", property.Name);
                    var attributeName = Assert.Single(property.GetCustomAttributes<HtmlAttributeNameAttribute>());
                    Assert.Equal("asp-append-version", attributeName.Name);
                    Assert.Equal("prefix", attributeName.DictionaryAttributePrefix);
                },
                property =>
                {
                    Assert.Equal("ViewContext", property.Name);
                    Assert.Equal(typeof(ViewContext).FullName, property.PropertyType.FullName);
                    Assert.Empty(property.GetCustomAttributes<HtmlAttributeNameAttribute>());
                    Assert.Single(property.GetCustomAttributes<ViewContextAttribute>());
                    Assert.Single(property.GetCustomAttributes<HtmlAttributeNotBoundAttribute>());
                    editorBrowsable = Assert.Single(property.GetCustomAttributes<EditorBrowsableAttribute>());
                    Assert.Equal(EditorBrowsableState.Advanced, editorBrowsable.State);
                });
        }

        [Fact]
        public void GetExportedTypes_CorrectlyIdentifiesIfTypeDerivesFromDictionary()
        {
            // Arrange
            var compilation = GetCompilation("TypeWithDictionaryProperties.cs");
            var tagHelperResolver = new PrecompilationTagHelperTypeResolver(compilation);

            // Act
            var exportedTypes = tagHelperResolver.GetExportedTypes(compilation.AssemblyName);

            // Assert
            var type = exportedTypes.First();
            Assert.Equal("TypeWithDictionaryProperties", type.Name);

            Assert.Collection(type.Properties,
                property =>
                {
                    Assert.Equal("RouteValues1", property.Name);
                    Assert.Equal(new[] { typeof(string).FullName, typeof(string).FullName },
                        property.PropertyType.GetGenericDictionaryParameterNames());
                },
                property =>
                {
                    Assert.Equal("RouteValues2", property.Name);
                    Assert.Equal(new[] { typeof(int).FullName, typeof(string).FullName },
                        property.PropertyType.GetGenericDictionaryParameterNames());
                },
                property =>
                {
                    Assert.Equal("RouteValues3", property.Name);
                    Assert.Equal(new[] { "System.Collections.Generic.List", typeof(float).FullName },
                        property.PropertyType.GetGenericDictionaryParameterNames());
                },
                property =>
                {
                    Assert.Equal("CustomDictionary", property.Name);
                    Assert.Equal(new[] { typeof(string).FullName, $"{TypeNamespace}.CustomType" },
                        property.PropertyType.GetGenericDictionaryParameterNames());
                },
                property =>
                {
                    Assert.Equal("NonGenericDictionary", property.Name);
                    Assert.Null(property.PropertyType.GetGenericDictionaryParameterNames());
                },
                property =>
                {
                    Assert.Equal("ObjectType", property.Name);
                    Assert.Null(property.PropertyType.GetGenericDictionaryParameterNames());
                });
        }

        private CodeAnalysis.Compilation GetCompilation(string fileName)
        {
            var resourceText = ResourceFile.ReadResource(
                ExecutingAssembly,
                $"Precompilation.Files.{fileName}",
                sourceFile: true);

            var syntaxTree = CSharpSyntaxTree.ParseText(resourceText);
            var references = RoslynCompilationService.GetApplicationReferences(
                new ConcurrentDictionary<string, CodeAnalysis.AssemblyMetadata>(StringComparer.Ordinal),
                CallContextServiceLocator.Locator.ServiceProvider.GetService<ILibraryExporter>(),
                ExecutingAssembly.GetName().Name);

            return CSharpCompilation.Create(
                Path.GetRandomFileName(),
                new[] { syntaxTree },
                references);
        }
    }
}
