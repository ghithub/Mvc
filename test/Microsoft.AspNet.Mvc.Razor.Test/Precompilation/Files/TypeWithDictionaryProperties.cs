// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.AspNet.Razor.Runtime.TagHelpers;

namespace Microsoft.AspNet.Mvc.Razor.Precompilation
{
    public class TypeWithDictionaryProperties
    {
        private const string RouteValuesDictionaryName = "route";
        private const string RouteValuesPrefix = "route-prefix";

        public IDictionary<string, string> RouteValues1 { get; set; } =
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        public Dictionary<int, string> RouteValues2 { get; set; } =
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        public ReadOnlyDictionary<List<string>, float> RouteValues3 { get; set; } =
            new Dictionary<List<string>, float>(StringComparer.OrdinalIgnoreCase);

        public CustomDictionary<string, CustomType> CustomDictionary { get; set; } =
            new CustomDictionary<string, CustomType>();

        public IDictionary NonGenericDictionary { get; set; } =
            new Dictionary<string, string>();

        public object ObjectType { get; set; } =
            new Dictionary<string, string>();
    }

    public class CustomDictionary<TKey, TValue> : Dictionary<TKey, TValue>
    {

    }

    public class CustomType
    {

    }
}
