﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Globalization;
using Microsoft.AspNet.Http.Internal;

namespace Microsoft.AspNet.Mvc.ModelBinding
{
    public class ReadableStringCollectionValueProviderTest : EnumerableValueProviderTest
    {
        protected override IEnumerableValueProvider GetEnumerableValueProvider(
            BindingSource bindingSource,
            IDictionary<string, string[]> values,
            CultureInfo culture)
        {
            var backingStore = new ReadableStringCollection(values);
            return new ReadableStringCollectionValueProvider(bindingSource, backingStore, culture);
        }
    }
}
