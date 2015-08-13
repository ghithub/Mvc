// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.Framework.Internal;

namespace Microsoft.AspNet.Mvc.ModelBinding
{
    public class RouteValueValueProviderFactory : IValueProviderFactory
    {
        public IValueProvider GetValueProvider(ValueProviderFactoryContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            return new DictionaryBasedValueProvider(BindingSource.Path, context.RouteValues);
        }
    }
}
